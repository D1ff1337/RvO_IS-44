using System.Collections.Generic;

public interface IDataStorage<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    T GetById(int id);
    List<T> GetAll();
    void Save();
}
