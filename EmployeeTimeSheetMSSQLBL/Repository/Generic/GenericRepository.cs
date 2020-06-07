using EmployeeTimeSheetMSSQLBL.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace EmployeeTimeSheetMSSQLBL.Repository.Generic
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private EmployeeTimesheetEntities _context = null;
        private DbSet<T> _table = null;

        public GenericRepository()
        {
            this._context = new EmployeeTimesheetEntities();
            this._table = _context.Set<T>();
        }

        public GenericRepository(EmployeeTimesheetEntities _context)
        {
            this._context = _context;
            this._table = _context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _table.ToList();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> Criteria)
        {
            return _table.Where(Criteria).ToList();
        }

        public T GetByID(object ID)
        {
            return _table.Find(ID);
        }

        public void Insert(T Object)
        {
            _table.Add(Object);
        }

        public void Update(T Object)
        {
            _table.Attach(Object);
            _context.Entry(Object).State = EntityState.Modified;
        }

        public void Delete(object ID)
        {
            T existing = _table.Find(ID);
            _table.Remove(existing);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
