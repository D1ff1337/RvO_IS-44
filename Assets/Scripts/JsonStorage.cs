using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class JsonStorage<T> : IDataStorage<T> where T : class
{
    private List<T> items = new List<T>();
    private string filePath;

    public JsonStorage(string fileName)
    {
        filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), fileName);
        Load();
    }


    private void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            if (wrapper != null && wrapper.items != null)
                items = wrapper.items;
        }
    }

    public void Add(T entity)
    {
        items.Add(entity);
    }

    public void Update(T entity)
    {
        Save(); // На лабу достаточно просто пересохранить весь список
    }

    public void Delete(int id)
    {
        var item = items.FirstOrDefault(e => GetId(e) == id);
        if (item != null)
        {
            items.Remove(item);
            Save();
        }
    }

    public T GetById(int id)
    {
        return items.FirstOrDefault(e => GetId(e) == id);
    }

    public List<T> GetAll()
    {
        return new List<T>(items);
    }

    public void Save()
    {
        Wrapper<T> wrapper = new Wrapper<T> { items = items };
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"💾 Данные сохранены в файл: {filePath}");
    }

    // ===== ВАЖНО: получение Id из объекта =====
    private int GetId(T entity)
    {
        var idField = entity.GetType().GetField("Id");
        if (idField != null)
        {
            return (int)idField.GetValue(entity);
        }
        else
        {
            Debug.LogError("❌ У объекта нет поля 'Id'!");
            return -1;
        }
    }

    [System.Serializable]
    private class Wrapper<U>
    {
        public List<U> items;
    }

    public void ClearAll()
    {
        items.Clear();
    }

}
