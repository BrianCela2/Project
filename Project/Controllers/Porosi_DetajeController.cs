﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.Models;

namespace Project.Controllers
{
    public class Porosi_DetajeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Porosi_DetajeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public  ActionResult Details(int? id)
        {
            if (id == null || _context.Porosit == null)
            {
                return NotFound();
            }
            List<Porosi_Detaje> porosi_detaje = _context.Porosi_Detajet.Where(p => p.PorosiId == id).ToList();
            return View(porosi_detaje);

        }
    }
}
