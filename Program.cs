using Spectre.Console;
using StudentManagementConsole.Models;
using StudentManagementConsole.Services;
using System;
using System.Linq;

namespace StudentManagementConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var repository = new StudentRepository();
            string? action = null;
            string? studentId = null;

            // Parse arguments (e.g., --action=edit --id=1234567)
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--action" && i + 1 < args.Length)
                {
                    action = args[i + 1];
                }
                else if (args[i] == "--id" && i + 1 < args.Length)
                {
                    studentId = args[i + 1];
                }
            }

            if (!string.IsNullOrEmpty(action))
            {
                // Perform the action specified by the user
                PerformAction(action, studentId, repository);
                return; // Exit after performing the action
            }
            else
            {
                // If no action is provided, show the interactive menu
                ShowInteractiveMenu(repository);
            }
        }

        static void PerformAction(string action, string? studentId, StudentRepository repository)
        {
            switch (action.ToLower())
            {
                case "add":
                    AddStudent(repository);
                    break;
                case "edit":
                    if (string.IsNullOrEmpty(studentId))
                    {
                        AnsiConsole.MarkupLine("[red]Student ID is required to edit.[/]");
                        return;
                    }
                    EditStudent(repository, studentId);
                    break;
                case "delete":
                    if (string.IsNullOrEmpty(studentId))
                    {
                        AnsiConsole.MarkupLine("[red]Student ID is required to delete.[/]");
                        return;
                    }
                    DeleteStudent(repository, studentId);
                    break;
                case "show":
                    SearchStudents(repository);
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Invalid action! Use 'add', 'edit', 'delete', or 'show'.[/]");
                    break;
            }
        }

        static void ShowInteractiveMenu(StudentRepository repository)
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.Write(
                    new FigletText("Student Management")
                        .Color(Color.Green)
                        .Justify(Justify.Left));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]What would you like to do?[/]")
                        .PageSize(10)
                        .AddChoices(new[] {
                            "Add Student", "Edit Student", "Delete Student",
                            "Search Students", "Export Data", "Import Data",
                            "View Statistics", "Exit"
                        }));

                switch (choice)
                {
                    case "Add Student":
                        AddStudent(repository);
                        break;
                    case "Edit Student":
                        string studentId = AnsiConsole.Ask<string>("[cyan]Enter student ID to edit:[/]");
                        EditStudent(repository, studentId);
                        break;
                    case "Delete Student":
                        studentId = AnsiConsole.Ask<string>("[cyan]Enter student ID to delete:[/]");
                        DeleteStudent(repository, studentId);
                        break;
                    case "Search Students":
                        SearchStudents(repository);
                        break;
                    case "Export Data":
                        ExportData(repository);
                        break;
                    case "Import Data":
                        ImportData(repository);
                        break;
                    case "View Statistics":
                        ViewStatistics(repository);
                        break;
                    case "Exit":
                        AnsiConsole.MarkupLine("[bold yellow]Goodbye![/]");
                        return;
                }

                AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
                Console.ReadKey();
            }
        }

        static void AddStudent(StudentRepository repository)
        {
            var id = AnsiConsole.Ask<string>("[cyan]Enter student ID (7 digits):[/]");
            if (id.Length != 7 || !int.TryParse(id, out _)) 
            {
                AnsiConsole.MarkupLine("[red]Student ID must be a 7-digit number.[/]");
                return;
            }

            var name = AnsiConsole.Ask<string>("[cyan]Enter name:[/]");
            var surname = AnsiConsole.Ask<string>("[cyan]Enter surname:[/]");
            var age = AnsiConsole.Ask<int>("[cyan]Enter age:[/]");
            if (age < 16 || age > 120)
            {
                AnsiConsole.MarkupLine("[red]Age must be between 16 and 120.[/]");
                return;
            }

            var curriculum = AnsiConsole.Ask<string>("[cyan]Enter curriculum:[/]");

            var student = new Student { Id = id, Name = name, Surname = surname, Age = age, Curriculum = curriculum };
            repository.AddStudent(student);
            AnsiConsole.MarkupLine("[green]Student added successfully![/]");
        }

        static void EditStudent(StudentRepository repository, string studentId)
        {
            var student = repository.GetStudentById(studentId);

            if (student == null)
            {
                AnsiConsole.MarkupLine("[red]Student not found![/]");
                return;
            }

            var name = AnsiConsole.Ask<string>("[cyan]Enter name (Leave empty to keep existing value):[/]", student.Name);
            var surname = AnsiConsole.Ask<string>("[cyan]Enter surname (Leave empty to keep existing value):[/]", student.Surname);
            var age = AnsiConsole.Ask<int?>("[cyan]Enter age (Leave empty to keep existing value):[/]", student.Age);
            var curriculum = AnsiConsole.Ask<string>("[cyan]Enter curriculum (Leave empty to keep existing value):[/]", student.Curriculum);

            student.Name = string.IsNullOrEmpty(name) ? student.Name : name;
            student.Surname = string.IsNullOrEmpty(surname) ? student.Surname : surname;
            student.Age = age ?? student.Age;
            student.Curriculum = string.IsNullOrEmpty(curriculum) ? student.Curriculum : curriculum;

            repository.UpdateStudent(student);
            AnsiConsole.MarkupLine("[green]Student information updated successfully![/]");
        }

        static void DeleteStudent(StudentRepository repository, string studentId)
        {
            if (AnsiConsole.Confirm("[yellow]Are you sure you want to delete this student?[/]"))
            {
                repository.DeleteStudent(studentId);
                AnsiConsole.MarkupLine("[green]Student deleted successfully![/]");
            }
        }

        static void SearchStudents(StudentRepository repository)
        {
            var query = AnsiConsole.Ask<string>("[cyan]Enter search criteria (e.g., name=sizwe, age=20||Type ALL to return all students):[/]");
            
            if (string.IsNullOrEmpty(query))
            {
                var allStudents = repository.GetAllStudents();
                DisplayStudentsInTable(allStudents);
                return;
            }

            var criteria = ParseSearchQuery(query);
            var results = repository.SearchStudents(criteria.Name, criteria.Surname, criteria.Age, criteria.Curriculum);

            if (results.Any())
            {
                DisplayStudentsInTable(results);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No students found matching the search criteria.[/]");
            }
        }

        private static void DisplayStudentsInTable(IEnumerable<Student> students)
        {
            var table = new Table()
                .AddColumn("ID")
                .AddColumn("Name")
                .AddColumn("Surname")
                .AddColumn("Age")
                .AddColumn("Curriculum")
                .Border(TableBorder.Rounded)
                .Centered();

            foreach (var student in students)
            {
                table.AddRow(student.Id, student.Name, student.Surname, student.Age.ToString(), student.Curriculum);
            }

            AnsiConsole.Write(table);
        }

        private static (string? Name, string? Surname, int? Age, string? Curriculum) ParseSearchQuery(string query)
        {
            string? name = null, surname = null, curriculum = null;
            int? age = null;

            var criteria = query.Split(',');

            foreach (var criterion in criteria)
            {
                var parts = criterion.Split('=');
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim().ToLower();
                    var value = parts[1].Trim();

                    switch (key)
                    {
                        case "name":
                            name = value;
                            break;
                        case "surname":
                            surname = value;
                            break;
                        case "age":
                            if (int.TryParse(value, out var parsedAge))
                            {
                                age = parsedAge;
                            }
                            break;
                        case "curriculum":
                            curriculum = value;
                            break;
                    }
                }
            }

            return (name, surname, age, curriculum);
        }

        static void ViewStatistics(StudentRepository repository)
        {
            var totalStudents = repository.GetAllStudents().Count();
            AnsiConsole.MarkupLine($"[yellow]Total students: {totalStudents}[/]");

            var averageAge = repository.GetAllStudents().Average(s => s.Age);
            AnsiConsole.MarkupLine($"[yellow]Average age: {averageAge:F2}[/]");

            var curriculumDistribution = repository.GetAllStudents()
                .GroupBy(s => s.Curriculum)
                .Select(g => new { Curriculum = g.Key, Count = g.Count() });

            foreach (var item in curriculumDistribution)
            {
                AnsiConsole.MarkupLine($"[yellow]{item.Curriculum}: {item.Count}[/]");
            }
        }

        static void ExportData(StudentRepository repository)
        {
            var filePath = AnsiConsole.Ask<string>("[cyan]Enter export file path:[/]");
            repository.ExportData(filePath);
            AnsiConsole.MarkupLine("[green]Data exported successfully![/]");
        }

        static void ImportData(StudentRepository repository)
        {
            var filePath = AnsiConsole.Ask<string>("[cyan]Enter import file path:[/]");
            repository.ImportData(filePath);
            AnsiConsole.MarkupLine("[green]Data imported successfully![/]");
        }
    }
}
