using Microsoft.EntityFrameworkCore;
using NoteApp.Domain.Entities;
using NoteApp.Persistence.Contexts;

namespace NoteApp.Persistence.Seed;

public static class SeedData
{
    public static async Task InitializeAsync(NoteAppDbContext context)
    {
        try
        {
            // Veritabanı değişiklikleri kaydet
            await context.Database.EnsureCreatedAsync();
            
            // Rolleri ekle
            if (!await context.Roles.AnyAsync())
            {
                Console.WriteLine("Roller ekleniyor...");
                var adminRole = new Role { Name = "Admin", NormalizedName = "ADMIN", Description = "Sistem yöneticisi" };
                var userRole = new Role { Name = "User", NormalizedName = "USER", Description = "Standart kullanıcı" };
                var editorRole = new Role { Name = "Editor", NormalizedName = "EDITOR", Description = "İçerik düzenleyicisi" };

                await context.Roles.AddRangeAsync(adminRole, userRole, editorRole);
                await context.SaveChangesAsync();
                Console.WriteLine("Roller başarıyla eklendi.");
            }

            // Kullanıcıları ekle
            if (!await context.Users.AnyAsync())
            {
                Console.WriteLine("Kullanıcılar ekleniyor...");
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    PasswordHash = "6TahL8vLTZpVm6nqqJsJpNPzwbkpM3+UTH2w4jR/ZdAwbp1GoiLeJYl79yUG7+UUeRWQN3q9cLIN29QCpfpHiw==", // Şifre: Admin123!
                    PasswordSalt = "5yVrYVY4HQv4x0P7Oky1TA7nDFwNwQQEH8h1ThYIZvw3rj7+tzdYOrdNLiYnMkdVMn0Y46UOD2Gau7Q6qPsWf6dRwCRYEDn42dkzYLp9/C3+aNlZCbPPwOFr19/L1gw9jX8JZHVT09Hs7JRWQf6oxQ6jlFILyTC20iiH0ICJIqE=",
                    IsActive = true,
                    EmailConfirmed = true
                };

                var testUser = new User
                {
                    Username = "testuser",
                    Email = "test@example.com",
                    FirstName = "Test",
                    LastName = "User",
                    PasswordHash = "6TahL8vLTZpVm6nqqJsJpNPzwbkpM3+UTH2w4jR/ZdAwbp1GoiLeJYl79yUG7+UUeRWQN3q9cLIN29QCpfpHiw==", // Şifre: User123!
                    PasswordSalt = "5yVrYVY4HQv4x0P7Oky1TA7nDFwNwQQEH8h1ThYIZvw3rj7+tzdYOrdNLiYnMkdVMn0Y46UOD2Gau7Q6qPsWf6dRwCRYEDn42dkzYLp9/C3+aNlZCbPPwOFr19/L1gw9jX8JZHVT09Hs7JRWQf6oxQ6jlFILyTC20iiH0ICJIqE=",
                    IsActive = true,
                    EmailConfirmed = true
                };

                await context.Users.AddRangeAsync(adminUser, testUser);
                await context.SaveChangesAsync();
                Console.WriteLine("Kullanıcılar başarıyla eklendi.");

                // Kullanıcı-Rol ilişkilerini ekle
                var adminRoleEntity = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                var userRoleEntity = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

                if (adminRoleEntity != null && userRoleEntity != null)
                {
                    Console.WriteLine("Kullanıcı-Rol ilişkileri ekleniyor...");
                    var adminUserRole = new UserRole { UserId = adminUser.Id, RoleId = adminRoleEntity.Id };
                    var testUserRole = new UserRole { UserId = testUser.Id, RoleId = userRoleEntity.Id };

                    await context.UserRoles.AddRangeAsync(adminUserRole, testUserRole);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Kullanıcı-Rol ilişkileri başarıyla eklendi.");
                }
            }

            // Kategorileri ekle
            if (!await context.Categories.AnyAsync())
            {
                Console.WriteLine("Kategoriler ekleniyor...");
                var general = new Category { Name = "General" };
                var tech = new Category { Name = "Technology" };

                await context.Categories.AddRangeAsync(general, tech);
                await context.SaveChangesAsync();
                Console.WriteLine("Kategoriler başarıyla eklendi.");

                // Kullanıcı bilgisini edinme
                var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");

                if (adminUser != null)
                {
                    Console.WriteLine("Örnek notlar ekleniyor...");
                    var note1 = new Note
                    {
                        Title = "Hoş Geldiniz",
                        Content = "Bu bir örnek nottur.",
                        Category = general,
                        UserId = adminUser.Id,
                        IsPublished = true
                    };

                    var note2 = new Note
                    {
                        Title = "Teknoloji Trendleri 2025",
                        Content = "AI, ML, LLM ve ötesi.",
                        Category = tech,
                        UserId = adminUser.Id,
                        IsPublished = true
                    };

                    await context.Notes.AddRangeAsync(note1, note2);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Örnek notlar başarıyla eklendi.");
                }
            }

            Console.WriteLine("Seed işlemi başarıyla tamamlandı.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Seed işlemi sırasında hata oluştu: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            // Hatayı yakaladık ama uygulamanın çalışmasını engellemiyoruz
        }
    }
}