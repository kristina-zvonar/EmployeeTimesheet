using EmployeeTimeSheetMSSQL.Repository;
using EmployeeTimeSheetMSSQLBL.DAL;
using EmployeeTimeSheetMSSQLBL.Models;
using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using EmployeeTimeSheetMSSQLBL.Repository.Generic;
using EmployeeTimeSheetMSSQLBL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EmployeeTimeSheetMSSQLBL.BusinessLogic
{
    public class WorkItemLogic
    {
        private IGenericRepository<WorkItem> _repository;
        private IWorkItemRepository _workItemRepository;

        public WorkItemLogic()
        {
            _repository = new GenericRepository<WorkItem>();
            _workItemRepository = new WorkItemRepository();
        }

        public WorkItemLogic(IGenericRepository<WorkItem> Repository, IWorkItemRepository WorkItemRepository)
        {
            _repository = Repository;
            _workItemRepository = WorkItemRepository;
        }

        public List<WorkItemViewModel> GetAll(Expression<Func<WorkItem, bool>> Criteria, int Skip, int Take, out int TotalCount)
        {
            var workItemsDB = _repository.GetAll(Criteria);
            TotalCount = workItemsDB.Count();

            workItemsDB = workItemsDB.Skip(Skip).Take(Take);

            var result = workItemsDB.Select(x => new WorkItemViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Estimate = x.Estimate,
                TimeLogged = GetSumHours(x.ID, x.TypeID),
                UserID = x.AspNetUser.UserName,
                Creator = x.AspNetUser.Employees.FirstOrDefault().FirstName + " " + x.AspNetUser.Employees.FirstOrDefault().LastName,
                Supervisor = x.Employee.FirstName + " " + x.Employee.LastName,
                Type = x.WorkItemType.Name,
                Status = x.WorkItemStatu.Name
            }).ToList();

            return result;
        }

        public decimal GetSumHours(int WorkItemID, int TypeID)
        {
            decimal hours = 0;
            var workItem = _repository.GetByID(WorkItemID);
            if(workItem != null)
            {
                hours = workItem.TimeLogEntries.Sum(x => x.Hours);
                if (TypeID == (int)WorkItemTypeUtil.Project)
                {
                    var childWorkItems = _repository.GetAll(x => x.ParentID == WorkItemID);
                    if (childWorkItems.Any())
                    {
                        hours += childWorkItems.Sum(x => x.TimeLogEntries.Sum(y => y.Hours));
                    }
                }                 
            }

            return hours;
        }

        public WorkItemDropdowns GetDropdowns(Expression<Func<WorkItem, bool>> Criteria)
        {
            var dropdowns = _workItemRepository.GetDropdowns(Criteria);
            return dropdowns;
        }

        public void CreateWorkItem(WorkItemViewModel Model)
        {
            var workItem = CreateWorkItemObject(Model);
            _repository.Insert(workItem);
            _repository.Save();
        }

        public WorkItem CreateWorkItemObject(WorkItemViewModel Model)
        {
            var workItem = new WorkItem
            {
                ID = Model.ID,
                Name = Model.Name,
                Estimate = Model.Estimate,
                UserID = Model.UserID,
                SupervisorID = Model.SupervisorID,
                TypeID = Model.TypeID,
                StatusID = Model.StatusID,
                ParentID = Model.ParentID
            };

            return workItem;
        }

        public WorkItemViewModel CreateWorkItemViewModel(WorkItem Model)
        {
            var workItem = new WorkItemViewModel
            {
                ID = Model.ID,
                Name = Model.Name,
                Estimate = Model.Estimate,
                UserID = Model.UserID,
                SupervisorID = Model.SupervisorID,
                TypeID = Model.TypeID,
                StatusID = Model.StatusID,
                ParentID = Model.ParentID
            };

            return workItem;
        }

        public WorkItemViewModel GetModel(int ID)
        {
            WorkItem modelDB = _repository.GetByID(ID);
            WorkItemViewModel model = CreateWorkItemViewModel(modelDB);

            return model;
        }

        public void UpdateModel(WorkItemViewModel Model)
        {
            var workItem = CreateWorkItemObject(Model);
            _repository.Update(workItem);
            _repository.Save();
        }

        public bool HasDataAttached(int WorkItemID)
        {
            bool hasDataAttached = _workItemRepository.HasDataAttached(WorkItemID);
            return hasDataAttached;
        }

        public bool IsOwnWorkItem(int SupervisorID, int WorkItemID)
        {
            var workItem = _repository.GetByID(WorkItemID);
            bool isOwnWorkItem = workItem.SupervisorID == SupervisorID;
            return isOwnWorkItem;
        }

        public void Delete(int WorkItemID)
        {
            _repository.Delete(WorkItemID);
            _repository.Save();
        }

        public bool IsNameUnique(string Name)
        {
            bool isNameUnique = _workItemRepository.IsNameUnique(Name);
            return isNameUnique;
        }

        public void UpdateStatus(int[] WorkItemIDs, int Status)
        {
            foreach (int workItemID in WorkItemIDs)
            {
                var workItem = _repository.GetByID(workItemID);
                if (workItem != null)
                {
                    workItem.StatusID = Status;
                }
            }

            _repository.Save();
        }
    }
}
