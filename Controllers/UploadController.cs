using LocalMusicStreamer.Models;
using Microsoft.AspNetCore.Mvc;

public class UploadController : Controller
{
    /*
     * DbContext
     */
    private readonly LocalMusicStreamerContext _context;

    public UploadController(LocalMusicStreamerContext context)
    {
        _context = context;
    }
    
    /*
     * GET /Upload
     * Main View
     */
    public IActionResult Index()
    {
        return View();
    }
    
    /*
     * POST /Upload/UploadFile
     * Saves user uploaded files into /wwwroot/uploads
     */
    [HttpPost]
    public IActionResult UploadFile(IFormFile file)
    {
        if (file.Length > 0)
        {
            // Store Locally
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            var song = new SongModel
            {
                Name = file.FileName,
                FilePath = filePath
            };

            // Add to DB
            _context.Songs.Add(song);
            _context.SaveChanges();
        }

        return View("Index");
    }
}
