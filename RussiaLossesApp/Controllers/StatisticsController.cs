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
            /*var query = $"SELECT * FROM LOSSOBJECT WHERE " +
                $"DATE >='{start:yyyy-MM-dd}'" +
                $"AND DATE <='{end:yyyy-MM-dd}'";*/

            /*var query = $"SELECT lo.*, et.* FROM LOSSOBJECT lo " +
                $"INNER JOIN EquipTypes et ON lo.EquipTypeId = et.Id " +
                $"WHERE lo.DATE >= '{start:yyyy-MM-dd}' " +
                $"AND lo.DATE <= '{end:yyyy-MM-dd}'";*/
            //var fquery = FormattableStringFactory.Create(query);
            //List<LossObject> entries = await _lossContext.LossObject.FromSql(fquery).Include(lo => lo.type).ToListAsync();
            var entriesHandle = _lossContext.LossObject.Include(lo => lo.EquipType).Where(lo => lo.date >= start && lo.date <= end).ToListAsync();
            
            Dictionary<string, LossListObject> dict = new Dictionary<string, LossListObject>();
            List<LossObject> entries = await entriesHandle;
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

        [HttpGet("Summary/summary_json")]
        public async Task<ActionResult> loadSummaryJson(DateTime start, DateTime end)
        {
            return Json(await loadSummary(start, end));
        }


        [HttpGet("summary_day")]
        public async Task<IActionResult> loadSummaryPeriod(DateTime date)
        {
            var summary = await loadSummary(date, date);

            TempData["month"] = $"{date:D}";
            TempData["start"] = $"{date:D}";
            TempData["end"] = $"{date:D}";
            return View("summary", summary);
        }

        [HttpGet("summary_range")]
        public async Task<IActionResult> loadSummaryPeriod(DateTime start, DateTime end)
        {
            var summary = await loadSummary(start, end);

            TempData["month"] = $"{start:D} to {end:D}";
            TempData["start"] = $"{start:D}";
            TempData["end"] = $"{end:D}";
            return View("summary", summary);
        }

        [HttpGet("summary_month")]
        public async Task<IActionResult> loadSummaryMonth(string month)
        {
            Debug.WriteLine($"Month: {month}");
            DateTime start = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);
            DateTime end = start.AddMonths(1);
            TempData["start"] = $"{start:D}";
            TempData["end"] = $"{end:D}";
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
            /*List<(string, float[])> graphValues = new List<(string, float[])>();
            foreach (var dictEntry in data)
            {
                graphValues.Add((dictEntry.Key, dictEntry.Value));
            }*/

            TempData["month"] = $"{start:Y}";
            return View("graph", convertToGraph(data));
        }

        [HttpPost("FilterByCategories")]
        public async Task<IActionResult> LoadGraphCategories(List<int> categoryIds, DateTime start, DateTime end)
        {
            var categories = await _lossContext.EquipCategory.Include(cat => cat.EquipTypes).Where(cat => categoryIds.Contains(cat.Id)).ToListAsync();
            var graphValues = await loadGraphCategories(start, end, categories);
            return View("graph", convertToGraph(graphValues));
        }
        /// <summary>
        /// Load a dataset for a line chart (dictionary of string, int[]) for equipment losses from start to end
        /// </summary>
        /// <param name="start">start day of chart</param>
        /// <param name="end">end day of chart</param>
        /// <returns>dictionary of (string, int[]) for loss counts per-day for each class of equipment</returns>
        public async Task<Dictionary<string, int[]>> loadGraphRange(DateTime start, DateTime end)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var allEntriesHandle = _lossContext.LossObject.Where(loss => loss.date >= start && loss.date <= end).Include(loss => loss.EquipType).ToListAsync();

            int days = end.Subtract(start).Days;
            Dictionary<string, int[]> graphValues = new Dictionary<string, int[]>();
            int n = 0;

            var allEntries = await allEntriesHandle;
            while (start < end)
            {

                var entries = allEntries.Where(loss => loss.date == start);
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
            sw.Stop();
            Debug.WriteLine($"Time taken for loadGraphRange call: {sw.Elapsed}");
            return graphValues;
        }

        /// <summary>
        /// Loads a dataset for a line chart (dictionary of int[]s) for loss counts within a given list of categories, within a given time window
        /// </summary>
        /// <param name="start">start of time window</param>
        /// <param name="end">end of time window</param>
        /// <param name="catlist">list of categories to include</param>
        /// <returns></returns>
        public async Task<Dictionary<string, int[]>> loadGraphCategories(DateTime start, DateTime end, List<EquipCategory> catlist)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var allLossesHandle = _lossContext.LossObject.Include(loss => loss.EquipType).Where(loss => loss.date >= start && loss.date <= end).ToListAsync();

            Dictionary<string, int[]> graphValues = new Dictionary<string, int[]>(); //basically, a table
            foreach (var cat in catlist)
            {
                graphValues.Add(cat.Name, new int[end.Subtract(start).Days]);
            }

            var allLosses = await allLossesHandle;

            foreach (var lo in allLosses)
            {
                foreach(var cat in catlist)
                {
                    if (cat.EquipTypes.Contains(lo.EquipType)) //for categories that the loss's equiptype fits into,
                    {
                        if(graphValues.TryGetValue(cat.Name, out int[] value))
                            ++value[lo.date.Subtract(start).Days]; //increment the count of losses at the specific day the loss occurred
                    }
                }
            }
            sw.Stop();
            Debug.WriteLine($"Time taken for loadGraphCategories call: {sw.Elapsed}, list size: {graphValues.Count} key/value pairs");
            /*foreach(KeyValuePair<string, int[]> pair in graphValues)
            {
                Debug.WriteLine($"Data for {pair.Key}: {string.Join(",", pair.Value)}");
            }*/
            return graphValues;
        }

        //TODO: re-implement in JS frontend (why is the controller doing this job?)
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

        private List<(string, float[])> convertToGraph(Dictionary<string, int[]> data)
        {
            List<(string, float[])> graphValues = new List<(string, float[])>();
            foreach (var dictEntry in data)
            {
                graphValues.Add((dictEntry.Key, Array.ConvertAll(dictEntry.Value, n => (float) n)));
            }
            return graphValues;
        }

        private List<(string, float[])> convertToGraph(Dictionary<string, float[]> data)
        {
            List<(string, float[])> graphValues = new List<(string, float[])>();
            foreach (var dictEntry in data)
            {
                graphValues.Add((dictEntry.Key, dictEntry.Value));
            }
            return graphValues;
        }


    }
}
