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
        public string CurCategory { get; set; }

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

        [HttpGet("Category/GetTypesInCategory")]
        public async Task<ActionResult> GetTypesInCategory(int catId)
        {
            EquipCategory? category = await _losscontext.EquipCategory.Where(cat => cat.Id == catId).Include(cat => cat.EquipTypes).FirstOrDefaultAsync();
            List<EquipType> typeList = new List<EquipType>();
            if(category != null)
            {
                typeList = category.EquipTypes;
            }
            
            return Json(typeList);
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
                TempData["CurCategory"] = cat.Name;
            }

            //the "add" view requires basically a dictionary of all equipment types
            
            return View($"add", await getEquipTypes());
        }


        [HttpPost("Category/AddToCategory")]
        public async Task<IActionResult> AddToCategory(string equipName, string catName)
        {
            Debug.WriteLine($"Adding {equipName} to category {catName}...");
            EquipCategory? category = await _losscontext.EquipCategory.Where(cat => cat.Name == catName).Include(cat => cat.EquipTypes).FirstOrDefaultAsync();
            EquipType? equipType = await _losscontext.EquipType.Where(et => et.name == equipName).FirstOrDefaultAsync();

            if (category != null && equipType != null && !category.EquipTypes.Exists(et => et.Id == equipType.Id))
            {
                category.EquipTypes.Add(equipType);
                await _losscontext.SaveChangesAsync();
            }

            TempData.Keep("CurCategory");

            return View("add", await getEquipTypes());
        }

        [HttpPost("Category/CreateCategory")]
        public async Task<IActionResult> CreateCategory(string name)
        {
            //Debug.WriteLine("Entered CreateCategory controller");
            EquipCategory newcat = new EquipCategory();
            newcat.Name = name;
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

        private async Task<List<string>> getCategories()
        {
            List<string> categoryNames = await _losscontext.EquipCategory.Select(cat => cat.Name).ToListAsync();
            return categoryNames;
        }

        private async Task<List<(string, EquipType[])>> getEquipTypes()
        {
            //then a bunch of fetching all equipment types
            Dictionary<string, List<EquipType>> equipDict = new Dictionary<string, List<EquipType>>();
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
            }
            return equipTypes;
        }
    }
}
