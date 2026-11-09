namespace AKP.Models
{
    public class StudentIndexViewModel
    {
        public List<Student> Students { get; set; } = new();

        public int TotalStudents { get; set; }

        public int ActiveStudents { get; set; }

        public int InactiveStudents => TotalStudents - ActiveStudents;

        public int MajorsCount { get; set; }

        public string? SearchTerm { get; set; }
    }
}
