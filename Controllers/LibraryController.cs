using Microsoft.AspNetCore.Mvc;
using LocalMusicStreamer.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalMusicStreamer.Controllers
{
    public class LibraryController : Controller
    {
        /*
         * DbContext
         */
        private readonly LocalMusicStreamerContext _context;

        public LibraryController(LocalMusicStreamerContext context)
        {
            _context = context;
        }

        /*
         * GET /Library
         */
        public IActionResult Index()
        {
            return View();
        }

        /*
         * /Library/DeleteFile
         * Searches for the local file, then attempts to delete it
         */
        [HttpPost]
        public IActionResult DeleteFile([FromBody] string filePath)
        {
            var sanitizedPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", Path.GetFileName(filePath));
    
            try
            {
                if (!System.IO.File.Exists(sanitizedPath))
                {
                    return NotFound(new { message = "File does not exist." });
                }

                // Delete the local file
                System.IO.File.Delete(sanitizedPath);

                // Delete database entries
                var song = _context.Songs.FirstOrDefault(s => s.FilePath == filePath);
                
                if (song != null)
                {
                    _context.PlaylistSongs.RemoveRange(_context.PlaylistSongs.Where(ps => ps.SongId == song.Id));
                    _context.Songs.Remove(song);
                    _context.SaveChanges();
                }

                return Ok(new { message = "Song deleted successfully." });
            }
            catch (IOException ex)
            {
                return StatusCode(500, new { message = "Error caught" });
            }
        }
    }
}
