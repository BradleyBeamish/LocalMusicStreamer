using LocalMusicStreamer.Models;

namespace LocalMusicStreamer.Utilities;

/*
 * Load all audio files from wwwroot/uploads into DB
 * Would allow the user to drag and drop files into this folder manually (without program running)
 */
public class DatabaseSeeder
{
    public static void Seed(LocalMusicStreamerContext context, IWebHostEnvironment env)
    {
        var uploadsPath = Path.Combine(env.WebRootPath, "uploads");

        if (Directory.Exists(uploadsPath))
        {
            var files = Directory.GetFiles(uploadsPath);

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);

                if (!context.Songs.Any(s => s.FilePath == file))
                {
                    context.Songs.Add(new SongModel
                    {
                        Name = fileName,
                        FilePath = $"/uploads/{fileName}"
                    });
                }
            }

            context.SaveChanges();
        }
    }
}
