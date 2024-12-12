using AsmAppDev.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AsmAppDev.Repository
{
	public class Repository<T> where T : class
	{
		private readonly ApplicationDBContext _dbContext;
		internal DbSet<T> _dbSet { get; set; }

		public Repository(ApplicationDBContext dbContext)
		{
			_dbContext = dbContext;
			_dbSet = _dbContext.Set<T>();
		}
		public IEnumerable<T> GetAll(string? includedProperty = null)
		{
			IQueryable<T> query = _dbSet;
			if (!String.IsNullOrEmpty(includedProperty))
			{
				query.Include(includedProperty).ToList();
			}
			
			
			
			return query.ToList();
		}
		public T Get(Expression<Func<T, bool>> predicate, string? includedProperty = null)
		{
			IQueryable<T> query = _dbSet;
			query = query.Where(predicate);
			if (!String.IsNullOrEmpty(includedProperty))
			{
				query.Include(includedProperty).ToList();
			}
			return query.FirstOrDefault();
		}
		public void Add(T entity)
		{
			_dbSet.Add(entity);
		}
		public void Delete(T entity)
		{
			_dbSet.Remove(entity);
		}
		public void Save()
		{
			_dbContext.SaveChanges();
		}
	}

}
