﻿using BeeNetServer.Models;
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
        public DbSet<Label> Labels { get; set; }
        public DbSet<PictureLabel> PictureLabels { get; set; }
        public DbSet<Workspace> Workspaces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PictureLabel>()
                .HasKey(t => new { t.PictureId, t.LabelName });
            modelBuilder.Entity<PictureLabel>()
                .HasOne(pt => pt.Picture)
                .WithMany(t => t.PictureLabels)
                .HasForeignKey(pt => pt.PictureId);
            modelBuilder.Entity<Workspace>()
                .HasMany(w=>w.Pictures)
                .WithOne()
                .HasForeignKey()

            modelBuilder.Entity<Picture>()
                .HasIndex(t => t.MD5)
                .IsUnique();
            modelBuilder.Entity<Picture>()
                .Property(p => p.CreatedTime)
                .HasDefaultValueSql("datetime('now','localtime')");
            modelBuilder.Entity<Picture>()
                .Property(p => p.ModifiedTime)
                .HasDefaultValueSql("datetime('now','localtime')");
            modelBuilder.Entity<Label>()
                .Property(l => l.Num)
                .HasDefaultValue(0);
            modelBuilder.Entity<Label>()
                .Property(l=>l.CreatedTime)
                .HasDefaultValueSql("datetime('now','localtime')");
            modelBuilder.Entity<Label>()
                .Property(l => l.ModifiedTime)
                .HasDefaultValueSql("datetime('now','localtime')");
        }

        

    }
}
