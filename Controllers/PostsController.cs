using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RedeSocial.Models;

namespace RedeSocial.Controllers
{
    public class PostsController : Controller
    {

        private readonly Contexto _context;

        public PostsController(Contexto context)
        {
            _context = context;
        }

        public async Task<IActionResult> HomePost()
        {
            try
            {
                ViewData["Usuario"] = _context.usuario.ToList();
                ViewBag.UserId = HttpContext.Session.GetString("UserId");
                var contexto = _context.post.Include(p => p.usuarioPost);
                return View(await contexto.ToListAsync());

            } catch (Exception ex) { Console.WriteLine("-----------  ERRO:" + ex.Message); return View(); }
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var contexto = _context.post.Include(p => p.usuarioPost);
            return View(await contexto.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.post == null)
            {
                return NotFound();
            }

            var post = await _context.post
                .Include(p => p.usuarioPost)
                .FirstOrDefaultAsync(m => m.postId == id);
            var comentarios = await _context.comentarios
                .Include(c => c.usuarioComentario)
                .Where(m => m.postId == id)
                .ToListAsync();


            if (post == null)
            {
                return NotFound();
            }

            int userId = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            ViewBag.UserId = userId;

            ViewData["Comentarios"] = comentarios;
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarComentario(int postId, string comentario)
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            // Verifique se o post existe
            var post = await _context.post.FindAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            // Crie uma nova instância de Comentarios
            var novoComentario = new Comentarios
            {
                usuarioId = int.Parse(ViewBag.UserId),
                postId = postId,
                comentario = comentario,
                data = DateTime.Now.ToString()
            };

            // Adicione o novo comentário ao contexto
            _context.comentarios.Add(novoComentario);

            // Salve as mudanças no banco de dados
            await _context.SaveChangesAsync();

            // Redirecione de volta para a página de detalhes do post
            return RedirectToAction("Details", "Posts", new { id = postId });
        }


        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewBag.UserId = HttpContext.Session.GetString("UserId");
            var post = new Post
            {
                usuarioPost = _context.usuario.Find(int.Parse(ViewBag.UserId)),
                usuarioId = int.Parse(ViewBag.UserId),
                postDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                postStatus = "0"
            };
            ViewData["usuarioId"] = new SelectList(_context.usuario, "usuarioId", "usuarioId");
            return View(post);
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("postId,postTitulo,postDesc,postCor,postStatus")] Post post, IFormFile? postArquivo)
        {
            post.usuarioId = int.Parse(HttpContext.Session.GetString("UserId")!);
            if (ModelState.IsValid)
            {
                if (postArquivo != null)
                {
                    string extensaoArquivo = Path.GetExtension(postArquivo.FileName).ToLower();
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    var uniqueFileName = "Post" + post.postId.ToString() + "_" + DateTime.Now.ToString().Replace('/', '-').Replace(':', '.').Replace(' ', 't') + "IMG" + extensaoArquivo;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    Directory.CreateDirectory(uploadsFolder);
                    try
                    {
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await postArquivo.CopyToAsync(fileStream);
                        }
                        post.postArquivo = "/uploads/" + uniqueFileName;
                    } catch (Exception ex) { Console.WriteLine("----------- ERRO: " + ex.Message); }
                }
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(HomePost));
            }
            return View(post);
        }


        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.post == null)
            {
                return NotFound();
            }

            var post = await _context.post.FirstOrDefaultAsync(m => m.postId == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["usuarioId"] = new SelectList(_context.usuario, "usuarioId", "usuarioId", post.usuarioId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("postId,usuarioId,postTitulo,postDesc,postArquivo,postCor,postDate,postStatus")] Post post)
        {
            if (id != post.postId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.postId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(HomePost));
            }
            ViewData["usuarioId"] = new SelectList(_context.usuario, "usuarioId", "usuarioId", post.usuarioId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.post == null)
            {
                return NotFound();
            }

            var post = await _context.post
                .Include(p => p.usuarioPost)
                .FirstOrDefaultAsync(m => m.postId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.post == null)
            {
                return Problem("Entity set 'Contexto.post'  is null.");
            }
            var post = await _context.post.FindAsync(id);
            var comentarios = await _context.comentarios.ToListAsync();
            List<Comentarios> commentsToDelete = new List<Comentarios>();
            if (post != null)
            {
                // Busque todos os posts do usuário
                foreach (Comentarios comment in comentarios)
                {
                    if (comment.postId == post.postId)
                    {
                        commentsToDelete.Add(comment);
                    }
                }
                // Exclua os posts e os comentários deste
                _context.comentarios.RemoveRange(commentsToDelete);
                await _context.SaveChangesAsync();
                _context.post.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(HomePost));
        }

        private bool PostExists(int id)
        {
            return (_context.post?.Any(e => e.postId == id)).GetValueOrDefault();
        }
    }
}
