using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        bool SaveEmployee(Data.Models.Employee employee);
    }
}
