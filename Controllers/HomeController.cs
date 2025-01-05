using LocalMusicStreamer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocalMusicStreamer.Controllers;

public class HomeController : Controller
{
    /*
     * DbContext
     */
    private readonly LocalMusicStreamerContext _context;

    public HomeController(LocalMusicStreamerContext context)
    {
        _context = context;
    }
        
    /*
     * GET /Home
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
}
