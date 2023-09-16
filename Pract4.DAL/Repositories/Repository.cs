using System;
using System.Collections.Generic;
using System.Linq;
using Pract4.DAL.Entities;

namespace Pract4.DAL.Repositories
{
    public class Repository
    {
        private readonly StudentsContext _context;

        public Repository()
        {
            _context = new StudentsContext();
        }

        public List<Student> GetAllStudents()
        {
            return _context.Students.ToList();
        }

        public List<StudentCard> GetStudentCards()
        {
            return _context.StudentCards.ToList();
        }
    }
}
