using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using StudentManagementConsole.Models;

namespace StudentManagementConsole.Services
{
    public class StudentRepository
    {
        private readonly string basePath = "students";

        public StudentRepository()
        {
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
        }

        public void AddStudent(Student student)
        {
            string dirPath = Path.Combine(basePath, student.Id.Substring(0, 2));
            Directory.CreateDirectory(dirPath);

            string filePath = Path.Combine(dirPath, $"{student.Id}.json");
            string jsonData = JsonSerializer.Serialize(student);
            File.WriteAllText(filePath, jsonData);
        }

        public Student? GetStudentById(string id)
        {
            string filePath = Path.Combine(basePath, id.Substring(0, 2), $"{id}.json");
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Student>(jsonData);
            }
            return null;
        }

        public void UpdateStudent(Student student)
        {
            DeleteStudent(student.Id);
            AddStudent(student);
        }

        public void DeleteStudent(string id)
        {
            string filePath = Path.Combine(basePath, id.Substring(0, 2), $"{id}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public IEnumerable<Student> SearchStudents(string? name = null, string? surname = null, int? age = null, string? curriculum = null)
{
    var students = new List<Student>();

    foreach (var file in Directory.GetFiles(basePath, "*.json", SearchOption.AllDirectories))
    {
        var student = JsonSerializer.Deserialize<Student>(File.ReadAllText(file));
        if (student != null &&
            (string.IsNullOrEmpty(name) || student.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) &&
            (string.IsNullOrEmpty(surname) || student.Surname.Contains(surname, StringComparison.OrdinalIgnoreCase)) &&
            (!age.HasValue || student.Age == age) &&
            (string.IsNullOrEmpty(curriculum) || student.Curriculum.Contains(curriculum, StringComparison.OrdinalIgnoreCase)))
        {
            students.Add(student);
        }
    }
    return students;
}
        public void ExportData(string filePath)
        {
            var allStudents = SearchStudents();
            File.WriteAllText(filePath, JsonSerializer.Serialize(allStudents));
        }

        public void ImportData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Import file not found.");
                return;
            }

            var students = JsonSerializer.Deserialize<List<Student>>(File.ReadAllText(filePath));
            if (students != null)
            {
                foreach (var student in students)
                {
                    AddStudent(student);
                }
            }
        }

        // New method to get statistics
        public (int TotalStudents, double AverageAge, int MinAge, int MaxAge, IEnumerable<(string Curriculum, int Count)> CurriculumDistribution) GetStatistics()
        {
            var students = GetAllStudents();

            // Total number of students
            int totalStudents = students.Count();

            // Average age
            double averageAge = students.Any() ? students.Average(s => s.Age) : 0;

            // Age range
            int minAge = students.Any() ? students.Min(s => s.Age) : 0;
            int maxAge = students.Any() ? students.Max(s => s.Age) : 0;

            // Curriculum distribution
            var curriculumDistribution = students
                .GroupBy(s => s.Curriculum)
                .Select(group => new { Curriculum = group.Key, Count = group.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            return (totalStudents, averageAge, minAge, maxAge, curriculumDistribution.Select(x => (x.Curriculum, x.Count)));
        }

        // Helper method to retrieve all students
        public IEnumerable<Student> GetAllStudents()
        {
            var students = new List<Student>();
            foreach (var file in Directory.GetFiles(basePath, "*.json", SearchOption.AllDirectories))
            {
                var student = JsonSerializer.Deserialize<Student>(File.ReadAllText(file));
                if (student != null)
                {
                    students.Add(student);
                }
            }
            return students;
        }
    }
}
