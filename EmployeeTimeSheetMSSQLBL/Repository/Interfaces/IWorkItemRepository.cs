using EmployeeTimeSheetMSSQLBL.DAL;
using EmployeeTimeSheetMSSQLBL.Models;
using System;
using System.Linq.Expressions;

namespace EmployeeTimeSheetMSSQLBL.Repository.Interfaces
{
    public interface IWorkItemRepository
    {
        Employee GetEmployee(string Username);
        WorkItemDropdowns GetDropdowns(Expression<Func<WorkItem, bool>> Criteria);
        bool HasDataAttached(int ID);
        bool IsNameUnique(string Name);
    }
}
