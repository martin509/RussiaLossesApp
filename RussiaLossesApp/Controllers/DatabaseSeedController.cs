using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RussiaLossesApp.Data;
using RussiaLossesApp.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RussiaLossesApp.Controllers
{
    public class DatabaseSeedController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static readonly HttpClient client = new HttpClient();
        private readonly LossObjectContext _lossContext;

        public DatabaseSeedController(ILogger<HomeController> logger, LossObjectContext lossContext)
        {
            _logger = logger;
            _lossContext = lossContext;
            //loadLossesUntil(DateTime.Today);

        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task SeedRange(DateTime start, DateTime end)
        {
            List<LossObject> entries;
            while (start.CompareTo(end) <= 0)
            {
                Debug.WriteLine($"Adding losses for {start:yyyy-MM-dd}..");
                var query = $"SELECT * FROM LOSSOBJECT WHERE DATE = '{start:yyyy-MM-dd}'";
                var fquery = FormattableStringFactory.Create(query);
                var result = await _lossContext.LossObject.FromSql(fquery).ToListAsync();
                if (result.Count == 0)
                {
                    entries = await getLossesDate(start);
                    foreach (LossObject lo in entries)
                    {
                        Debug.WriteLine($"Adding loss: {lo.Id}");
                        var findLoss = await _lossContext.LossObject.FindAsync(lo.Id);
                        Debug.WriteLine($"Equipment type: {lo.EquipType.ToString()}");
                        if(findLoss == null)
                            _lossContext.LossObject.Add(lo);
                        //_lossContext.EquipType.Add(lo.EquipType);

                    }
                }
                else
                {
                    Debug.WriteLine($"Skipping date: {start:yyyy-MM-dd}...");
                }
                
                start = start.AddDays(1);
            }
            Debug.WriteLine("Done filling out losses!");
            await _lossContext.SaveChangesAsync();
            Debug.WriteLine("Done saving losses!");
        }

        [HttpPost("seedday")]
        public async Task<IActionResult> LoadLossesDay(DateTime date)
        {

            await SeedRange(date, date);

            var query = $"SELECT * FROM LOSSOBJECT WHERE " +
               $"DATE ='{date:yyyy-MM-dd}'";
            var fquery = FormattableStringFactory.Create(query);

            var entries = await _lossContext.LossObject.FromSql(fquery).ToListAsync();

            return View("losstable", entries);
        }

        [HttpPost("seedmonth")]
        public async Task<IActionResult> loadLossesMonth(string month)
        {
            DateTime d = DateTime.ParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture);
            DateTime theMonth = new DateTime(d.Year, d.Month, d.Day);
            await SeedRange(d, theMonth);
            var query = $"SELECT * FROM LOSSOBJECT WHERE " +
                $"MONTH(DATE)={d.Month} " +
                $"AND YEAR(DATE)={d.Year}";
            var fquery = FormattableStringFactory.Create(query);

            var entries = await _lossContext.LossObject.FromSql(fquery).ToListAsync();
            //_lossContext.Database.ExecuteSql(FormattableStringFactory.Create("SET IDENTITY_INSERT dbo.LossObject OFF"));
            return View("losstable", entries);
        }

        [HttpPost("seeduntil")]
        public async Task<IActionResult> loadLossesUntil(DateTime date)
        {
            DateTime start = new DateTime(2022, 2, 24);
            await SeedRange(start, date);

            var query = $"SELECT * FROM LOSSOBJECT WHERE " +
                $"DATE >= '{start:yyyy-MM-dd}' AND " +
                $"DATE <= '{date:yyyy-MM-dd}'";
            var fquery = FormattableStringFactory.Create(query);
            var entries = await _lossContext.LossObject.FromSql(fquery).ToListAsync();

            return View("losstable", entries);
        }

        public async Task<List<LossObject>> getLossesDate(DateTime date)
        {
            string responseString = await client.GetStringAsync($"https://ukr.warspotting.net/api/losses/russia/{date:yyyy-MM-dd}");
            Debug.WriteLine($"Getting losses from https://ukr.warspotting.net/api/losses/russia/{date:yyyy-MM-dd}");
            List<JsonLoss> entries = JsonConvert.DeserializeObject<Dictionary<string, List<JsonLoss>>>(responseString).First().Value;
            List<LossObject> losses = new List<LossObject>();
            foreach (var entry in entries)
            {
                var loss = new LossObject();
                loss.copyAttributes(entry);
                Debug.WriteLine($"{entry.model} ({entry.type})");
                var etlist = await _lossContext.EquipType.Where(et => et.name == entry.model && et.category == entry.type).ToListAsync();
                EquipType et;
                if (etlist.Count == 0)
                {
                    Debug.WriteLine("Adding new model!");
                    et = new EquipType { category = entry.type, name = entry.model };
                    _lossContext.EquipType.Add(et);
                    _lossContext.SaveChanges();
                }
                else
                {
                    et = etlist.First();
                }
                loss.EquipType = et;
                losses.Add(loss);
                
            }
            //List<LossObject> entries = JsonConvert.DeserializeObject<Dictionary<string, List<LossObject>>>(responseString).First().Value;
            return losses;
        }
    }
}
