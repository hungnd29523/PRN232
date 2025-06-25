using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MICHO.API.Models;

public partial class Prn232Pt2Context : DbContext
{
    public Prn232Pt2Context()
    {
    }

    public Prn232Pt2Context(DbContextOptions<Prn232Pt2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<IceCream> IceCreams { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderDetailIceCream> OrderDetailIceCreams { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DBContext");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8DDCF3878");

            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Contact).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmpId).HasName("PK__Employee__AF2DBA797D153061");

            entity.ToTable("Employee");

            entity.Property(e => e.EmpId).HasColumnName("EmpID");
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.EmpName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.Idcard)
                .HasMaxLength(20)
                .HasColumnName("IDCard");
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<IceCream>(entity =>
        {
            entity.HasKey(e => e.IceId).HasName("PK__IceCream__A82E24CB30F74C33");

            entity.ToTable("IceCream");

            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.Flavor).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAFCB132923");

            entity.ToTable("Order");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.EmpId).HasColumnName("EmpID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order__CustomerI__3D5E1FD2");

            entity.HasOne(d => d.Emp).WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmpId)
                .HasConstraintName("FK__Order__EmpID__3E52440B");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C271B36C7");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__412EB0B6");
        });

        modelBuilder.Entity<OrderDetailIceCream>(entity =>
        {
            entity.HasKey(e => new { e.OrderDetailId, e.IceId }).HasName("PK__OrderDet__793B3140669157C6");

            entity.ToTable("OrderDetailIceCream");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Ice).WithMany(p => p.OrderDetailIceCreams)
                .HasForeignKey(d => d.IceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__IceId__45F365D3");

            entity.HasOne(d => d.OrderDetail).WithMany(p => p.OrderDetailIceCreams)
                .HasForeignKey(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderDeta__Order__440B1D61");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetailIceCreams)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__44FF419A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
