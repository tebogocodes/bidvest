# Student Management Console

A console-based application for managing student records, developed in C# using the `Spectre.Console` library for a rich, interactive user experience.

## Features

- **Add Student:** Add a new student with details like ID, name, surname, age, and curriculum.
- **Edit Student:** Modify an existing student's information using their unique ID.
- **Delete Student:** Remove a student's record by their ID, with confirmation.
- **Search Students:** Find students based on various criteria like name, surname, age, or curriculum.
- **Export Data:** (Placeholder) Export student data to a file format (not yet implemented).
- **Import Data:** (Placeholder) Import student data from a file (not yet implemented).
- **View Statistics:** (Placeholder) View statistics about students like average age, curriculum distribution, etc. (not yet implemented).
  
The system uses a simple `StudentRepository` to handle student data in memory, but this can be extended to support database integration.

## Prerequisites

Ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8 or above)
- [Spectre.Console](https://spectreconsole.net/) (for rich console rendering)

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/your-username/student-management-console.git
2. cd into bidvest

   ```bash
   cd bivest
1. run application

   ```bash
   dotnet run 


Overall Design

The program is well-organized, leveraging Spectre.Console for a modern CLI interface. Improving error handling and extracting reusable validation logic would further enhance maintainability.
