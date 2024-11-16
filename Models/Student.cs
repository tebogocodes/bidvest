namespace StudentManagementConsole.Models
{
    public class Student
    {
        public string Id { get; set; } = string.Empty; // 7-digit unique ID
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Curriculum { get; set; } = string.Empty;
    }
}
