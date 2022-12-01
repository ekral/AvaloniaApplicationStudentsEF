using AvaloniaApplicationStudentsEF.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AvaloniaApplicationStudentsEF.Services
{
    public class DatabaseService
    {
        public async Task<List<Student>> GetAllStudents()
        {
            await using SchoolContext schoolContext = new SchoolContext();

            List<Student> students = await schoolContext.Students.ToListAsync();

            return students;
        }
    }
}
