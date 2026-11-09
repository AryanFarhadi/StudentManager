using AKP.Data;
using AKP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AKP.Controllers
{
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Students or /
        public async Task<IActionResult> Index(string? search)
        {
            var totalCount = await _context.Students.CountAsync();
            var activeCount = await _context.Students.CountAsync(s => s.IsActive);
            var majorsCount = await _context.Majors.CountAsync();

            var query = _context.Students
                .Include(s => s.Major)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim();
                query = query.Where(s =>
                    s.FirstName.Contains(cleanSearch) ||
                    s.LastName.Contains(cleanSearch) ||
                    s.StudentNumber.Contains(cleanSearch) ||
                    s.NationalCode.Contains(cleanSearch) ||
                    (s.Major != null && s.Major.Title.Contains(cleanSearch)));
            }

            var students = await query
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var viewModel = new StudentIndexViewModel
            {
                Students = students,
                TotalStudents = totalCount,
                ActiveStudents = activeCount,
                MajorsCount = majorsCount,
                SearchTerm = search
            };

            return View(viewModel);
        }

        // GET: /Students/Create
        public async Task<IActionResult> Create()
        {
            await PopulateMajorsDropdown();

            var student = new Student
            {
                EnrollmentDate = DateTime.Today,
                IsActive = true
            };
            return View(student);
        }

        // POST: /Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            if (await _context.Students.AnyAsync(s => s.StudentNumber == student.StudentNumber))
            {
                ModelState.AddModelError(nameof(student.StudentNumber), "این شماره دانشجویی قبلاً در سیستم ثبت شده است.");
            }

            if (!await _context.Majors.AnyAsync(m => m.Id == student.MajorId))
            {
                ModelState.AddModelError(nameof(student.MajorId), "لطفاً یک رشته تحصیلی معتبر انتخاب نمایید.");
            }

            if (ModelState.IsValid)
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"دانشجو «{student.FirstName} {student.LastName}» با موفقیت ثبت گردید.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateMajorsDropdown(student.MajorId);
            return View(student);
        }

        // GET: /Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            await PopulateMajorsDropdown(student.MajorId);
            return View(student);
        }

        // POST: /Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id) return NotFound();

            if (await _context.Students.AnyAsync(s => s.StudentNumber == student.StudentNumber && s.Id != id))
            {
                ModelState.AddModelError(nameof(student.StudentNumber), "این شماره دانشجویی برای دانشجوی دیگری ثبت شده است.");
            }

            if (!await _context.Majors.AnyAsync(m => m.Id == student.MajorId))
            {
                ModelState.AddModelError(nameof(student.MajorId), "لطفاً یک رشته تحصیلی معتبر انتخاب نمایید.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"اطلاعات پرونده «{student.FirstName} {student.LastName}» با موفقیت بروزرسانی شد.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Students.AnyAsync(s => s.Id == student.Id))
                        return NotFound();
                    throw;
                }
            }

            await PopulateMajorsDropdown(student.MajorId);
            return View(student);
        }

        // GET: /Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Major)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // POST: /Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"پرونده دانشجو «{student.FirstName} {student.LastName}» با موفقیت از سیستم حذف گردید.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateMajorsDropdown(int? selectedId = null)
        {
            var majors = await _context.Majors
                .OrderBy(m => m.Title)
                .ToListAsync();

            ViewBag.Majors = new SelectList(majors, "Id", "Title", selectedId);
            ViewBag.HasMajors = majors.Count > 0;
        }
    }
}
