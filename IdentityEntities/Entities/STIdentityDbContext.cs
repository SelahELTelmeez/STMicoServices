using IdentityEntities.Entities.Grades;
using IdentityEntities.Entities.Identities;
using IdentityEntities.Entities.Locations;
using IdentityEntities.Entities.Subjects;
using IdentityEntities.Shared.Identities;
using Microsoft.EntityFrameworkCore;

namespace IdentityEntities.Entities;
public class STIdentityDbContext : DbContext
{
    public STIdentityDbContext(DbContextOptions<STIdentityDbContext> options) : base(options)
    {
    }
    public DbSet<Avatar> Avatars { get; set; }
    public DbSet<ExternalIdentityProvider> ExternalIdentityProviders { get; set; }
    public DbSet<IdentityRelation> IdentityRelations { get; set; }
    public DbSet<IdentityRole> IdentityRoles { get; set; }
    public DbSet<IdentitySchool> IdentitySchools { get; set; }
    public DbSet<IdentityUser> IdentityUsers { get; set; }
    public DbSet<IdentityActivation> IdentityActivations { get; set; }
    public DbSet<IdentityReferralTracker> IdentityReferralTrackers { get; set; }
    public DbSet<IdentitySubject> IdentitySubjects { get; set; }
    public DbSet<Governorate> Governorates { get; set; }
    public DbSet<Grade> Grades { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Avatar>().Property(e => e.Id).ValueGeneratedNever();
        //https://selaheltelmeez.com/Media21-22/LMSApp/avatar/Default/default.png

        modelBuilder.Entity<Avatar>().HasData(
            new Avatar { Id = 0, AvatarType = AvatarType.Default, ImageUrl = "default.png" },
            new Avatar { Id = 1, AvatarType = AvatarType.Student, ImageUrl = "01.png" },
            new Avatar { Id = 2, AvatarType = AvatarType.Student, ImageUrl = "02.png" },
            new Avatar { Id = 3, AvatarType = AvatarType.Student, ImageUrl = "03.png" },
            new Avatar { Id = 5, AvatarType = AvatarType.Student, ImageUrl = "04.png" },
            new Avatar { Id = 6, AvatarType = AvatarType.Student, ImageUrl = "05.png" },
            new Avatar { Id = 7, AvatarType = AvatarType.Student, ImageUrl = "06.png" },
            new Avatar { Id = 8, AvatarType = AvatarType.Student, ImageUrl = "07.png" },
            new Avatar { Id = 9, AvatarType = AvatarType.Student, ImageUrl = "08.png" },
            new Avatar { Id = 10, AvatarType = AvatarType.Parent, ImageUrl = "01.png" },
            new Avatar { Id = 11, AvatarType = AvatarType.Parent, ImageUrl = "02.png" },
            new Avatar { Id = 12, AvatarType = AvatarType.Parent, ImageUrl = "03.png" },
            new Avatar { Id = 13, AvatarType = AvatarType.Parent, ImageUrl = "04.png" },
            new Avatar { Id = 14, AvatarType = AvatarType.Parent, ImageUrl = "05.png" },
            new Avatar { Id = 15, AvatarType = AvatarType.Parent, ImageUrl = "06.png" },
            new Avatar { Id = 16, AvatarType = AvatarType.Parent, ImageUrl = "07.png" },
            new Avatar { Id = 17, AvatarType = AvatarType.Parent, ImageUrl = "08.png" },
            new Avatar { Id = 18, AvatarType = AvatarType.Teacher, ImageUrl = "01.png" },
            new Avatar { Id = 19, AvatarType = AvatarType.Teacher, ImageUrl = "02.png" },
            new Avatar { Id = 20, AvatarType = AvatarType.Teacher, ImageUrl = "03.png" },
            new Avatar { Id = 21, AvatarType = AvatarType.Teacher, ImageUrl = "04.png" },
            new Avatar { Id = 22, AvatarType = AvatarType.Teacher, ImageUrl = "05.png" },
            new Avatar { Id = 23, AvatarType = AvatarType.Teacher, ImageUrl = "06.png" },
            new Avatar { Id = 24, AvatarType = AvatarType.Teacher, ImageUrl = "07.png" },
            new Avatar { Id = 25, AvatarType = AvatarType.Teacher, ImageUrl = "08.png" },
            new Avatar { Id = 26, AvatarType = AvatarType.Student, ImageUrl = "09.png" },
            new Avatar { Id = 27, AvatarType = AvatarType.Student, ImageUrl = "10.png" },
            new Avatar { Id = 28, AvatarType = AvatarType.Student, ImageUrl = "11.png" },
            new Avatar { Id = 29, AvatarType = AvatarType.Student, ImageUrl = "12.png" },
            new Avatar { Id = 30, AvatarType = AvatarType.Student, ImageUrl = "13.png" },
            new Avatar { Id = 31, AvatarType = AvatarType.Student, ImageUrl = "14.png" },
            new Avatar { Id = 32, AvatarType = AvatarType.Student, ImageUrl = "15.png" },
            new Avatar { Id = 33, AvatarType = AvatarType.Student, ImageUrl = "16.png" },
            new Avatar { Id = 34, AvatarType = AvatarType.Teacher, ImageUrl = "09.png" },
            new Avatar { Id = 35, AvatarType = AvatarType.Teacher, ImageUrl = "10.png" },
            new Avatar { Id = 36, AvatarType = AvatarType.Teacher, ImageUrl = "11.png" },
            new Avatar { Id = 37, AvatarType = AvatarType.Teacher, ImageUrl = "12.png" },
            new Avatar { Id = 38, AvatarType = AvatarType.Teacher, ImageUrl = "13.png" },
            new Avatar { Id = 39, AvatarType = AvatarType.Teacher, ImageUrl = "14.png" },
            new Avatar { Id = 40, AvatarType = AvatarType.Teacher, ImageUrl = "15.png" },
            new Avatar { Id = 41, AvatarType = AvatarType.Teacher, ImageUrl = "16.png" });


        modelBuilder.Entity<Governorate>().HasData(
            new Governorate { Id = 1, Name = "القاهرة", IsEnabled = true },
            new Governorate { Id = 2, Name = "الجيزة", IsEnabled = true },
            new Governorate { Id = 3, Name = "حلوان", IsEnabled = false },
            new Governorate { Id = 4, Name = "الدقهلية", IsEnabled = true },
            new Governorate { Id = 5, Name = "المنوفية", IsEnabled = true },
            new Governorate { Id = 6, Name = "الاسكندرية", IsEnabled = true },
            new Governorate { Id = 7, Name = "الشرقية", IsEnabled = true },
            new Governorate { Id = 8, Name = "الغربية", IsEnabled = true },
            new Governorate { Id = 9, Name = "القليوبية", IsEnabled = true },
            new Governorate { Id = 10, Name = "بورسعيد", IsEnabled = true },
            new Governorate { Id = 11, Name = "اسوان", IsEnabled = true },
            new Governorate { Id = 12, Name = "6 أكتوبر", IsEnabled = false },
            new Governorate { Id = 13, Name = "اسيوط", IsEnabled = true },
            new Governorate { Id = 14, Name = "كفر الشيخ", IsEnabled = true },
            new Governorate { Id = 15, Name = "السويس", IsEnabled = true },
            new Governorate { Id = 16, Name = "بنى سويف", IsEnabled = true },
            new Governorate { Id = 17, Name = "الفيوم", IsEnabled = true },
            new Governorate { Id = 18, Name = "البحيرة", IsEnabled = true },
            new Governorate { Id = 19, Name = "المنيا", IsEnabled = true },
            new Governorate { Id = 20, Name = "سوهاج", IsEnabled = true },
            new Governorate { Id = 21, Name = "الاسماعيلية", IsEnabled = true },
            new Governorate { Id = 22, Name = "شمال سيناء", IsEnabled = true },
            new Governorate { Id = 23, Name = "دمياط", IsEnabled = true },
            new Governorate { Id = 24, Name = "الاقصر", IsEnabled = true },
            new Governorate { Id = 25, Name = "جنوب سيناء", IsEnabled = true },
            new Governorate { Id = 26, Name = "البحر الاحمر", IsEnabled = true },
            new Governorate { Id = 27, Name = "قنا", IsEnabled = true },
            new Governorate { Id = 28, Name = "الوادى الجديد", IsEnabled = true },
            new Governorate { Id = 29, Name = "مرسى مطروح", IsEnabled = true });

        modelBuilder.Entity<Grade>().HasData(
           new Grade { Id = 1, Name = "KG1" },
           new Grade { Id = 2, Name = "KG2" },
           new Grade { Id = 3, Name = "الصف الأول الإبتدائى" },
           new Grade { Id = 4, Name = "الصف الثانى الإبتدائى" },
           new Grade { Id = 5, Name = "الصف الثالث الإبتدائى" },
           new Grade { Id = 6, Name = "الصف الرابع الإبتدائى" },
           new Grade { Id = 7, Name = "الصف الخامس الإبتدائى" },
           new Grade { Id = 8, Name = "الصف السادس الإبتدائى" },
           new Grade { Id = 9, Name = "الصف الأول الإعدادى" },
           new Grade { Id = 10, Name = "الصف الثانى الإعدادى" },
           new Grade { Id = 11, Name = "الصف الثالث الإعدادى" },
           new Grade { Id = 12, Name = "الصف الأول الثانوى" },
           new Grade { Id = 13, Name = "الصف الثانى الثانوى" },
           new Grade { Id = 14, Name = "الثانوية العامة" });

        base.OnModelCreating(modelBuilder);
    }
}
