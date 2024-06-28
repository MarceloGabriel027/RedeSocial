using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using RedeSocial.Models;

namespace RedeSocial.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Contexto _context;

        public UsuarioController(Contexto context)
        {
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Entrar(Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _context.usuario
                        .FirstOrDefault(u => u.usuarioEmail == usuario.usuarioEmail && u.usuarioSenha == usuario.usuarioSenha);

                    if (user != null)
                    {
                        // Definir o cookie (por exemplo, após o login)
                        Response.Cookies.Append("UserId", user.usuarioId.ToString());


                        HttpContext.Session.SetString("UserId", user.usuarioId.ToString());
                        return RedirectToAction("HomePost", "Posts");
                    }
                    else
                    {
                        TempData["Mensagem"] = "Email ou senha inválidos.";
                        return View("Login");
                    }
                }

                return View("Login");
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = "Erro: " + ex.Message;
                return View("Login");
            }
        }

        [HttpPost]
        public ActionResult Registrar(Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(usuario);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
                }

                return View("Cadastro");
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = "Erro: " + ex.Message;
                return View("Cadastro");
            }
        }


        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            return _context.usuario != null ?
                        View(await _context.usuario.ToListAsync()) :
                        Problem("Entity set 'Contexto.usuario'  is null.");
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.usuario == null)
            {
                return NotFound();
            }
            int userID = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            ViewBag.UserId = userID;

            ViewData["Posts"] = _context.post.Where(p => p.usuarioId == id).ToList();
            var usuario = await _context.usuario.FirstOrDefaultAsync(m => m.usuarioId == id);
            List<Bloqueados>? bloqueados = await _context.bloqueados.Where(b => b.idUsuario == id).ToListAsync();
            ViewBag.Bloqueados = bloqueados;
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        // GET: Usuario/Details/5
        public async Task<IActionResult> DetailsUser(int? id)
        {
            if (id == null || _context.usuario == null)
            {
                return NotFound();
            }
            ViewData["Users"] = _context.usuario.ToList();
            ViewData["Posts"] = _context.post.Where(p => p.usuarioId == id).ToList();
            var usuario = await _context.usuario
                .FirstOrDefaultAsync(m => m.usuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }
            int userID = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            ViewBag.UserId = userID;
            List<Bloqueados>? bloqueados = await _context.bloqueados.Where(b => b.idUsuario == id).ToListAsync();
            ViewBag.Bloqueados = bloqueados;

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("usuarioId,usuarioEmail,usuarioSenha,usuarioImagem,usuarioNome,usuarioTelefone,usuarioEndereco,usuarioCPF")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("usuarioId,usuarioEmail,usuarioSenha,usuarioImagem,usuarioNome,usuarioTelefone,usuarioEndereco,usuarioCPF")] Usuario usuario, IFormFile? usuarioImagem)
        {
            if (id != usuario.usuarioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (usuarioImagem != null)
                    {
                        string extensaoArquivo = Path.GetExtension(usuarioImagem.FileName).ToLower();
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        var uniqueFileName = "User" + usuario.usuarioId.ToString() + "_" + DateTime.Now.ToString() + "IMG" + extensaoArquivo;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Certifique-se de que o diretório existe
                        Directory.CreateDirectory(uploadsFolder);

                        // Salvar o arquivo no servidor
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await usuarioImagem.CopyToAsync(fileStream);
                        }

                        // Atualizar o caminho da imagem no objeto usuario
                        usuario.usuarioImagem = "/uploads/" + uniqueFileName;
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.usuarioId))
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
            return View(usuario);
        }


        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuario
                .FirstOrDefaultAsync(m => m.usuarioId == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.usuario == null)
            {
                return Problem("Entity set 'Contexto.usuario'  is null.");
            }
            var usuario = await _context.usuario.FindAsync(id);
            if (usuario != null)
            {
                // Busque todos os posts do usuário
                List<Post> postsToDelete = new List<Post>();
                foreach (var post in postsToDelete)
                {
                    if (post.usuarioId == id)
                    {
                        postsToDelete.Add(post);
                        if (post.postArquivo != null && post.postArquivo != "")
                        {
                            System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", post.postArquivo));
                        }
                    }
                }
                // Exclua os posts
                _context.post.RemoveRange(postsToDelete);

                if (usuario.usuarioImagem != null && usuario.usuarioImagem != null)
                {
                    System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", usuario.usuarioImagem));
                }
                _context.usuario.Remove(usuario);
                _context.SaveChanges();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return (_context.usuario?.Any(e => e.usuarioId == id)).GetValueOrDefault();
        }
    }
}
