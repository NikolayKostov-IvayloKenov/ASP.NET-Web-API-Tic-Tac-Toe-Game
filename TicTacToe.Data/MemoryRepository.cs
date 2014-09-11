namespace TicTacToe.Data
{
    using System.Collections.Generic;
    using System.Linq;

    public class MemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly Dictionary<object, T> data;

        public MemoryRepository()
        {
            this.data = new Dictionary<object, T>();
        }

        public bool Saved { get; set; }

        public void Dispose()
        {
        }

        public IQueryable<T> All()
        {
            return this.data.Values.AsQueryable();
        }

        public T GetById(object id)
        {
            return this.data[id];
        }

        public void Add(T entity)
        {
            this.AddOrUpdate(entity);
        }

        public void Update(T entity)
        {
            this.AddOrUpdate(entity);
        }

        public void Delete(T entity)
        {
            var id = entity.GetId();
            this.Delete(id);
        }

        public void Delete(object id)
        {
            if (this.data.ContainsKey(id))
            {
                this.data.Remove(id);
            }
        }

        public int SaveChanges()
        {
            this.Saved = true;
            return 1;
        }

        private void AddOrUpdate(T entity)
        {
            var id = entity.GetId();
            if (this.data.ContainsKey(id))
            {
                this.data[id] = entity;
            }
            else
            {
                this.data.Add(entity.GetId(), entity);
            }
        }
    }
}
