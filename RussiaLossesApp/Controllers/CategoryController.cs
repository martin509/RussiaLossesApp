using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;
using RussiaLossesApp.Data;
using RussiaLossesApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RussiaLossesApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly LossObjectContext _losscontext;

        [TempData]
        public int CurCategory { get; set; }

        public CategoryController(LossObjectContext context)
        {
            _losscontext = context;
        }


        // GET: Category
        public async Task<IActionResult> Index()
        {
            List<EquipCategory> categoryNames = await _losscontext.EquipCategory.ToListAsync();
            //Debug.WriteLine("Entered Index controller!");
            return View(categoryNames);
        }

        [HttpGet("Category/GetCategory")]
        public async Task<ActionResult> GetCategory(int catId)
        {
            Debug.WriteLine($"GetCategory has been called! Id: {catId}");
            EquipCategory? category = await _losscontext.EquipCategory.Include(cat => cat.EquipTypes).Where(cat => cat.Id == catId).FirstOrDefaultAsync();
            
            //List<EquipType> typeList = new List<EquipType>();
            if(category != null)
            {
                Debug.WriteLine($"EquipTypes: (list of length {category.EquipTypes.Count})");
                foreach (EquipType et in category.EquipTypes)
                {
                    Debug.WriteLine($"{et.Id} ({et.name})");
                }
                return Json(category);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("Category/GetAllCategories")]
        public async Task<ActionResult> GetAllCategories()
        {
            Debug.WriteLine("GetAllCategories called.");
            Stopwatch sw = Stopwatch.StartNew();
            List<EquipCategory>? categories = await _losscontext.EquipCategory.ToListAsync();
            if (categories != null)
            {
                sw.Stop();
                Debug.WriteLine($"Time taken for GetAllCategories call: {sw.Elapsed}");
                return Json(categories);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("Category/SelectCategory")]
        public async Task<IActionResult> SelectCategory(int catId)
        {
            Debug.WriteLine($"catId: {catId}");
            EquipCategory? cat = await _losscontext.EquipCategory.Include(cat => cat.EquipTypes).Where(cat => cat.Id == catId).FirstOrDefaultAsync();
            int id = 0;
            if (cat != null) { 
                id = cat.Id;
                //CurCategory = cat.Name;
                TempData["CurCategory"] = cat.Id;
                return View($"add", await getEquipTypes(cat));
            }
            else
            {
                return RedirectToAction("Index");
            }

            //the "add" view requires basically a dictionary of all equipment types
            
            
        }


        [HttpPost("Category/AddToCategory")]
        public async Task<IActionResult> AddToCategory(int equipId, int catId)
        {
            Debug.WriteLine($"Adding {equipId} to category {catId}...");
            EquipCategory? category = await _losscontext.EquipCategory.Where(cat => cat.Id == catId).Include(cat => cat.EquipTypes).FirstOrDefaultAsync();
            EquipType? equipType = await _losscontext.EquipType.Where(et => et.Id == equipId).FirstOrDefaultAsync();

            if (category != null && equipType != null && !category.EquipTypes.Exists(et => et.Id == equipType.Id))
            {
                Debug.WriteLine($"Found {equipId}: {equipType.name}!");
                category.EquipTypes.Add(equipType);
                await _losscontext.SaveChangesAsync();
                TempData.Keep("CurCategory");
                return await SelectCategory(category.Id);
                //return View("add", await getEquipTypes(category));
            }
            else
            {
                return RedirectToAction("/Index");
            }
            
        }

        [HttpPost("Category/CreateCategory")]
        public async Task<IActionResult> CreateCategory(string categoryClass, string name)
        {
            //Debug.WriteLine("Entered CreateCategory controller");
            EquipCategory newcat = new EquipCategory();
            newcat.Name = name;
            newcat.categoryClass = categoryClass;
            EquipCategory? oldcat = _losscontext.EquipCategory.Find(newcat.Id);
            if(oldcat == null)
            {
                await _losscontext.AddAsync(newcat);
                await _losscontext.SaveChangesAsync();
            }
            
            //var categories = await getCategories();
            return RedirectToAction("Index");
            //return View("create", await getCategories());
        }

        private async Task<List<string>> GetCategories()
        {
            List<string> categoryNames = await _losscontext.EquipCategory.Select(cat => cat.Name).ToListAsync();
            return categoryNames;
        }

        private async Task<List<EquipType>> getEquipTypes(EquipCategory cat)
        {
            var list = await _losscontext.EquipType.Where(et => et.category == cat.categoryClass).ToListAsync();
            //then a bunch of fetching all equipment types
            /*Dictionary<string, List<EquipType>> equipDict = new Dictionary<string, List<EquipType>>();
            var list = await (_losscontext.EquipType.ToDictionaryAsync(et => et.name));
            foreach (string key in list.Keys)
            {
                if (!equipDict.ContainsKey(list[key].category))
                {
                    equipDict.Add(list[key].category, new List<EquipType>());
                }
                equipDict[list[key].category].Add(list[key]);
            }
            List<(string, EquipType[])> equipTypes = new List<(string, EquipType[])>();
            foreach (var dictEntry in equipDict)
            {
                equipTypes.Add((dictEntry.Key, dictEntry.Value.ToArray()));
            }*/
            return list;
        }
    }
}
