using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CrawlingTask.Models;

public partial class IneichenContext : DbContext
{
    public IneichenContext()
    {
    }

    public IneichenContext(DbContextOptions<IneichenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auction> Auctions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Ineichen;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auction>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.EndMonth).HasColumnName("end_month");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.EndYear).HasColumnName("end_year");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("image_url");
            entity.Property(e => e.Link)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("link");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("location");
            entity.Property(e => e.LotCount).HasColumnName("lot_count");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.StartMonth).HasColumnName("start_month");
            entity.Property(e => e.StartTime).HasColumnName("start_time");
            entity.Property(e => e.StartYear).HasColumnName("start_year");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
