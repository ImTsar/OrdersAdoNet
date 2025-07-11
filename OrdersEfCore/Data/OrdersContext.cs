using OrdersEfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace OrdersEfCore.Data
{
    public class OrdersContext: DbContext
    {
        public OrdersContext(DbContextOptions<OrdersContext> options)
        : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Analysis> Analyses { get; set; }
        public DbSet<Group> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("ord_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.CreationDate)
                    .HasColumnName("ord_datetime")
                    .HasColumnType("datetime")
                    .IsRequired();

                entity.Property(e => e.AnalysisId)
                    .HasColumnName("ord_an")
                    .IsRequired();

                entity.HasOne(e => e.Analysis)
                      .WithMany(a => a.Orders)
                      .HasForeignKey(e => e.AnalysisId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Analysis>(entity =>
            {
                entity.ToTable("Analysis");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("an_id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("an_name")
                    .HasMaxLength(100)
                    .IsUnicode(true)
                    .IsRequired();

                entity.Property(e => e.Cost)
                    .HasColumnName("an_cost")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(e => e.Price)
                    .HasColumnName("an_price")
                    .HasColumnType("decimal(10,2)")
                    .IsRequired();

                entity.Property(e => e.GroupId)
                    .HasColumnName("an_group")
                    .IsRequired();

                entity.HasOne(e => e.Group)
                      .WithMany(g => g.Analyses)
                      .HasForeignKey(e => e.GroupId);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Groups");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                       .HasColumnName("gr_id")
                       .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("gr_name")      
                    .HasMaxLength(100)            
                    .IsUnicode(true)
                    .IsRequired();

                entity.Property(e => e.StorageTemp)
                    .HasColumnName("gr_temp")   
                    .HasMaxLength(50)
                    .IsUnicode(true)
                    .IsRequired();
            });
        }
    }
}
