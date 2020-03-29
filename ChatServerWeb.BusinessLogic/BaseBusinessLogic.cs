using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ChatServerWeb.Data;
using ChatServerWeb.Model.Entity;
using Microsoft.Win32.SafeHandles;

namespace ChatServerWeb.BusinessLogic
{
    public class BaseBusinessLogic<E> where E : class
    {
        protected ChatServerDbContext context;
        protected IRepository _repository;
        protected const string ArgumentNullException = "Null object argument. Please contact your system administrator";

        public BaseBusinessLogic(IRepository repository)
        {
            _repository = repository;
        }


        public virtual E GetEntityBy(Expression<Func<E, bool>> selector, string includeProperties = "")
        {
            try
            {
                return _repository.GetSingleBy(selector, includeProperties);
            }
            catch (Exception)

            {
                throw;
            }
        }
        public virtual List<E> GetAllEntities()
        {
            try
            {
                return _repository.GetAll<E>().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public virtual List<E> GetEntitiesBy(Expression<Func<E, bool>> selector = null, Func<IQueryable<E>, IOrderedQueryable<E>> orderBy = null, string includeProperties = "")
        {
            try
            {
                return _repository.GetBy(selector, orderBy, includeProperties).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public bool Delete(Expression<Func<E, bool>> selector)
        {
            try
            {
                _repository.Delete(selector);
                return Save() > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool Delete(object id)
        {
            try
            {
                _repository.Delete(id);
                return Save() > 0 ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Save()
        {
            return _repository.Save();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_repository != null)
                {
                    _repository.Dispose();
                    _repository = null;
                }
            }
        }
        public virtual E AddEntity(E entity)
        {
            try
            {

                E addedEntity = _repository.Add(entity);

                //_repository.Save(); // ToDo: Remove this and create a unit of work for each entity domain

                return addedEntity;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException(ArgumentNullException);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
