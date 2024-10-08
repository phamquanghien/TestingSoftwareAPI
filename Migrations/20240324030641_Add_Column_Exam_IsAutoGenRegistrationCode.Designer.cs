﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestingSoftwareAPI.Data;

#nullable disable

namespace TestingSoftwareAPI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240324030641_Add_Column_Exam_IsAutoGenRegistrationCode")]
    partial class Add_Column_Exam_IsAutoGenRegistrationCode
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.3");

            modelBuilder.Entity("TestingSoftwareAPI.Models.ActionHistory", b =>
                {
                    b.Property<Guid>("ActionHistoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Detail")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("ActionHistoryID");

                    b.ToTable("ActionHistories");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.Exam", b =>
                {
                    b.Property<int>("ExamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("CreatePerson")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExamCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExamName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsAutoGenRegistrationCode")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsDelete")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Note")
                        .HasColumnType("TEXT");

                    b.Property<int>("StartRegistrationCode")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ExamId");

                    b.ToTable("Exam");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.ExamResult", b =>
                {
                    b.Property<Guid>("ExamResultID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AverageScore")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("TEXT");

                    b.Property<int>("ExamBag")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExamCodeNumber")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExamId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExamResult1")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("TEXT");

                    b.Property<string>("ExamResult2")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsReview")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ReviewScore")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("ExamResultID");

                    b.ToTable("ExamResults");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.RegistrationCode", b =>
                {
                    b.Property<Guid>("RegistrationCodeID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("ExamId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RegistrationCodeNumber")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("StudentExamID")
                        .HasColumnType("TEXT");

                    b.HasKey("RegistrationCodeID");

                    b.HasIndex("ExamId");

                    b.HasIndex("StudentExamID");

                    b.ToTable("RegistrationCodes");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.Student", b =>
                {
                    b.Property<string>("StudentCode")
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("BirthDay")
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.HasKey("StudentCode");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.StudentExam", b =>
                {
                    b.Property<Guid>("StudentExamID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ClassName")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExamBag")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExamId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IdentificationNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsActive")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LessonNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LessonStart")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("StudentCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TestDay")
                        .HasColumnType("TEXT");

                    b.Property<string>("TestRoom")
                        .HasColumnType("TEXT");

                    b.HasKey("StudentExamID");

                    b.HasIndex("StudentCode");

                    b.ToTable("StudentExams");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.SubjectExams", b =>
                {
                    b.Property<Guid>("SubjectExamID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("ExamBag")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ExamId")
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsDownloadFileDiem")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsEnterCandidatesAbsent")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMatchingTestScore")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SubjectCode")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SubjectName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserEnterCandidatesAbsent")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserMatchingTestScore")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("SubjectExamID");

                    b.ToTable("SubjectExams");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.RegistrationCode", b =>
                {
                    b.HasOne("TestingSoftwareAPI.Models.Exam", "Exam")
                        .WithMany()
                        .HasForeignKey("ExamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TestingSoftwareAPI.Models.StudentExam", "StudentExam")
                        .WithMany()
                        .HasForeignKey("StudentExamID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exam");

                    b.Navigation("StudentExam");
                });

            modelBuilder.Entity("TestingSoftwareAPI.Models.StudentExam", b =>
                {
                    b.HasOne("TestingSoftwareAPI.Models.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentCode")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Student");
                });
#pragma warning restore 612, 618
        }
    }
}
