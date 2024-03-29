using CodeChallenge.Models;
using System;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation Add(Compensation compensation);
        Compensation GetByEmployeeId(String id);
    }
}
