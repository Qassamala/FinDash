﻿// <auto-generated />
using System;
using FinDash.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FinDash.Migrations
{
    [DbContext(typeof(FinDashDbContext))]
    [Migration("20231026073811_AddedCurrencyToStaticStockDataEntity")]
    partial class AddedCurrencyToStaticStockDataEntity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("FinDash.Models.StaticStockData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Symbol")
                        .IsUnique();

                    b.ToTable("StaticStockData");
                });

            modelBuilder.Entity("FinDash.Models.StockPrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int>("StaticStockDataId")
                        .HasColumnType("int");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("StaticStockDataId");

                    b.ToTable("StockPrices");
                });

            modelBuilder.Entity("FinDash.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FinDash.Models.UserStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("StaticStockDataId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StaticStockDataId");

                    b.HasIndex("UserId", "StaticStockDataId")
                        .IsUnique();

                    b.ToTable("UserStocks");
                });

            modelBuilder.Entity("FinDash.Models.StockPrice", b =>
                {
                    b.HasOne("FinDash.Models.StaticStockData", "StaticStockData")
                        .WithMany("StockPrices")
                        .HasForeignKey("StaticStockDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StaticStockData");
                });

            modelBuilder.Entity("FinDash.Models.UserStock", b =>
                {
                    b.HasOne("FinDash.Models.StaticStockData", "StaticStockData")
                        .WithMany("UserStocks")
                        .HasForeignKey("StaticStockDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FinDash.Models.User", "User")
                        .WithMany("UserStocks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StaticStockData");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FinDash.Models.StaticStockData", b =>
                {
                    b.Navigation("StockPrices");

                    b.Navigation("UserStocks");
                });

            modelBuilder.Entity("FinDash.Models.User", b =>
                {
                    b.Navigation("UserStocks");
                });
#pragma warning restore 612, 618
        }
    }
}
