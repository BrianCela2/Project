using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class FurnizuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FurnizuesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
              return View(await _context.Furnizuesit.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Furnizuesit == null)
            {
                return NotFound();
            }
            var furnizues = await _context.Furnizuesit
                .FirstOrDefaultAsync(m => m.Id == id);
            if (furnizues == null)
            {
                return NotFound();
            }

            return View(furnizues);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Emri,Shteti")] Furnizues furnizues)
        {
            if (ModelState.IsValid)
            {
                _context.Add(furnizues);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(furnizues);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Furnizuesit == null)
            {
                return NotFound();
            }

            var furnizues = await _context.Furnizuesit.FindAsync(id);
            if (furnizues == null)
            {
                return NotFound();
            }
            return View(furnizues);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Emri,Shteti")] Furnizues furnizues)
        {
            if (id != furnizues.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                    _context.Update(furnizues);
                    await _context.SaveChangesAsync();
            
                return RedirectToAction(nameof(Index));
            }
            return View(furnizues);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Furnizuesit == null)
            {
                return NotFound();
            }

            var furnizues = await _context.Furnizuesit
                .FirstOrDefaultAsync(m => m.Id == id);
            if (furnizues == null)
            {
                return NotFound();
            }

            return View(furnizues);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Furnizuesit == null)
            {
                return Problem("Tabela Furnizuesit eshte bosh");
            }
            var furnizues = await _context.Furnizuesit.FindAsync(id);
            if (furnizues != null)
            {
                _context.Furnizuesit.Remove(furnizues);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FurnizuesExists(int id)
        {
          return _context.Furnizuesit.Any(e => e.Id == id);
        }
    }
}
