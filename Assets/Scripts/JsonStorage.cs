using System;
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
        // Збереження файлів у кореневій папці проєкту
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Load();
    }

    private void Load()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
                if (wrapper != null && wrapper.Items != null)
                {
                    items = wrapper.Items;
                    Debug.Log($"📂 Завантажено {items.Count} елементів з файлу: {filePath}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Помилка завантаження файлу: {filePath}\n{ex.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Файл не знайдено: {filePath}");
        }
    }

    public void Add(T entity)
    {
        if (entity == null)
        {
            Debug.LogError("❌ Неможливо додати порожній об'єкт!");
            return;
        }

        items.Add(entity);
        Save();
    }

    public void Update(T entity)
    {
        if (entity == null)
        {
            Debug.LogError("❌ Неможливо оновити порожній об'єкт!");
            return;
        }

        int id = GetId(entity);
        var existingItem = items.FirstOrDefault(e => GetId(e) == id);
        if (existingItem != null)
        {
            int index = items.IndexOf(existingItem);
            items[index] = entity;
            Save();
        }
        else
        {
            Debug.LogWarning($"⚠️ Об'єкт з ID {id} не знайдено!");
        }
    }

    public void Delete(int id)
    {
        var item = items.FirstOrDefault(e => GetId(e) == id);
        if (item != null)
        {
            items.Remove(item);
            Save();
            Debug.Log($"🗑️ Видалено об'єкт з ID {id}");
        }
        else
        {
            Debug.LogWarning($"⚠️ Об'єкт з ID {id} не знайдено!");
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
        try
        {
            Wrapper<T> wrapper = new Wrapper<T> { Items = items };
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"💾 Дані збережено у файл: {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Помилка збереження файлу: {filePath}\n{ex.Message}");
        }
    }

    private int GetId(T entity)
    {
        var idProperty = entity.GetType().GetProperty("Id");
        if (idProperty != null)
        {
            return (int)idProperty.GetValue(entity);
        }
        else
        {
            Debug.LogError("❌ У об'єкта немає поля 'Id'!");
            return -1;
        }
    }

    public void ClearAll()
    {
        items.Clear();
        Save();
        Debug.Log($"🗑️ Усі дані очищено у файлі: {filePath}");
    }

    [Serializable]
    private class Wrapper<U>
    {
        public List<U> Items;
    }
}
