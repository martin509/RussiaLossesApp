using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RussiaLossesApp.Data;
using RussiaLossesApp.Models;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RussiaLossesApp.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LossObjectContext _lossContext;
        public StatisticsController(ILogger<HomeController> logger, LossObjectContext lossContext)
        {
            _logger = logger;
            _lossContext = lossContext;
        }

        public IActionResult Index()
        {

            return View();
        }
        public async Task<List<LossListObject>> loadSummary(DateTime start, DateTime end)
        {
            var query = $"SELECT * FROM LOSSOBJECT WHERE " +
                $"DATE >='{start:yyyy-MM-dd}'" +
                $"AND DATE <='{end:yyyy-MM-dd}'";

            /*var query = $"SELECT lo.*, et.* FROM LOSSOBJECT lo " +
                $"INNER JOIN EquipTypes et ON lo.EquipTypeId = et.Id " +
                $"WHERE lo.DATE >= '{start:yyyy-MM-dd}' " +
                $"AND lo.DATE <= '{end:yyyy-MM-dd}'";*/
            var fquery = FormattableStringFactory.Create(query);
            //List<LossObject> entries = await _lossContext.LossObject.FromSql(fquery).Include(lo => lo.type).ToListAsync();
            List<LossObject> entries = await _lossContext.LossObject.Include(lo=>lo.EquipType).Where(lo => lo.date >= start && lo.date <= end).ToListAsync();

            Dictionary<string, LossListObject> dict = new Dictionary<string, LossListObject>();
            foreach (LossObject loss in entries)
            {
                if (dict.ContainsKey(loss.getModel()))
                {
                    dict.GetValueOrDefault(loss.getModel()).addStatus(loss.status);

                }
                else
                {
                    var listobj = new LossListObject(loss.getModel());
                    listobj.addStatus(loss.status);
                    listobj.type = loss.getType();
                    dict.Add(loss.getModel(), listobj);
                }
            }

            var summary = dict.Values.ToList();
            summary = summary.OrderBy(o=>o.type).ToList();
            return summary;
        }


        [HttpGet("summary_day")]
        public async Task<IActionResult> loadSummaryPeriod(DateTime date)
        {
            var summary = await loadSummary(date, date);

            TempData["month"] = $"{date:D}";
            return View("summary", summary);
        }

        [HttpGet("summary_range")]
        public async Task<IActionResult> loadSummaryPeriod(DateTime start, DateTime end)
        {
            var summary = await loadSummary(start, end);

            TempData["month"] = $"{start:D} to {end:D}";
            return View("summary", summary);
        }

        [HttpGet("summary_month")]
        public async Task<IActionResult> loadSummaryMonth(string month)
        {
            Debug.WriteLine($"Month: {month}");
            DateTime start = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);
            DateTime end = start.AddMonths(1);

            var summary = await loadSummary(start, end);

            TempData["month"] = $"{start:Y}";
            return View("summary", summary);
        }

        [HttpGet("graph_month")]
        public async Task<IActionResult> loadGraphMonth(string month, int avg)
        {
            Debug.WriteLine($"Month: {month}");
            DateTime start = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);
            DateTime end = start.AddMonths(1);

            var data = await loadRunningAverage(start, end, avg);//loadGraphRange(start, end);
            List<(string, float[])> graphValues = new List<(string, float[])>();
            foreach (var dictEntry in data)
            {
                graphValues.Add((dictEntry.Key, dictEntry.Value));
            }

            TempData["month"] = $"{start:Y}";
            return View("graph", graphValues);
        }

        public async Task<Dictionary<string, int[]>> loadGraphRange(DateTime start, DateTime end)
        {
            int days = end.Subtract(start).Days;
            Dictionary<string, int[]> graphValues = new Dictionary<string, int[]>();
            int n = 0;
            while(start < end)
            {
                //var query = $"SELECT * FROM LOSSOBJECT WHERE " +
                //$"DATE ='{start:yyyy-MM-dd}'";
                //var fquery = FormattableStringFactory.Create(query);
                var entries = await _lossContext.LossObject.Where(loss => loss.date == start).Include(loss => loss.EquipType).ToListAsync();
                
                foreach (LossObject lo in entries)
                {
                    if (!graphValues.ContainsKey(lo.getType()))
                    {
                        graphValues.Add(lo.getType(), new int[days]);
                    }
                    
                    graphValues.TryGetValue(lo.getType(), out int[] value);
                    value[n] = value[n] + 1;

                }
                n++;
                start = start.AddDays(1);
            }
            return graphValues;
        }

        public async Task<Dictionary<string, int[]>> loadGraphCategories(DateTime start, DateTime end, List<string> catlist)
        {
            int days = end.Subtract(start).Days;
            Dictionary<string, int[]> graphValues = new Dictionary<string, int[]>();
            int n = 0;
            //List<EquipCategory> categories = new List<EquipCategory>();

            foreach(string cat in catlist)
            {
                //var category = ((await _lossContext.EquipCategory.Include(cat => cat.EquipTypes).Where(ct => ct.Name == cat)
                //if(category != null) {
                  //  categories.Add(category);
                graphValues.Add(cat, new int[days]);
                //}
            }
            while (start < end)
            {
                //var query = $"SELECT * FROM LOSSOBJECT WHERE " +
                //$"DATE ='{start:yyyy-MM-dd}'";
                //var fquery = FormattableStringFactory.Create(query);
                foreach (string cat in catlist)
                {
                    int count = 0;
                    var category = _lossContext.EquipCategory.Where(ct => ct.Name == cat).Include(ct => ct.EquipTypes).ToList().First();
                    foreach(EquipType et in category.EquipTypes)
                    {
                        count += (await _lossContext.LossObject.Where(lo => lo.EquipType == et && lo.date == start).Include(loss => loss.EquipType).ToListAsync()).Count();
                    }
                    graphValues.TryGetValue(category.Name, out int[] value);
                    value[n] = count;
                }
                n++;
                start = start.AddDays(1);
            }
            return graphValues;
        }

        public async Task<Dictionary<string, float[]>> loadRunningAverage(DateTime start, DateTime end, int period)
        {
            DateTime realStart = start.AddDays(-period);
            Dictionary<string, int[]> graphValues = await loadGraphRange(realStart, end);
            Dictionary<string, float[]> graphAverages = new Dictionary<string, float[]>();
            foreach(var key in graphValues.Keys)
            {
                var value = graphValues[key];
                float[] averages = new float[value.Length - period];
                
                for(int n = 0; n < averages.Length; n++)
                {
                    float sum = 0;
                    for(int i = 0; i < period; i++)
                    {
                        sum += value[i + n];
                    }
                    averages[n] = sum / period;
                }
                graphAverages.Add(key, averages);
            }
            return graphAverages;
        }

        

        
    }
}
