using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AKP.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "شماره دانشجویی")]
        [Required(ErrorMessage = "وارد کردن شماره دانشجویی الزامی است.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "شماره دانشجویی باید بین ۵ تا ۲۰ رقم باشد.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "شماره دانشجویی فقط باید شامل اعداد باشد.")]
        public string StudentNumber { get; set; } = string.Empty;

        [Display(Name = "نام")]
        [Required(ErrorMessage = "وارد کردن نام الزامی است.")]
        [StringLength(50, ErrorMessage = "نام نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "وارد کردن نام خانوادگی الزامی است.")]
        [StringLength(50, ErrorMessage = "نام خانوادگی نمی‌تواند بیشتر از ۵۰ کاراکتر باشد.")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "وارد کردن کد ملی الزامی است.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "کد ملی باید دقیقاً ۱۰ رقم باشد.")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "کد ملی باید یک عدد ۱۰ رقمی معتبر باشد.")]
        public string NationalCode { get; set; } = string.Empty;

        [Display(Name = "رشته تحصیلی")]
        [Required(ErrorMessage = "انتخاب رشته تحصیلی الزامی است.")]
        public int MajorId { get; set; }

        [ForeignKey(nameof(MajorId))]
        public Major? Major { get; set; }

        [Display(Name = "تاریخ ثبت‌نام")]
        [Required(ErrorMessage = "وارد کردن تاریخ ثبت‌نام الزامی است.")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Display(Name = "وضعیت تحصیلی")]
        public bool IsActive { get; set; } = true;
    }
}
