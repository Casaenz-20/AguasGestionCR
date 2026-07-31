using AguasGestionCR.Local_cofig;
using AguasGestionCR.LocalConfig;
using AguasGestionCR.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;


namespace AguasGestionCR.Models;

public partial class AcueductoDbContext : DbContext
{
    public AcueductoDbContext()
    {
    }

    public AcueductoDbContext(DbContextOptions<AcueductoDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
       => optionsBuilder.UseSqlServer(LocalConfig.LocalConfig.CadenaConexion);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Clientes__71ABD0A7D155C412");

            entity.HasIndex(e => e.Identificacion, "UQ__Clientes__D6F931E5E2B3B81A").IsUnique();

            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.CodigoCliente)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasComputedColumnSql("('CLT-'+right('000000'+CONVERT([varchar](6),[ClienteID]),(6)))", false);
            entity.Property(e => e.CorreoElectronico).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(255);
            entity.Property(e => e.DocumentoCedulaPdf).HasColumnName("DocumentoCedulaPDF");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Activo");
            entity.Property(e => e.EstadoPrevista)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Identificacion).HasMaxLength(30);
            entity.Property(e => e.NombreArchivoCedula).HasMaxLength(255);
            entity.Property(e => e.NombreCompleto).HasMaxLength(150);
            entity.Property(e => e.NumeroMedidor).HasMaxLength(50);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.UltimaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE83E51B6E6C");

            entity.HasIndex(e => e.CodigoProducto, "UQ__Producto__785B009F5A1BD69F").IsUnique();

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CantidadMinima)
                .HasDefaultValue(20.00m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Categoria).HasMaxLength(50);
            entity.Property(e => e.CodigoProducto).HasMaxLength(50);
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Activo");
            entity.Property(e => e.FechaIngreso).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Unidad).HasMaxLength(30);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE798E5F05EF1");

            entity.HasIndex(e => e.NombreUsuario, "UQ__Usuarios__6B0F5AE001F306B6").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.ContrasenaHash).HasMaxLength(256);
            entity.Property(e => e.CorreoElectronico).HasMaxLength(100);
            entity.Property(e => e.Estado).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Identificacion).HasMaxLength(30);
            entity.Property(e => e.NombreCompleto).HasMaxLength(150);
            entity.Property(e => e.NombreUsuario).HasMaxLength(50);
            entity.Property(e => e.NumeroMedidor).HasMaxLength(50);
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .HasDefaultValue("Cliente");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
