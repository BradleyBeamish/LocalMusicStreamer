using Microsoft.AspNetCore.Mvc;
using LocalMusicStreamer.Models;

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
         * POST /Library/DeleteFile
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
         * Request body for compression
         */
        public class CompressRequest
        {
            public string FileName { get; set; }
            public int Bitrate { get; set; }
            public string OutputFormat { get; set; }
        }
        
        /*
         * POST /Library/Compress
         * Uses FFMpeg to compress a audio file, also creates a copy of the file
         */
        [HttpPost]
        public IActionResult Compress([FromBody] CompressRequest request)
        {
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            var inputFile = Path.Combine(uploadPath, request.FileName);
            var outputFileName = Path.GetFileNameWithoutExtension(request.FileName) + $"_compressed.{request.OutputFormat}";
            var outputFile = Path.Combine(uploadPath, outputFileName);

            if (!System.IO.File.Exists(inputFile))
            {
                return NotFound(new { message = "Input file not found." });
            }

            try
            {
                // Build ffmpeg arguments
                var ffmpegArgs = $"-i \"{inputFile}\" -b:a {request.Bitrate}k \"{outputFile}\"";

                // "FFMpeg" keyword must be accessible through CLI for this to run
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = ffmpegArgs,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    return StatusCode(500, new { message = $"Compression failed: {error}" });
                }
                
                // Add new _compressed file to database
                var song = new SongModel
                {
                    Name = outputFileName,
                    FilePath = outputFile
                };

                _context.Songs.Add(song);
                _context.SaveChanges();

                return Ok(new { message = "Compression complete.", outputFileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error during compression: {ex.Message}" });
            }
        }
    }
}
