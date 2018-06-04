using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Api.Models;
using Data.Models;
using Contracts;

namespace Facade
{
    public class EmployeeMapper : IEmployeeMapper
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeMapper(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public bool SaveEmployee(Api.Models.Employee employee)
        {
            var result = new Data.Models.Employee();
            result.EmployeeName = employee.Name;
            result.EmployeeAge = employee.Age;

            return _repository.SaveEmployee(result);
        }
    }
}
