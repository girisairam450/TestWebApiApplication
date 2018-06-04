using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;

namespace Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        public bool SaveEmployee(Data.Models.Employee employee)
        {
            // Interact with DAL code and save details in Employee table.



            return true;
        }
    }
}
