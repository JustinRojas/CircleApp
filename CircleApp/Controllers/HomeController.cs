
using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace CircleApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _appDbContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var allPosts = await _appDbContext.Posts
                .Include(u => u.User)
                .OrderByDescending( d => d.DateCreated)
                .ToListAsync();

            return View(allPosts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            //Get the logged user
            int idUserLogged = 1;

            //Create a post
            Post newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                UserId = idUserLogged,
                ImageUrl = "",
                NrOfReports = 0,
            };

            //Check and save the image
            // Verifica que se haya enviado una imagen y que el archivo no esté vacío
            if (post.Image != null && post.Image.Length > 0)
            {
                // Obtiene la ruta física del directorio 'wwwroot' en el servidor
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Valida que el archivo subido sea realmente una imagen según su ContentType
                if (post.Image.ContentType.Contains("image"))
                {
                    // Define la ruta absoluta hacia la carpeta de imágenes (wwwroot/images)
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images");

                    // Crea el directorio 'images' en el disco en caso de que aún no exista
                    Directory.CreateDirectory(rootFolderPathImages);

                    // Genera un nombre único usando GUID para evitar sobrescribir archivos y conserva la extensión original
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);

                    // Combina la ruta de la carpeta de imágenes con el nombre único asignado
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    // Abre un flujo de datos (stream) para crear el archivo físico en el disco
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // Copia el contenido de la imagen subida hacia el flujo de archivo de forma asíncrona
                        await post.Image.CopyToAsync(stream);
                    }

                    // Guarda la URL relativa del archivo guardado en el objeto 'newPost'
                    newPost.ImageUrl = "/images/" + fileName;
                }
            }

            //Add the post to DB
            await _appDbContext.Posts.AddAsync(newPost);
            await _appDbContext.SaveChangesAsync();

            //Rediret to Index
            return RedirectToAction("Index");
        }



    }
}
