using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SchoolJournalEF.Models;

namespace SchoolJournalEF.Data;

public partial class SchoolJournalDbContext : DbContext
{
    public SchoolJournalDbContext(DbContextOptions<SchoolJournalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectHour> SubjectHours { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<VFinalGrade> VFinalGrades { get; set; }

    public virtual DbSet<VSchoolTimetable> VSchoolTimetables { get; set; }

    public virtual DbSet<VSubjectStatsByClass> VSubjectStatsByClasses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.AcademicYear).HasMaxLength(9);
            entity.Property(e => e.ClassLetter)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.HomeroomTeacherId).HasColumnName("HomeroomTeacherID");

            entity.HasOne(d => d.HomeroomTeacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.HomeroomTeacherId)
                .HasConstraintName("FK_Classes_Teachers");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.ToTable("Journal");

            entity.Property(e => e.JournalId).HasColumnName("JournalID");
            entity.Property(e => e.AbsenceReason).HasMaxLength(50);
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.WorkType).HasMaxLength(50);

            entity.HasOne(d => d.Lesson).WithMany(p => p.Journals)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Lessons");

            entity.HasOne(d => d.Student).WithMany(p => p.Journals)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Students");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.Property(e => e.LessonId).HasColumnName("LessonID");
            entity.Property(e => e.ClassId).HasColumnName("ClassID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.Topic).HasMaxLength(250);

            entity.HasOne(d => d.Class).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lessons_Classes");

            entity.HasOne(d => d.Subject).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Lessons_Subjects");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Lessons_Teachers");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.ContactInfo).HasMaxLength(250);
            entity.Property(e => e.CurrentClassId).HasColumnName("CurrentClassID");
            entity.Property(e => e.FullName).HasMaxLength(150);

            entity.HasOne(d => d.CurrentClass).WithMany(p => p.Students)
                .HasForeignKey(d => d.CurrentClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Classes");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.ExamType).HasMaxLength(50);
            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<SubjectHour>(entity =>
        {
            entity.HasKey(e => new { e.SubjectId, e.GradeLevel });

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Subject).WithMany(p => p.SubjectHours)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_SubjectHours_Subjects");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.QualificationCategory).HasMaxLength(50);
        });

        modelBuilder.Entity<VFinalGrade>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_FinalGrades");

            entity.Property(e => e.AcademicYear).HasMaxLength(9);
            entity.Property(e => e.PeriodType).HasMaxLength(10);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.StudentName).HasMaxLength(150);
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<VSchoolTimetable>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_SchoolTimetable");

            entity.Property(e => e.IdУрока).HasColumnName("ID Урока");
            entity.Property(e => e.Класс).HasMaxLength(14);
            entity.Property(e => e.Предмет).HasMaxLength(100);
            entity.Property(e => e.ТемаЗанятия)
                .HasMaxLength(250)
                .HasColumnName("Тема занятия");
            entity.Property(e => e.ТипАттестации)
                .HasMaxLength(50)
                .HasColumnName("Тип аттестации");
            entity.Property(e => e.Урока).HasColumnName("№ Урока");
        });

        modelBuilder.Entity<VSubjectStatsByClass>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_SubjectStatsByClass");

            entity.Property(e => e.ВсегоПроведенныхУроков).HasColumnName("Всего проведенных уроков");
            entity.Property(e => e.Класс).HasMaxLength(14);
            entity.Property(e => e.НазваниеПредмета)
                .HasMaxLength(100)
                .HasColumnName("Название предмета");
            entity.Property(e => e.УчебныйГод)
                .HasMaxLength(9)
                .HasColumnName("Учебный год");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
