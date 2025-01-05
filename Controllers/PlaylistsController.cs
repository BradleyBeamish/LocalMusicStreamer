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
        
        /*
         * GET /Playlist
         * Grabs ID and Name of each playlist, returns in JSON
         */
        [HttpGet("/Playlist")]
        public IActionResult GetPlaylists()
        {
            var playlists = _context.Playlists.Select(p => new { p.Id, p.Name }).ToList();
            return Ok(playlists);
        }

        /*
         * POST /Playlist/{id}/AddSong
         * Adds a new song to a playlist
         */
        [HttpPost("/Playlist/{id}/AddSong")]
        public IActionResult AddSongToPlaylist(int id, [FromBody] string songName)
        {
            var playlist = _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefault(p => p.Id == id);

            // Playlist doesn't exist
            if (playlist == null)
            {
                return NotFound(new { message = "Playlist not found" });
            }

            // Song doesn't exist
            var song = _context.Songs.FirstOrDefault(s => s.Name == songName);
            if (song == null)
            {
                return NotFound(new { message = "Song not found" });
            }

            // Duplicate song
            if (playlist.PlaylistSongs.Any(ps => ps.SongId == song.Id))
            {
                return BadRequest(new { message = "Song already exists in the playlist" });
            }

            // Good to go
            playlist.PlaylistSongs.Add(new PlaylistSongModel
            {
                PlaylistId = playlist.Id,
                SongId = song.Id
            });

            _context.SaveChanges();
            return Ok(new { message = "Song added to playlist" });
        }
        
        /*
         * GET /Playlists/{id}/Songs
         * Returns the list of songs in a playlist (for Homepage)
         */
        [HttpGet("/Playlists/{id}/Songs")]
        public async Task<IActionResult> GetPlaylistSongs(int id)
        {
            var playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (playlist == null)
            {
                return NotFound(new { message = "Playlist not found" });
            }

            var songs = playlist.PlaylistSongs.Select(ps => new 
            {
                ps.Song.Name,
                FilePath = $"/uploads/{Uri.EscapeDataString(Path.GetFileName(ps.Song.FilePath))}" // Only include the filename (files are currently stored with full drive path)
            }).ToList();

            return Ok(songs);
        }
    }
}
