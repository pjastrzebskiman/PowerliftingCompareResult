﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PowerliftingCompareResult.Models;

#nullable disable

namespace PowerliftingCompareResult.Migrations
{
    [DbContext(typeof(ResultContext))]
    partial class ResultContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PowerliftingCompareResult.Models.LiftResult", b =>
                {
                    b.Property<float>("Age")
                        .HasColumnType("real");

                    b.Property<string>("AgeClass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Bench")
                        .HasColumnType("real");

                    b.Property<float>("Bench1")
                        .HasColumnType("real");

                    b.Property<float>("Bench2")
                        .HasColumnType("real");

                    b.Property<float>("Bench3")
                        .HasColumnType("real");

                    b.Property<float>("BodyWeight")
                        .HasColumnType("real");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<float>("Deadlift")
                        .HasColumnType("real");

                    b.Property<float>("Deadlift1")
                        .HasColumnType("real");

                    b.Property<float>("Deadlift2")
                        .HasColumnType("real");

                    b.Property<float>("Deadlift3")
                        .HasColumnType("real");

                    b.Property<string>("EQ")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Federation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MeetCountry")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MeetName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Sex")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Squat")
                        .HasColumnType("real");

                    b.Property<float>("Squat1")
                        .HasColumnType("real");

                    b.Property<float>("Squat2")
                        .HasColumnType("real");

                    b.Property<float>("Squat3")
                        .HasColumnType("real");

                    b.Property<float>("Total")
                        .HasColumnType("real");

                    b.Property<string>("WeightClass")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("LiftResults");
                });
#pragma warning restore 612, 618
        }
    }
}
