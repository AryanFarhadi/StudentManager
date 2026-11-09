using System.ComponentModel.DataAnnotations;

namespace AKP.Models
{
    public class Major
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "عنوان رشته تحصیلی")]
        [Required(ErrorMessage = "وارد کردن عنوان رشته تحصیلی الزامی است.")]
        [StringLength(100, ErrorMessage = "عنوان رشته تحصیلی نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد.")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "کد رشته")]
        [StringLength(20, ErrorMessage = "کد رشته نمی‌تواند بیشتر از ۲۰ کاراکتر باشد.")]
        public string? Code { get; set; }

        // Navigation property for related students
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
