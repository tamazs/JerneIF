using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public partial class JerneDbContext : DbContext
{
    public JerneDbContext(DbContextOptions<JerneDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Board> Boards { get; set; }

    public virtual DbSet<BoardNumber> BoardNumbers { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameWinningNumber> GameWinningNumbers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Board>(entity =>
        {
            entity.HasKey(e => e.BoardId).HasName("Boards_pkey");

            entity.Property(e => e.IsRepeating).HasDefaultValue(false);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.PurchasedAt).HasDefaultValueSql("now()");

            entity.HasOne(d => d.Game).WithMany(p => p.BoardGames)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_board_game");

            entity.HasOne(d => d.RepeatingUntilGame).WithMany(p => p.BoardRepeatingUntilGames)
                .HasForeignKey(d => d.RepeatingUntilGameId)
                .HasConstraintName("fk_board_repeating_until");

            entity.HasOne(d => d.User).WithMany(p => p.Boards)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_board_user");
        });

        modelBuilder.Entity<BoardNumber>(entity =>
        {
            entity.HasKey(e => e.BoardNumbersId).HasName("BoardNumbers_pkey");

            entity.HasIndex(e => e.BoardId, "BoardNumbers_BoardId_key").IsUnique();

            entity.HasOne(d => d.Board).WithOne(p => p.BoardNumber)
                .HasForeignKey<BoardNumber>(d => d.BoardId)
                .HasConstraintName("fk_boardnumbers_board");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.GameId).HasName("Games_pkey");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.Property(e => e.Status)
                .HasConversion<string>();

            entity.HasOne(d => d.PublishedByUser).WithMany(p => p.Games)
                .HasForeignKey(d => d.PublishedByUserId)
                .HasConstraintName("fk_game_published_by");
        });

        modelBuilder.Entity<GameWinningNumber>(entity =>
        {
            entity.HasKey(e => e.GameWinningNumbersId).HasName("GameWinningNumbers_pkey");

            entity.HasIndex(e => e.GameId, "GameWinningNumbers_GameId_key").IsUnique();

            entity.HasOne(d => d.Game).WithOne(p => p.GameWinningNumber)
                .HasForeignKey<GameWinningNumber>(d => d.GameId)
                .HasConstraintName("fk_gamewinningnumbers_game");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("Transactions_pkey");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            
            entity.Property(e => e.Status)
                .HasConversion<string>();

            entity.HasOne(d => d.ApprovedByUser).WithMany(p => p.TransactionApprovedByUsers)
                .HasForeignKey(d => d.ApprovedByUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_transaction_approved_by");

            entity.HasOne(d => d.User).WithMany(p => p.TransactionUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_transaction_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("Users_pkey");

            entity.HasIndex(e => e.Email, "Users_Email_key").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("now()");
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            
            entity.Property(e => e.Role)
                .HasConversion<string>();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
