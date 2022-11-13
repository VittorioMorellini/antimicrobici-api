using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Antimicrobici.Core.Models
{
    public partial class AmDbContext : DbContext
    {
        public AmDbContext()
        {
        }

        public AmDbContext(DbContextOptions<AmDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Company> Company { get; set; } = null!;
        public virtual DbSet<Delivery> Delivery { get; set; } = null!;
        public virtual DbSet<Group> Group { get; set; } = null!;
        public virtual DbSet<GroupMenu> GroupMenu { get; set; } = null!;
        public virtual DbSet<MatScaduto> MatScaduto { get; set; } = null!;
        public virtual DbSet<MatScadutoCatalogo> MatScadutoCatalogo { get; set; } = null!;
        public virtual DbSet<MatScadutoStruttura> MatScadutoStruttura { get; set; } = null!;
        public virtual DbSet<Menu> Menu { get; set; } = null!;
        public virtual DbSet<Principal> Principal { get; set; } = null!;
        public virtual DbSet<PrincipalGroup> PrincipalGroup { get; set; } = null!;
        public virtual DbSet<RichiestaImpegno> RichiestaImpegno { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=S-2020-000002\\Sqlexpress;Initial Catalog=Antimicrobici;Persist Security Info=False;User ID=sa;Password=sapwd");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Code)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InsertUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Mail)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TaxCode)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.VatCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InsertUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MedicalInsertDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MedicalInsertUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MedicalNotes)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.MedicalUpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.MedicalUpdateUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.NrImpegno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<GroupMenu>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupMenu)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupMenu_Group");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.GroupMenu)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupMenu_Menu");
            });

            modelBuilder.Entity<MatScaduto>(entity =>
            {
                entity.Property(e => e.CodCdc)
                    .HasMaxLength(24)
                    .IsUnicode(false);

                entity.Property(e => e.CodMateriale)
                    .HasMaxLength(18)
                    .IsUnicode(false);

                entity.Property(e => e.CompileUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ControlDate).HasColumnType("datetime");

                entity.Property(e => e.ExpireDate).HasColumnType("datetime");

                entity.Property(e => e.InsertDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.InsertUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Lotto)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Notes)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.QualificationUser)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RetirementDate).HasColumnType("datetime");

                entity.Property(e => e.RetirementNotes)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.RetirementUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<MatScadutoCatalogo>(entity =>
            {
                entity.Property(e => e.Azienda)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.CodGruppoMerceologico).HasMaxLength(2);

                entity.Property(e => e.CodMateriale).HasMaxLength(18);

                entity.Property(e => e.CodTipoMateriale).HasMaxLength(4);

                entity.Property(e => e.CodUnitaMisura).HasMaxLength(3);

                entity.Property(e => e.DescMateriale).HasMaxLength(40);

                entity.Property(e => e.DescTipoMateriale).HasMaxLength(25);

                entity.Property(e => e.DescUnitaMisura).HasMaxLength(30);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.MatScadutoCatalogo)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_MatScadutoCatalogo_Company");
            });

            modelBuilder.Entity<MatScadutoStruttura>(entity =>
            {
                entity.Property(e => e.Azienda)
                    .HasMaxLength(4)
                    .IsUnicode(false);

                entity.Property(e => e.Cdc).HasMaxLength(67);

                entity.Property(e => e.CdcOriginale).HasMaxLength(67);

                entity.Property(e => e.CodCdc).HasMaxLength(24);

                entity.Property(e => e.CodCdcOriginale).HasMaxLength(24);

                entity.Property(e => e.CodDipartimento).HasMaxLength(24);

                entity.Property(e => e.CodUo)
                    .HasMaxLength(24)
                    .HasColumnName("CodUO");

                entity.Property(e => e.Dipartimento).HasMaxLength(67);

                entity.Property(e => e.Uo)
                    .HasMaxLength(40)
                    .HasColumnName("UO");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.MatScadutoStruttura)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_MatScadutoStruttura_Company");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Icon)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UrlReport)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Menu_Menu");
            });

            modelBuilder.Entity<Principal>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LandingPage)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LockUserDate).HasColumnType("datetime");

                entity.Property(e => e.ModifyPasswordDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Qualification)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.WrongAccessDate).HasColumnType("datetime");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Principal)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Principal_Company");
            });

            modelBuilder.Entity<PrincipalGroup>(entity =>
            {
                entity.HasOne(d => d.Group)
                    .WithMany(p => p.PrincipalGroup)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrincipalGroup_Group");

                entity.HasOne(d => d.Principal)
                    .WithMany(p => p.PrincipalGroup)
                    .HasForeignKey(d => d.PrincipalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PrincipalGroup_Principal");
            });

            modelBuilder.Entity<RichiestaImpegno>(entity =>
            {
                entity.Property(e => e.Antibiogramma)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CdcRichiedente)
                    .HasMaxLength(24)
                    .IsUnicode(false);

                entity.Property(e => e.CodiceMateriale)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ConsulenzaInfettivologica)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DataCreazioneImpegno).HasColumnType("datetime");

                entity.Property(e => e.DataErogazione).HasColumnType("datetime");

                entity.Property(e => e.DescrizioneCdcRichiedente)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DescrizioneMateriale)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DestinatarioPazienteDettaglio)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InsertDate).HasColumnType("datetime");

                entity.Property(e => e.InsertUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MedicoRichiedente)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MedicoUo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("MedicoUO");

                entity.Property(e => e.Motivazione)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Note)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.NrImpegno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Posologia)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TipoImpegno)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
