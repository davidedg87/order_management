﻿// <auto-generated />
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PhotoSiTest.OrderService.Data;

#nullable disable

namespace PhotoSiTest.OrderService.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    [DbContext(typeof(OrderContext))]
    [Migration("20241124103846_OrderConstraints")]
    partial class OrderConstraints
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PhotoSiTest.OrderService.Core.Models.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AddressId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset?>("DeletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<List<int>>("ProductIds")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("numeric");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IsDeleted");

                    b.HasIndex("UserId", "AddressId", "OrderDate")
                        .IsUnique();

                    b.ToTable("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}