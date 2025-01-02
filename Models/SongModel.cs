namespace LocalMusicStreamer.Models;

public class SongModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }

    public List<PlaylistSongModel> PlaylistSongs { get; set; } = new List<PlaylistSongModel>();
}
