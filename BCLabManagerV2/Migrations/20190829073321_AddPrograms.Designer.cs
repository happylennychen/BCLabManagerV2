﻿// <auto-generated />
using System;
using BCLabManager.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BCLabManager.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20190829073321_AddPrograms")]
    partial class AddPrograms
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("BCLabManager.Model.AssetUsageRecordClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BatteryClassId");

                    b.Property<string>("ProgramName");

                    b.Property<int>("Status");

                    b.Property<string>("SubProgramName");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("BatteryClassId");

                    b.ToTable("AssetUsageRecordClass");
                });

            modelBuilder.Entity("BCLabManager.Model.BatteryClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("BatteryTypeId");

                    b.Property<double>("CycleCount");

                    b.Property<string>("Name");

                    b.Property<int>("Status");

                    b.HasKey("Id");

                    b.HasIndex("BatteryTypeId");

                    b.ToTable("Batteries");
                });

            modelBuilder.Entity("BCLabManager.Model.BatteryTypeClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CutoffDischargeVoltage");

                    b.Property<int>("LimitedChargeVoltage");

                    b.Property<string>("Manufactor");

                    b.Property<string>("Material");

                    b.Property<string>("Name");

                    b.Property<int>("NominalVoltage");

                    b.Property<int>("RatedCapacity");

                    b.Property<int>("TypicalCapacity");

                    b.HasKey("Id");

                    b.ToTable("BatteryTypes");
                });

            modelBuilder.Entity("BCLabManager.Model.ProgramClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CompleteDate");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<DateTime>("RequestDate");

                    b.Property<string>("Requester");

                    b.HasKey("Id");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("BCLabManager.Model.RawDataClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.HasKey("Id");

                    b.ToTable("RawDataClass");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int?>("ProgramClassId");

                    b.Property<int>("TestCount");

                    b.HasKey("Id");

                    b.HasIndex("ProgramClassId");

                    b.ToTable("SubPrograms");
                });

            modelBuilder.Entity("BCLabManager.Model.TestRecordClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BatteryStr");

                    b.Property<string>("BatteryTypeStr");

                    b.Property<string>("ChamberStr");

                    b.Property<string>("ChannelStr");

                    b.Property<string>("Comment");

                    b.Property<DateTime>("EndTime");

                    b.Property<double>("NewCycle");

                    b.Property<string>("ProgramStr");

                    b.Property<int?>("RawDataId");

                    b.Property<DateTime>("StartTime");

                    b.Property<int>("Status");

                    b.Property<string>("Steps");

                    b.Property<int?>("SubProgramClassId");

                    b.Property<int?>("SubProgramClassId1");

                    b.Property<string>("SubProgramStr");

                    b.Property<string>("TesterStr");

                    b.HasKey("Id");

                    b.HasIndex("RawDataId");

                    b.HasIndex("SubProgramClassId");

                    b.HasIndex("SubProgramClassId1");

                    b.ToTable("TestRecords");
                });

            modelBuilder.Entity("BCLabManager.Model.AssetUsageRecordClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryClass")
                        .WithMany("Records")
                        .HasForeignKey("BatteryClassId");
                });

            modelBuilder.Entity("BCLabManager.Model.BatteryClass", b =>
                {
                    b.HasOne("BCLabManager.Model.BatteryTypeClass", "BatteryType")
                        .WithMany()
                        .HasForeignKey("BatteryTypeId");
                });

            modelBuilder.Entity("BCLabManager.Model.SubProgramClass", b =>
                {
                    b.HasOne("BCLabManager.Model.ProgramClass")
                        .WithMany("SubPrograms")
                        .HasForeignKey("ProgramClassId");
                });

            modelBuilder.Entity("BCLabManager.Model.TestRecordClass", b =>
                {
                    b.HasOne("BCLabManager.Model.RawDataClass", "RawData")
                        .WithMany()
                        .HasForeignKey("RawDataId");

                    b.HasOne("BCLabManager.Model.SubProgramClass")
                        .WithMany("FirstTestRecords")
                        .HasForeignKey("SubProgramClassId");

                    b.HasOne("BCLabManager.Model.SubProgramClass")
                        .WithMany("SecondTestRecords")
                        .HasForeignKey("SubProgramClassId1");
                });
#pragma warning restore 612, 618
        }
    }
}
