using EmployeeTimeSheetMSSQL.Repository;
using EmployeeTimeSheetMSSQLBL.DAL;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns.Generic;
using EmployeeTimeSheetMSSQLBL.Models.Helpers;
using EmployeeTimeSheetMSSQLBL.Models.Results;
using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using EmployeeTimeSheetMSSQLBL.Repository.Generic;
using EmployeeTimeSheetMSSQLBL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeSheetMSSQLBL.BusinessLogic
{
    public class TimeLogEntryLogic
    {
        private IGenericRepository<TimeLogEntry> _repository;
        private ITimeLogEntryRepository _timeLogRepository;
        private IGenericRepository<WorkItem> _workItemRepository;

        public TimeLogEntryLogic()
        {
            _repository = new GenericRepository<TimeLogEntry>();
            _timeLogRepository = new TimeLogRepository();
            _workItemRepository = new GenericRepository<WorkItem>();
        }

        public TimeLogEntryLogic(IGenericRepository<TimeLogEntry> Repository, ITimeLogEntryRepository TimeLogRepository, IGenericRepository<WorkItem> WorkItemRepository)
        {
            _repository = Repository;
            _timeLogRepository = TimeLogRepository;
            _workItemRepository = WorkItemRepository;
        }

        public TimeLogDropdowns GetDropdowns(int SupervisorID)
        {
            var workItems = _workItemRepository.GetAll(x => x.SupervisorID == SupervisorID && x.StatusID == (int)WorkItemStatusUtil.Active).Select(x => new GenericDropdown
            {
                ID = x.ID,
                Text = x.WorkItemType.Name + " " + x.Name
            }).ToList();

            var dropdowns = new TimeLogDropdowns
            {
                WorkItems = workItems
            };

            return dropdowns;
        }

        public List<TimeLogEntryViewModel> GetAll(Expression<Func<TimeLogEntry, bool>> Criteria, int Skip, int Take, out int TotalCount)
        {
            var timeLogEntriesDB = _repository.GetAll(Criteria).AsEnumerable();
            TotalCount = timeLogEntriesDB.Count();

            timeLogEntriesDB = timeLogEntriesDB.Skip(Skip).Take(Take);

            var result = timeLogEntriesDB.Select(x => new TimeLogEntryViewModel
            {
                ID = x.ID,
                EntryDate = x.EntryDate.ToString("yyyy-MM-dd"),
                EmployeeID = x.EmployeeID,
                Employee = x.Employee.FirstName + " " + x.Employee.LastName,
                WorkItemID = x.WorkItemID,
                WorkItem = x.WorkItemID + " - " + x.WorkItem.Name,
                IsWorkItemActive = x.WorkItem.StatusID == (int)WorkItemStatusUtil.Active,
                Hours = x.Hours
            }).OrderByDescending(x => x.EntryDate).ToList();

            return result;
        }

        public void CreateTimeLogEntry(TimeLogEntryViewModel Model)
        {
            var timeLogEntry = CreateTimeLogEntryObject(Model);
            _repository.Insert(timeLogEntry);
            _repository.Save();
        }

        public TimeLogEntryViewModel GetModel(int TimeLogID)
        {
            TimeLogEntry modelDB = _repository.GetByID(TimeLogID);
            TimeLogEntryViewModel model = CreateTimeLogEntryViewModel(modelDB);
            return model;
        }

        public void UpdateModel(TimeLogEntryViewModel Model)
        {
            var timeLogEntry = CreateTimeLogEntryObject(Model);
            _repository.Update(timeLogEntry);
            _repository.Save();
        }

        public TimeLogEntry CreateTimeLogEntryObject(TimeLogEntryViewModel Model)
        {
            var timeLogEntry = new TimeLogEntry
            {
                ID = Model.ID,
                EntryDate = DateTime.ParseExact(Model.EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Hours = Model.Hours,
                EmployeeID = Model.EmployeeID,
                WorkItemID = Model.WorkItemID
            };

            return timeLogEntry;
        }

        public TimeLogEntryViewModel CreateTimeLogEntryViewModel(TimeLogEntry Model)
        {
            var timeLogEntry = new TimeLogEntryViewModel
            {
                ID = Model.ID,
                EntryDate = Model.EntryDate.ToString("dd/MM/yyyy"),
                Hours = Model.Hours,
                EmployeeID = Model.EmployeeID,
                WorkItemID = Model.WorkItemID
            };

            return timeLogEntry;
        }

        public List<SummaryObject> GetAnalyticsByWorkItem(string TimeSpan, int EmployeeID)
        {
            List<SummaryObject> results = new List<SummaryObject>();

            DateTime startOfPeriod;
            if(TimeSpan == TimeSpanUtil.DAY)
            {
                startOfPeriod = DateTime.Now.AddHours(-24);                
            } 
            else if(TimeSpan == TimeSpanUtil.WEEK)
            {
                startOfPeriod = DateTime.Now.AddDays(-7);
            } 
            else
            {
                startOfPeriod = DateTime.Now.AddMonths(-1);                
            }
            
            var workItemGroups = _repository.GetAll(x => x.EntryDate >= startOfPeriod && (EmployeeID != 0 ? x.WorkItem.SupervisorID == EmployeeID : true)).GroupBy(x => x.WorkItem.Name);
            results = workItemGroups.Select(x => new SummaryObject
            {
                X = x.Key,
                Y = x.Sum(y => y.Hours)
            }).ToList();

            return results;
        }

        public List<SummaryObject> GetAnalyticsByEmployeeAndWorkItem(string TimeSpan, int EmployeeID)
        {
            List<SummaryObject> results = new List<SummaryObject>();

            DateTime startOfPeriod;
            if (TimeSpan == TimeSpanUtil.DAY)
            {
                startOfPeriod = DateTime.Now.AddHours(-24);                
            }
            else if (TimeSpan == TimeSpanUtil.WEEK)
            {
                startOfPeriod = DateTime.Now.AddDays(-7);                
            }
            else
            {
                startOfPeriod = DateTime.Now.AddMonths(-1);               
            }

            var workItemGroups = _repository.GetAll(x => x.EntryDate >= startOfPeriod && (EmployeeID != 0 ? x.WorkItem.SupervisorID == EmployeeID : true)).
                    GroupBy(x => new { x.WorkItemID, x.EmployeeID }, (key, group) => new
                    {
                        Key1 = key.WorkItemID,
                        Key2 = key.EmployeeID,
                        Result = group.ToList()
                    });

            results = workItemGroups.Select(x => new SummaryObject
            {
                X = x.Result.FirstOrDefault().WorkItem.Name + " by " + x.Result.FirstOrDefault().Employee.FirstName + " " + x.Result.FirstOrDefault().Employee.LastName,
                Y = x.Result.Sum(y => y.Hours)
            }).ToList();

            return results;
        }
    }
}
