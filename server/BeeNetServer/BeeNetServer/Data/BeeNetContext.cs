using BeeNetServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeeNetServer.Data
{
    public class BeeNetContext : DbContext
    {
        public BeeNetContext(DbContextOptions<BeeNetContext> options) : base(options)
        { }

        public DbSet<Picture> Pictures { get; set; }
        public DbSet<ScreenShot> ScreenShots { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<PictureLabel> PictureLabels { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PictureLabel>()
                .HasKey(t => new { t.LabelName, t.PictureId });
            modelBuilder.Entity<WorkspacePicture>()
                .HasKey(w => new { w.PictureId, w.WorkspaceName });

            modelBuilder.Entity<Picture>()
                .HasIndex(t => t.MD5)
                .IsUnique();
            modelBuilder.Entity<Picture>()
                .Property(p => p.CreatedTime)
                .HasDefaultValueSql("datetime('now','localtime')");
            modelBuilder.Entity<Picture>()
                .HasIndex(p => p.CreatedTime);
            modelBuilder.Entity<Label>()
                .Property(l => l.Num)
                .HasDefaultValue(0);
            modelBuilder.Entity<Label>()
                .HasIndex(l => l.Num);
            modelBuilder.Entity<Workspace>()
                .HasIndex(w => w.Index);
        }



    }
}
