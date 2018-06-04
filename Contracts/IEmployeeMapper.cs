using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Api.Models;

namespace Contracts
{
    public interface IEmployeeMapper
    {
        bool SaveEmployee(Api.Models.Employee employee);
    }
}
