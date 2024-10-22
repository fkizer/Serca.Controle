﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Serca.Controle.Core.Application.Data;

#nullable disable

namespace Serca.Controle.Core.Application.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241022100435_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("Serca.Controle.Core.Domain.Entities.DeviceParametersEntity", b =>
                {
                    b.Property<int>("UtilisateurId")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "preparateur");

                    b.Property<bool>("IsPilote")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "pilote");

                    b.HasKey("UtilisateurId");

                    b.ToTable("DeviceParameters");
                });

            modelBuilder.Entity("Serca.Controle.Core.Domain.Entities.Trace", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("Code")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("Group")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Traces");
                });

            modelBuilder.Entity("Serca.Controle.Core.Domain.Entities.UtilisateurEntity", b =>
                {
                    b.Property<int>("UtilisateurId")
                        .HasColumnType("INTEGER")
                        .HasAnnotation("Relational:JsonPropertyName", "utilisateur");

                    b.Property<string>("Nom")
                        .HasColumnType("TEXT")
                        .HasAnnotation("Relational:JsonPropertyName", "nom");

                    b.HasKey("UtilisateurId");

                    b.ToTable("Utilisateurs");
                });
#pragma warning restore 612, 618
        }
    }
}
