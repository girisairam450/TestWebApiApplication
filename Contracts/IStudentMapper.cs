using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contracts
{
    public interface IStudentMapper
    {
        object RetrieveStudents(Api.Models.Student Student);
    }
}
