using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class ProduktController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public ProduktController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
       
        public async Task<IActionResult> Index(string emri = "", string StringKerkimi="",string Sorting="")
        {

            var produkte = from m in _context.Produkte
                           select m;
            if (emri == "" && StringKerkimi=="")
            {
                if (Sorting == "Cmimi")
                {
                    produkte = produkte.OrderByDescending(stu => stu.Cmimi);
                }else
                        produkte = produkte.OrderBy(stu => stu.Cmimi);

                return View(await produkte.ToListAsync());
            }
            if (!String.IsNullOrEmpty(StringKerkimi))
            {
                produkte = produkte.Where(s => s.Emri!.Contains(StringKerkimi) || s.Pershkrimi!.Contains(StringKerkimi));
                return View(await produkte.OrderBy(p=>p.Cmimi).ToListAsync());
            }
            else
            {
                Kategori kategori = await _context.Kategorit.Where(c => c.Emri == emri).FirstOrDefaultAsync();
                if (kategori == null) return RedirectToAction("Index");
                var produkteKategori = _context.Produkte.Where(p => p.KategoriID == kategori.Id);
                return View(await produkteKategori.OrderBy(p=>p.Cmimi).ToListAsync());
            }
        }
        public async Task<IActionResult> FurnizuesProdukte(string emri="")
        {
           
                Furnizues furnizues = await _context.Furnizuesit.Where(c => c.Emri == emri).FirstOrDefaultAsync();
                if (furnizues == null) return RedirectToAction("Index");

                var produktefurnizues = _context.Produkte.Where(p => p.FurnizuesID == furnizues.Id);
                return View(await produktefurnizues.OrderByDescending(p => p.Id).ToListAsync());
            
        }
        public async Task<IActionResult> Oferte()
        {
            var ProdukteOferte = _context.Produkte.Where(p => p.Oferte == true);
            return View(await ProdukteOferte.OrderByDescending(p => p.Id).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Produkte == null)
            {
                return NotFound();
            }

            var produkt = await _context.Produkte
                .Include(p => p.Furnizues)
                .Include(p => p.Kategori)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produkt == null)
            {
                return NotFound();
            }

            return View(produkt);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["FurnizuesID"] = new SelectList(_context.Furnizuesit, "Id", "Emri");
            ViewData["KategoriID"] = new SelectList(_context.Kategorit, "Id", "Emri");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Emri,Cmimi,Sasia,Pershkrimi,Foto,KategoriID,FurnizuesID,ImageFile")] Produkt produkt,IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/",
                ImageFile.FileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);

                    stream.Close();

                }
                produkt.Foto = ImageFile.FileName;
                _context.Produkte.Add(produkt);
                _context.SaveChanges();
                string message = "Created the record successfully";
                ViewBag.Message = message;
                return RedirectToAction(nameof(Index));
            }
            return View(produkt);
        }

        [Authorize(Roles="Admin")]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {

                return NotFound();
            }
            var prfromdb = _context.Produkte.Find(id);

            if (prfromdb == null)
            {
                return NotFound();
            }
            ViewData["FurnizuesID"] = new SelectList(_context.Furnizuesit, "Id", "Emri", prfromdb.FurnizuesID);
            ViewData["KategoriID"] = new SelectList(_context.Kategorit, "Id", "Emri", prfromdb.KategoriID);
            return View(prfromdb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Produkt produkt,IFormFile ImageFile)
        {
            ViewData["FurnizuesID"] = new SelectList(_context.Furnizuesit, "Id", "Emri", produkt.FurnizuesID);
            ViewData["KategoriID"] = new SelectList(_context.Kategorit, "Id", "Emri", produkt.KategoriID);
            if (ModelState.IsValid)
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/",
                 ImageFile.FileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);

                    stream.Close();

                }
                produkt.Foto = ImageFile.FileName;
                _context.Produkte.Update(produkt);
                _context.SaveChanges();
                TempData["Success"] = "Te dhenat u edituan !";
                return RedirectToAction("Index");
            }
            return View(produkt);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Produkte == null){
                return NotFound();
            }
            var produkt = await _context.Produkte
                .Include(p => p.Furnizues)
                .Include(p => p.Kategori)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (produkt == null)
            {
                return NotFound();
            }

            return View(produkt);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Produkte == null)
            {
                return Problem("Nuk ka produkt");
            }
            var produkt = await _context.Produkte.FindAsync(id);
            if (produkt != null)
            {
                _context.Produkte.Remove(produkt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProduktExists(int id)
        {
            return _context.Produkte.Any(e => e.Id == id);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ProduktOferte(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Produkt produkt = _context.Produkte.Where(x => x.Id == id).FirstOrDefault();

            if (produkt != null)
            {
               int? SasiaTani = produkt.Sasia;
               
                if (SasiaTani > 20 && produkt.Oferte != true)
                {
                    produkt.Cmimi = 0.9 * produkt.Cmimi;
                    produkt.Oferte = true;
                    _context.SaveChanges();
                }  
            }
            return RedirectToAction(nameof(Index));
             
            }
        }
    }
           
        


    
