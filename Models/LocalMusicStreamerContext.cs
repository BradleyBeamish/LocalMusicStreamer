namespace LocalMusicStreamer.Models;
using Microsoft.EntityFrameworkCore;

public class LocalMusicStreamerContext : DbContext
{
    public LocalMusicStreamerContext(DbContextOptions<LocalMusicStreamerContext> options) : base(options) { }

    public DbSet<PlaylistModel> Playlists { get; set; }
    public DbSet<SongModel> Songs { get; set; }
    public DbSet<PlaylistSongModel> PlaylistSongs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Many-to-many relationship
        modelBuilder.Entity<PlaylistSongModel>()
            .HasKey(ps => new { ps.PlaylistId, ps.SongId });

        modelBuilder.Entity<PlaylistSongModel>()
            .HasOne(ps => ps.Playlist)
            .WithMany(p => p.PlaylistSongs)
            .HasForeignKey(ps => ps.PlaylistId);

        modelBuilder.Entity<PlaylistSongModel>()
            .HasOne(ps => ps.Song)
            .WithMany(s => s.PlaylistSongs)
            .HasForeignKey(ps => ps.SongId);
    }
}
