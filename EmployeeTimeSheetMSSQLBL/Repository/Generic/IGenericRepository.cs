using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTimeSheetMSSQLBL.Repository.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Expression<Func<T, bool>> Criteria);
        T GetByID(object ID);
        void Insert(T Object);
        void Update(T Object);
        void Delete(object ID);
        void Save();
    }
}
