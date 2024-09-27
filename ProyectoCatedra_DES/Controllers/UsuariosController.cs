using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ProyectoCatedra_DES.Models;

namespace ProyectoCatedra_DES.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ProyectoDbContext _context;
        private readonly EmailService _emailService;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuariosController(ProyectoDbContext context, EmailService emailService)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
            _emailService = emailService;
        }

        // Deshabilitar Usuario
        [HttpPost, ActionName("Deshabilitar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deshabilitar(int usuarioId)
        {
            if (usuarioId == null)
            {
                return NotFound();
            }

            var dbUsuario = await _context.Usuarios
               .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId);

            if (dbUsuario == null)
            {
                return NotFound();
            }

            dbUsuario.Activo = false;
            _context.Entry(dbUsuario).Property(u => u.Activo).IsModified = true;
            await _context.SaveChangesAsync();

            TempData["Message"] = "El usuario " + dbUsuario.NombreUsuario + " ha sido deshabilitado correctamente.";
            return RedirectToAction("Dashboard", "Usuarios");
        }

        // Ver Perfil Usuario
        public async Task<IActionResult> VerPerfil()
        {
            var nombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            var dbUsername = _context.Usuarios.FirstOrDefault(x => x.NombreUsuario == nombreUsuario);
            return View("Perfil", dbUsername);
        }

        // Cerrar sesion Usuario
        public ActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Index", "Usuarios");
        }

        // DASHBOARD Usuario
        public async Task<IActionResult> Dashboard()
        {
            var nombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            var dbUsuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);


            if (dbUsuario != null)
            {
                if (dbUsuario.Rol == "Admin")
                {
                    var usuarios = await _context.Usuarios.ToListAsync();
                    return View("Dashboard", usuarios);
                }
            }

            return View("Dashboard");
        }

        // LOGIN Usuario
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (loginDto != null)
            {
                var revisarUsuarioDb = await _context.Usuarios.FirstOrDefaultAsync(m => m.NombreUsuario == loginDto.NombreUsuario);

                if (revisarUsuarioDb != null)
                {
                    if (revisarUsuarioDb.Activo == true)
                    {
                        var resultado = _passwordHasher.VerifyHashedPassword(revisarUsuarioDb, revisarUsuarioDb.Password, loginDto.Password);
                        if (resultado == PasswordVerificationResult.Success)
                        {
                            HttpContext.Session.SetString("NombreUsuario", loginDto.NombreUsuario);
                            HttpContext.Session.SetString("RolUsuario", revisarUsuarioDb.Rol);
                            HttpContext.Session.SetString("Nombre", revisarUsuarioDb.Nombre);
                            HttpContext.Session.SetString("Apellidos", revisarUsuarioDb.Apellidos);

                            // Actualizar ultimo acceso
                            revisarUsuarioDb.UltimoAcceso = DateTime.Now;
                            _context.Entry(revisarUsuarioDb).Property(u => u.UltimoAcceso).IsModified = true;
                            await _context.SaveChangesAsync();

                            return RedirectToAction("Dashboard", "Usuarios");
                        }
                        else
                        {
                            TempData["Error"] = "Error. Credenciales incorrectas.";
                            return RedirectToAction("Index", "Usuarios");
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Error. Este usuario esta deshabilitado.";
                        return RedirectToAction("Index", "Usuarios");
                    }
                }
                else
                {
                    TempData["Message"] = "No se encontro ningún usuario con esas credenciales.";
                    return RedirectToAction("Index", "Usuarios");
                }
            }
            else
            {
                TempData["Error"] = "Error. Completa todos los datos solicitados.";
                return RedirectToAction("Index", "Usuarios");
            }
        }

        public async Task<IActionResult> RegisterView()
        {
            return View("Register");
        }

        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Codigo de username
            string codigoUsername = GenerarUsername(registerDto.Apellidos);
            var dbUsername = _context.Usuarios.FirstOrDefault(x => x.NombreUsuario == codigoUsername);

            while (dbUsername != null)
            {
                codigoUsername = GenerarUsername(registerDto.Apellidos);
            }

            //Verificar no exista un usuario con otro correo
            var dbUserCorreo = _context.Usuarios.FirstOrDefault(x => x.CorreoElectronico == registerDto.CorreoElectronico);

            if (dbUserCorreo == null)
            {
                bool correoEnviado = await _emailService.EnviarCorreoAsync(registerDto.CorreoElectronico, codigoUsername);
                if (correoEnviado)
                {
                    _context.Usuarios.Add(new Usuario
                    {
                        NombreUsuario = codigoUsername,
                        Password = _passwordHasher.HashPassword(null, registerDto.Password),
                        CorreoElectronico = registerDto.CorreoElectronico,
                        Telefono = registerDto.Telefono,
                        Nombre = registerDto.Nombre,
                        Apellidos = registerDto.Apellidos,
                        Dui = registerDto.Dui,
                        Activo = true,
                        Rol = registerDto.Rol,
                        FechaNacimiento = registerDto.FechaNacimiento,
                        FechaCreacion = DateTime.Now,
                        UltimoAcceso = DateTime.Now,
                    });
                    _context.SaveChanges();
                    TempData["Message"] = "Usuario creado con exito.";
                    return RedirectToAction("Index", "Usuarios");
                }
                else
                {
                    TempData["Message"] = "Se creo el usuario pero no se pudo enviar el correo con el nombre de usuario.";
                    return RedirectToAction("Index", "Usuarios");
                }
            }
            else
            {
                TempData["Error"] = "Error. Ya existe un usuario con este correo electrónico.";
                return RedirectToAction("Register", "Usuarios");
            }
        }

        // Generar codigo de usuario
        public static string GenerarUsername(string ApellidosUsuario)
        {
            //Obtener iniciales apellidos            
            string iniciales = "";
            var palabras = ApellidosUsuario.Split(' ');
            if (palabras.Length >= 2)
                iniciales = palabras[0][0].ToString().ToUpper() + palabras[1][0].ToString().ToUpper();
            else
                iniciales = palabras[0][0].ToString().ToUpper() + palabras[0][0].ToString().ToUpper();

            string digitos = "0123456789";
            Random random = new Random();
            char[] resultado = new char[8];

            // Generar 8 caracteres aleatorios
            for (int i = 0; i < resultado.Length; i++)
            {
                resultado[i] = digitos[random.Next(digitos.Length)];
            }

            string stringAleatorio = new string(resultado);
            string codigoUsername = iniciales + stringAleatorio;
            return codigoUsername;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Codigo de username
            string codigoUsername = GenerarUsername(registerDto.Apellidos);
            var dbUsername = _context.Usuarios.FirstOrDefault(x => x.NombreUsuario == codigoUsername);

            while (dbUsername != null)
            {
                codigoUsername = GenerarUsername(registerDto.Apellidos);
            }

            //Verificar no exista un usuario con otro correo
            var dbUserCorreo = _context.Usuarios.FirstOrDefault(x => x.CorreoElectronico == registerDto.CorreoElectronico);

            if (dbUserCorreo == null)
            {
                _context.Usuarios.Add(new Usuario
                {
                    NombreUsuario = codigoUsername,
                    Password = _passwordHasher.HashPassword(null, registerDto.Password),
                    CorreoElectronico = registerDto.CorreoElectronico,
                    Telefono = registerDto.Telefono,
                    Nombre = registerDto.Nombre,
                    Apellidos = registerDto.Apellidos,
                    Dui = registerDto.Dui,
                    Activo = true,
                    Rol = registerDto.Rol,
                    FechaNacimiento = registerDto.FechaNacimiento,
                    FechaCreacion = DateTime.Now,
                    UltimoAcceso = DateTime.Now,
                });
                _context.SaveChanges();
                TempData["Message"] = "Usuario creado con exito.";
                return RedirectToAction("Dashboard", "Usuarios");
            }
            else
            {
                TempData["Error"] = "Error. Ya existe un usuario con este correo electrónico.";
                return RedirectToAction("Create", "Usuarios");
            }
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return NotFound();
            }

            var dbUsuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);

            if (dbUsuario == null)
            {
                return NotFound();
            }

            dbUsuario.CorreoElectronico = usuario.CorreoElectronico;
            dbUsuario.Telefono = usuario.Telefono;
            dbUsuario.Nombre = usuario.Nombre;
            dbUsuario.Apellidos = usuario.Apellidos;
            dbUsuario.Dui = usuario.Dui;
            dbUsuario.FechaNacimiento = usuario.FechaNacimiento;
            _context.Entry(dbUsuario).Property(u => u.CorreoElectronico).IsModified = true;
            _context.Entry(dbUsuario).Property(u => u.Telefono).IsModified = true;
            _context.Entry(dbUsuario).Property(u => u.Nombre).IsModified = true;
            _context.Entry(dbUsuario).Property(u => u.Apellidos).IsModified = true;
            _context.Entry(dbUsuario).Property(u => u.Dui).IsModified = true;
            _context.Entry(dbUsuario).Property(u => u.FechaNacimiento).IsModified = true;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Datos del usuario actualizados con exito.";
            return RedirectToAction("VerPerfil", "Usuarios");
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.UsuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
