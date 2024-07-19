using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RussiaLossesApp.Data;
using RussiaLossesApp.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;



namespace RussiaLossesApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static readonly HttpClient client = new HttpClient();
        private readonly LossObjectContext _lossContext;

        public HomeController(ILogger<HomeController> logger, LossObjectContext lossContext)
        {
            _logger = logger;
            _lossContext = lossContext;
            //loadLossesUntil(DateTime.Today);

        }


        public async void testHttpClient()
        {
            String responseString = await client.GetStringAsync("https://ukr.warspotting.net/api/losses/russia/");
            Console.WriteLine($"Response: {responseString}");
            
            
            var values = JsonConvert.DeserializeObject<Dictionary<string, List<LossObject>>>(responseString);
            foreach(KeyValuePair<string, List<LossObject>> entry in values)
            {
                foreach(LossObject lossObject in entry.Value)
                {
                    Console.WriteLine(lossObject.ToString());
                }
               
            }
            //List<LossList> losses = JsonSerializer.Deserialize<List<LossList>>(responseString);
            /*foreach (LossObject loss in losses[0].losses)
            {
                Console.WriteLine(loss.ToString()); 
            }*/
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
