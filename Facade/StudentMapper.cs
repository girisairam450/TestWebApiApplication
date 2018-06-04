using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;

namespace Facade
{
    public class StudentMapper : IStudentMapper
    {
        private readonly IStudentRepository _repository;
        public StudentMapper(IStudentRepository repository)
        {
            _repository = repository;
        }

        public object RetrieveStudents(Api.Models.Student Student)
        {
            string studentId = Convert.ToString(Student.StudentId);


            return _repository.RetrieveStudents(studentId);
        }
    }
}
