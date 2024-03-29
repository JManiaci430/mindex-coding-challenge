﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructure(Employee employee)
        {
            if (employee.DirectReports == null || employee.DirectReports.Count == 0)
                return new ReportingStructure()
                {
                    Employee = employee,
                    NumberOfReports = 0
                };

            var numOfReports = employee.DirectReports.Count;
            foreach (var report in employee.DirectReports)
            {
                if (report.DirectReports != null)
                    numOfReports += report.DirectReports.Count;
            }

            return new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = numOfReports
            };
        }

        public Compensation AddCompensation(Employee employee, Compensation compensation)
        {
            if (employee != null)
            {
                _employeeRepository.Remove(employee);
                if (compensation != null)
                {
                    employee.Salary = compensation.Salary;
                    employee.EffectiveDate = compensation.EffectiveDate;
                }

                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return compensation;
        }

        public Compensation GetCompensation(String id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var employee = _employeeRepository.GetById(id);
                return new Compensation()
                {
                    EmployeeId = employee.EmployeeId,
                    Salary = employee.Salary,
                    EffectiveDate = employee.EffectiveDate
                };
            }

            return null;
        }
    }
}
