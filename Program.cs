using AKP.Data;
using AKP.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services with Views
builder.Services.AddControllersWithViews();

// Configure SQLite Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Ensure SQLite database is created and seed initial Majors & Students
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        // 1. Seed Majors if empty
        if (!context.Majors.Any())
        {
            var majorCe = new Major { Title = "مهندسی کامپیوتر", Code = "CE-101" };
            var majorEe = new Major { Title = "مهندسی برق", Code = "EE-102" };
            var majorAi = new Major { Title = "هوش مصنوعی و رباتیک", Code = "AI-103" };
            var majorDs = new Major { Title = "علوم داده", Code = "DS-104" };

            context.Majors.AddRange(majorCe, majorEe, majorAi, majorDs);
            context.SaveChanges();

            // 2. Seed Sample Students with foreign keys
            if (!context.Students.Any())
            {
                context.Students.AddRange(
                    new Student
                    {
                        StudentNumber = "40112345",
                        FirstName = "سارا",
                        LastName = "رضایی",
                        NationalCode = "0012345678",
                        MajorId = majorCe.Id,
                        EnrollmentDate = new DateTime(2023, 9, 23),
                        IsActive = true
                    },
                    new Student
                    {
                        StudentNumber = "40054321",
                        FirstName = "علی",
                        LastName = "محمدی",
                        NationalCode = "0087654321",
                        MajorId = majorEe.Id,
                        EnrollmentDate = new DateTime(2022, 9, 23),
                        IsActive = true
                    },
                    new Student
                    {
                        StudentNumber = "40298765",
                        FirstName = "زهرا",
                        LastName = "حسینی",
                        NationalCode = "0055443322",
                        MajorId = majorAi.Id,
                        EnrollmentDate = new DateTime(2024, 9, 22),
                        IsActive = false
                    }
                );
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "خطایی در مقداردهی اولیه پایگاه داده رخ داد.");
    }
}

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Students}/{action=Index}/{id?}");

app.Run();
