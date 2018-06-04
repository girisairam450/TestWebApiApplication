using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contracts
{
    public interface IStudentRepository
    {
        object RetrieveStudents(string StudentId);
    }
}
