using Microsoft.AspNetCore.Mvc;
using LocalMusicStreamer.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalMusicStreamer.Controllers
{
    public class PlaylistsController : Controller
    {
        /*
         * DbContext
         */
        private readonly LocalMusicStreamerContext _context;

        public PlaylistsController(LocalMusicStreamerContext context)
        {
            _context = context;
        }
        
        /*
         * GET /Playlists
         * Grabs playlist data from DB, sends to View to display
         */
        public async Task<IActionResult> Index()
        {
            var playlists = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .ToListAsync();

            return View(playlists);
        }
        
        /*
         * POST /Playlists/Create
         * Creates a Playlist
         */
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            var playlist = new PlaylistModel
            {
                Name = name
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); // Updates View right away
        }
        
        /*
         * DELETE /Playlists/Delete/{ID}
         * Deletes playlist from DB
         */
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return Json(new { success = false, message = "Playlist not found" });
            }

            _context.Playlists.Remove(playlist);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Playlist deleted successfully" });
        }
    }
}
