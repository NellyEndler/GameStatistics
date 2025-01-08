﻿// <auto-generated />
using GameStatistics.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GameStatistics.Migrations
{
    [DbContext(typeof(GameStatisticsContext))]
    [Migration("20250108145705_UpdateWorkshopModel")]
    partial class UpdateWorkshopModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GameStatistics.Models.Workshop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("TotalWorkshopTimeInSeconds")
                        .HasColumnType("float");

                    b.Property<int>("WorkShopVisits")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Workshops");
                });
#pragma warning restore 612, 618
        }
    }
}
