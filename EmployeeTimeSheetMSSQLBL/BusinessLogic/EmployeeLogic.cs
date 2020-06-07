using EmployeeTimeSheetMSSQLBL.DAL;
using EmployeeTimeSheetMSSQLBL.Models.Dropdowns;
using EmployeeTimeSheetMSSQLBL.Models.ViewModels;
using EmployeeTimeSheetMSSQLBL.Repository;
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
    public class EmployeeLogic
    {
        private IGenericRepository<Employee> _repository;
        private IGenericRepository<AuthToken> _tokenRepository;
        private IEmployeeRepository _employeeRepository;        

        public EmployeeLogic()
        {
            _repository = new GenericRepository<Employee>();
            _tokenRepository = new GenericRepository<AuthToken>();
            _employeeRepository = new EmployeeRepository();
        }

        public EmployeeLogic(IGenericRepository<Employee> Repository, IGenericRepository<AuthToken> TokenRepository, IEmployeeRepository EmployeeRepository)
        {
            _repository = Repository;
            _tokenRepository = TokenRepository;
            _employeeRepository = EmployeeRepository;
        }

        public List<EmployeeViewModel> GetAll(Expression<Func<Employee, bool>> Criteria, out int TotalCount, int Skip, int Take)
        {
            var employeesDB = _repository.GetAll(Criteria);
            TotalCount = employeesDB.Count();            
            employeesDB = employeesDB.Skip(Skip).Take(Take);
            var result = employeesDB.Select(x => new EmployeeViewModel
            {
                ID = x.ID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Username = x.AspNetUser?.UserName,
                RankID = x.RankID,
                Supervisor = x.Employee2?.FirstName + " " + x.Employee2?.LastName,
                Active = x.Active
            }).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();
            return result;
        }

        public EmployeeDropdowns GetDropdowns()
        {
            var dropdowns = _employeeRepository.GetDropdowns();
            return dropdowns;
        }

        public EmployeeViewModel GetEmployeeModel(int ID)
        {
            Employee modelDB = _repository.GetByID(ID);
            EmployeeViewModel model = CreateEmployeeViewModel(modelDB);

            var dropdowns = GetDropdowns();
            model.Supervisors = new SelectList(dropdowns.Supervisors, "ID", "Text");
            model.Users = new SelectList(dropdowns.Users, "ID", "Text");
            model.Ranks = new SelectList(dropdowns.Ranks, "ID", "Text");

            return model;
        }

        public int GetEmployeeID(string Username)
        {
            var employee = _employeeRepository.GetEmployee(Username);
            if(employee != null)
            {
                return employee.ID;
            }

            return 0;
        }

        public int GetSupervisorID(string Username)
        {
            var employee = _employeeRepository.GetEmployee(Username);
            if (employee != null)
            {
                return employee.SupervisorID ?? 0;
            }

            return 0;
        }

        public string GetUserID(int ID)
        {
            var employee = _repository.GetByID(ID);
            if (employee != null)
            {
                return employee.UserID;
            }

            return "";
        }

        public void Delete(int ID)
        {
            _repository.Delete(ID);
            _repository.Save();
        }

        public void DeleteTokens(string UserID)
        {
            var tokens = _tokenRepository.GetAll(x => x.UserID == UserID);
            
            foreach(var token in tokens)
            {
                _tokenRepository.Delete(token.ID);
            }
            _tokenRepository.Save();
        }

        public void CreateEmployee(EmployeeViewModel Model)
        {
            var employee = CreateEmployeeObject(Model);
            _repository.Insert(employee);
            _repository.Save();
        }

        public Employee CreateEmployeeObject(EmployeeViewModel Model)
        {
            var employee = new Employee
            {
                ID = Model.ID,
                FirstName = Model.FirstName,
                LastName = Model.LastName,
                RankID = Model.RankID,
                SupervisorID = Model.SupervisorID,
                UserID = Model.UserID,
                Email = Model.Email,
                Active = true
            };

            return employee;
        }

        public string GetToken(int EmployeeID)
        {
            var employee = _repository.GetByID(EmployeeID);
            if (employee != null)
            {
                var token = employee.AspNetUser.AuthTokens.OrderByDescending(x => x.expires).FirstOrDefault();
                if (token != null)
                {
                    return token.access_token;
                }                
            }
            return "";
        }

        public void SaveToken(AuthToken Token)
        {
            _tokenRepository.Insert(Token);
            _tokenRepository.Save();
        }

        public void MarkTokensAsInactive(string UserID)
        {
            var tokens = _tokenRepository.GetAll(x => x.UserID == UserID);
            var lastToken = tokens.OrderByDescending(x => x.ID).FirstOrDefault();

            foreach(var token in tokens)
            {
                if(token.ID != lastToken.ID)
                {
                    token.Active = false;
                }
            }

            _tokenRepository.Save();
        }

        public void UpdateModel(Employee Model)
        {
            _repository.Update(Model);
            _repository.Save();
        }

        public bool HasDataAttached(int EmployeeID)
        {
            bool hasDataAttached = _employeeRepository.HasDataAttached(EmployeeID);
            return hasDataAttached;
        }

        public void SoftDeleteEmployee(int ID)
        {
            var employee = _repository.GetByID(ID);
            if (employee != null)
            {
                employee.Active = false;
                _repository.Save();
            }
        }

        public EmployeeViewModel CreateEmployeeViewModel(Employee Model)
        {
            var employee = new EmployeeViewModel
            {
                ID = Model.ID,
                FirstName = Model.FirstName,
                LastName = Model.LastName,
                RankID = Model.RankID,
                SupervisorID = Model.SupervisorID,
                UserID = Model.UserID,
                Email = Model.Email,
                Active = true
            };

            return employee;
        }

    }
}
