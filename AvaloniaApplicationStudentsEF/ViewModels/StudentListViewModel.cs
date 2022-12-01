using AvaloniaApplicationStudentsEF.Models;
using AvaloniaApplicationStudentsEF.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AvaloniaApplicationStudentsEF.ViewModels
{
    public class StudentListViewModel
    {
        private readonly DatabaseService databaseService;

        public StudentListViewModel(DatabaseService databaseService)
        {
            this.databaseService = databaseService;
        }

        public ObservableCollection<Student> Students { get; } = new();

        public async void LoadStudents()
        {
            List<Student> studentsFromDatabase = await databaseService.GetAllStudents();

            foreach (Student student in studentsFromDatabase)
            {
                Students.Add(student);
            }
        }

    }
}
