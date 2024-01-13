using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.data.Abstract
{
    public interface IRepository<T> 
    {
        T GetById(int id);

        List<T> Getall();

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}