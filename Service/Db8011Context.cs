using System;
using System.Collections.Generic;
using Lab2Places.Models;
using Lab2Places.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Lab2Places.Service;

public partial class Db8011Context : DbContext
{
    public Db8011Context()
    {
    }

    public Db8011Context(DbContextOptions<Db8011Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AllPack> AllPacks { get; set; }

    public virtual DbSet<Pack> Packs { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<PlaceInPack> PlaceInPacks { get; set; }

    public virtual DbSet<PlacesType> PlacesTypes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPack> UserPacks { get; set; }

    public virtual DbSet<UserReview> UserReviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=db8011.public.databaseasp.net; Database=db8011; User Id=db8011; Password=9t#PR3g!2?Ac; Encrypt=False; MultipleActiveResultSets=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AllPack>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("AllPacks");

            entity.Property(e => e.PackName)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Pack>(entity =>
        {
            entity.HasKey(e => e.PackId).HasName("PK__Packs__FA6765691C3C53A6");

            entity.Property(e => e.PackName)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Packs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Packs__UserId__440B1D61");
        });

        modelBuilder.Entity<Place>(entity =>
        {
            entity.HasKey(e => e.PlaceId).HasName("PK__Places__D5222B6EFB389D59");

            entity.Property(e => e.GeolocationA).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.GeolocationB).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.PlaceDescription)
                .HasMaxLength(60)
                .IsUnicode(false);

            entity.HasOne(d => d.Type).WithMany(p => p.Places)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Places__TypeId__3D5E1FD2");
        });

        modelBuilder.Entity<PlaceInPack>(entity =>
        {
            entity.HasKey(e => e.PipId).HasName("PK__PlaceInP__145FF26C09E955D9");

            entity.ToTable("PlaceInPack");

            entity.Property(e => e.PipId).HasColumnName("PIP_Id");

            entity.HasOne(d => d.Pack).WithMany(p => p.PlaceInPacks)
                .HasForeignKey(d => d.PackId)
                .HasConstraintName("FK__PlaceInPa__PackI__47DBAE45");

            entity.HasOne(d => d.Place).WithMany(p => p.PlaceInPacks)
                .HasForeignKey(d => d.PlaceId)
                .HasConstraintName("FK__PlaceInPa__Place__46E78A0C");
        });

        modelBuilder.Entity<PlacesType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__PlacesTy__516F03B5FEC5AAB8");

            entity.HasIndex(e => e.TypeName, "UQ__PlacesTy__D4E7DFA8B9F0B283").IsUnique();

            entity.Property(e => e.TypeName)
                .HasMaxLength(40)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE73E948AA");

            entity.Property(e => e.ReviewText).HasMaxLength(2000);

            entity.HasOne(d => d.Place).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.PlaceId)
                .HasConstraintName("FK__Reviews__PlaceId__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__UserId__403A8C7D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC20C9558");

            entity.HasIndex(e => e.UserLogin, "UQ__Users__7F8E8D5EF25219E4").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserLogin)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UserPassword)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserPack>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserPacks");

            entity.Property(e => e.PackName)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserLogin)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserReview>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("UserReviews");

            entity.Property(e => e.PlaceDescription)
                .HasMaxLength(60)
                .IsUnicode(false);
            entity.Property(e => e.ReviewText).HasMaxLength(2000);
            entity.Property(e => e.UserLogin)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
