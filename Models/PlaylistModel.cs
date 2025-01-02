namespace LocalMusicStreamer.Models;

public class PlaylistModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<PlaylistSongModel> PlaylistSongs { get; set; } = new List<PlaylistSongModel>();
}
