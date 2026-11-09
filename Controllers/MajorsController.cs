using AKP.Data;
using AKP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AKP.Controllers
{
    public class MajorsController : Controller
    {
        private readonly AppDbContext _context;

        public MajorsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Majors
        public async Task<IActionResult> Index()
        {
            var majors = await _context.Majors
                .Include(m => m.Students)
                .OrderBy(m => m.Title)
                .ToListAsync();

            return View(majors);
        }

        // POST: /Majors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Major major)
        {
            if (string.IsNullOrWhiteSpace(major.Title))
            {
                ModelState.AddModelError(nameof(major.Title), "وارد کردن عنوان رشته تحصیلی الزامی است.");
            }
            else if (await _context.Majors.AnyAsync(m => m.Title.Trim().ToLower() == major.Title.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(major.Title), "این عنوان رشته تحصیلی قبلاً در سیستم ثبت شده است.");
            }

            if (ModelState.IsValid)
            {
                major.Title = major.Title.Trim();
                if (!string.IsNullOrWhiteSpace(major.Code))
                {
                    major.Code = major.Code.Trim();
                }

                _context.Majors.Add(major);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"رشته تحصیلی «{major.Title}» با موفقیت افزوده شد.";
                return RedirectToAction(nameof(Index));
            }

            // If invalid, reload list and return View
            var majors = await _context.Majors
                .Include(m => m.Students)
                .OrderBy(m => m.Title)
                .ToListAsync();

            ViewBag.ActiveTab = "create";
            return View(nameof(Index), majors);
        }

        // GET: /Majors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var major = await _context.Majors
                .Include(m => m.Students)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (major == null) return NotFound();

            return View(major);
        }

        // POST: /Majors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var major = await _context.Majors
                .Include(m => m.Students)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (major != null)
            {
                // Integrity check: Block delete if students exist in this major
                if (major.Students.Count > 0)
                {
                    TempData["ErrorMessage"] = $"امکان حذف رشته «{major.Title}» وجود ندارد زیرا تعداد {major.Students.Count} دانشجو در این رشته ثبت‌نام شده‌اند. ابتدا پرونده این دانشجویان را به رشته دیگری منتقل کنید.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Majors.Remove(major);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"رشته تحصیلی «{major.Title}» با موفقیت از سیستم حذف گردید.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
