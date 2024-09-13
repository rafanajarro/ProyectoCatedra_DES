using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProyectoCatedra_DES.Models;

namespace ProyectoCatedra_DES.Controllers
{
    public class OrdenComprasController : Controller
    {
        private readonly ProyectoDbContext _context;

        public OrdenComprasController(ProyectoDbContext context)
        {
            _context = context;
        }

        // GET: OrdenCompras
        public async Task<IActionResult> Index()
        {
            var proyectoDbContext = _context.OrdenCompras.Include(o => o.Usuario);
            return View(await proyectoDbContext.ToListAsync());
        }

        // GET: OrdenCompras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenCompra = await _context.OrdenCompras
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.OrdenCompraId == id);
            if (ordenCompra == null)
            {
                return NotFound();
            }

            return View(ordenCompra);
        }

        // GET: OrdenCompras/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido");
            return View();
        }

        // POST: OrdenCompras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrdenCompraId,FechaCompra,UsuarioId,Estado,ProductoId,CantidadCompra,TotalOrdenCompra")] OrdenCompra ordenCompra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordenCompra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", ordenCompra.UsuarioId);
            return View(ordenCompra);
        }

        // GET: OrdenCompras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenCompra = await _context.OrdenCompras.FindAsync(id);
            if (ordenCompra == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", ordenCompra.UsuarioId);
            return View(ordenCompra);
        }

        // POST: OrdenCompras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrdenCompraId,FechaCompra,UsuarioId,Estado,ProductoId,CantidadCompra,TotalOrdenCompra")] OrdenCompra ordenCompra)
        {
            if (id != ordenCompra.OrdenCompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordenCompra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdenCompraExists(ordenCompra.OrdenCompraId))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "UsuarioId", "Apellido", ordenCompra.UsuarioId);
            return View(ordenCompra);
        }

        // GET: OrdenCompras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenCompra = await _context.OrdenCompras
                .Include(o => o.Usuario)
                .FirstOrDefaultAsync(m => m.OrdenCompraId == id);
            if (ordenCompra == null)
            {
                return NotFound();
            }

            return View(ordenCompra);
        }

        // POST: OrdenCompras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordenCompra = await _context.OrdenCompras.FindAsync(id);
            if (ordenCompra != null)
            {
                _context.OrdenCompras.Remove(ordenCompra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdenCompraExists(int id)
        {
            return _context.OrdenCompras.Any(e => e.OrdenCompraId == id);
        }
    }
}
