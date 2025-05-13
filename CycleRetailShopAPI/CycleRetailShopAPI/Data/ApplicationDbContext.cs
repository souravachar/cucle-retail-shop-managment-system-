using Microsoft.EntityFrameworkCore;
using CycleRetailShopAPI.Models;
using System.Collections.Generic;

namespace CycleRetailShopAPI.Data // Make sure this namespace matches your project
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Cycle> Cycles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique Constraint for Email in User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Unique Constraint for Email in Customer
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();


            // One-to-Many: Customer → CustomerAddresses
            modelBuilder.Entity<CustomerAddress>()
                .HasOne(ca => ca.Customer)
                .WithMany(c => c.Addresses)  // ✅ New Relationship
                .HasForeignKey(ca => ca.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);


            // One-to-Many: User (Employee) → Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Employee)
                .WithMany()
                .HasForeignKey(o => o.EmployeeID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: Customer → Orders
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
               .HasMany(o => o.OrderDetails)
               .WithOne(od => od.Order)
               .HasForeignKey(od => od.OrderID)
               .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Order → OrderDetails
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany()
                .HasForeignKey(od => od.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Cycle → OrderDetails
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Cycle)
                .WithMany()
                .HasForeignKey(od => od.CycleID)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-One: Order → Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne()
                .HasForeignKey<Payment>(p => p.OrderID)
                .OnDelete(DeleteBehavior.Cascade);

            //------------------------------------------------------------

            // ✅ One-to-Many: Customer → CustomerAddresses
            modelBuilder.Entity<CustomerAddress>()
                .HasOne(ca => ca.Customer)
                .WithMany(c => c.Addresses)
                .HasForeignKey(ca => ca.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ One-to-One: Order → Address
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressID)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
