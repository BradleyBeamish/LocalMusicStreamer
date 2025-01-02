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
    }
}
