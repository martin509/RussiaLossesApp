using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RussiaLossesApp.Data;
using RussiaLossesApp.Models;

namespace RussiaLossesApp.Controllers
{
    public class LossObjectsController : Controller
    {
        private readonly LossObjectContext _context;

        public LossObjectsController(LossObjectContext context)
        {
            _context = context;
        }

        // GET: LossObjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.LossObject.ToListAsync());
        }

        // GET: LossObjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lossObject = await _context.LossObject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lossObject == null)
            {
                return NotFound();
            }

            return View(lossObject);
        }

        // GET: LossObjects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LossObjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,type,model,date,status,lost_by,nearest_location,geo,unit,tags")] LossObject lossObject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lossObject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lossObject);
        }

        // GET: LossObjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lossObject = await _context.LossObject.FindAsync(id);
            if (lossObject == null)
            {
                return NotFound();
            }
            return View(lossObject);
        }

        // POST: LossObjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,type,model,date,status,lost_by,nearest_location,geo,unit,tags")] LossObject lossObject)
        {
            if (id != lossObject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lossObject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LossObjectExists(lossObject.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(lossObject);
        }

        // GET: LossObjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lossObject = await _context.LossObject
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lossObject == null)
            {
                return NotFound();
            }

            return View(lossObject);
        }

        // POST: LossObjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lossObject = await _context.LossObject.FindAsync(id);
            if (lossObject != null)
            {
                _context.LossObject.Remove(lossObject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LossObjectExists(int id)
        {
            return _context.LossObject.Any(e => e.Id == id);
        }
    }
}
