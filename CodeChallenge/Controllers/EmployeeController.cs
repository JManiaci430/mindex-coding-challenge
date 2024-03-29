using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        // REST Endpoint for retrieving Employee Structure
        [HttpGet("structure/{id}", Name = "getStructureById")]
        public IActionResult GetStructureById(String id)
        {
            _logger.LogDebug($"Received employee structure get request for '{id}'");

            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound();

            var structure = _employeeService.GetReportingStructure(employee);

            return Ok(structure);
        }

        // REST Endpoint for adding Compensation information for Employee
        [HttpPost("compensation")]
        public IActionResult AddCompensation([FromBody]Compensation compensation)
        {
            _logger.LogDebug($"Received add compensation post request for '{compensation.EmployeeId}'");

            var employee = _employeeService.GetById(compensation.EmployeeId);
            if (employee == null)
                return NotFound();

            _employeeService.AddCompensation(employee, compensation);

            return CreatedAtRoute("getCompensationById", new { id = compensation.EmployeeId }, compensation);
        }

        [HttpGet("compensation/{id}", Name = "getCompensationById")]
        public IActionResult GetCompensationById(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _employeeService.GetCompensation(id);
            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
    }
}
