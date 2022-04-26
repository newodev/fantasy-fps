using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_ECS.ECSCore;

internal class NewArchetypeCollection
{
    private GenericComponentArray[] Components;

    // Buffer of entities that will be destroyed at EoF (end of frame)
    // The buffer of entities to spawn is stored in each component array
    private List<int> EntitiesToDestroy = new();

    public void DestroyEntityByID(int entityID)
    {
        EntitiesToDestroy.Add(entityID);
    }

    public void Update()
    {
        ClearDestroyBuffer();
        ClearSpawnBuffer();
    }

    public void ClearSpawnBuffer()
    {
        for (int i = 0; i < Components.Length; i++)
        {
            Components[i].ClearSpawnBuffer();
        }
    }

    private void ClearDestroyBuffer()
    {
        for (int i = 0; i < Components.Length; i++)
        {
            Components[i].ClearDestroyBuffer(EntitiesToDestroy);
        }
        EntitiesToDestroy.Clear();
    }
}

public abstract class GenericComponentArray
{
    public Type ComponentType { get; protected init; }
    internal abstract void DestroyByID(int entityID);
    internal abstract void ClearSpawnBuffer();
    internal abstract void ClearDestroyBuffer(List<int> entitiesToDestroy);
}
public class NewComponentArray<T> : GenericComponentArray where T : IComponent
{
    public int Count { get => contents.Count(); }
    private List<T> contents = new();

    private List<T> spawnBuffer = new();
    // TODO: At end of each frame, ensure lists are still sorted by ID. (only sort if changed this frame)

    internal NewComponentArray()
    {
        ComponentType = typeof(T);
    }

    internal override void ClearSpawnBuffer()
    {
        if (spawnBuffer.Count == 0)
            return;
        for (int i = 0; i < spawnBuffer.Count; i++)
        {
            contents.Add(spawnBuffer[i]);
        }

        SortByID();

        spawnBuffer.Clear();
    }

    internal override void ClearDestroyBuffer(List<int> entitiesToDestroy)
    {
        for (int j = entitiesToDestroy.Count - 1; j >= 0; j--)
        {
            int id = entitiesToDestroy[j];
            DestroyByID(id);
        }
    }

    // The ComponentArray is sorted by entity ID after each frame if new entities were spawned
    // This is so binary search can be used when getting a component by ID
    private void SortByID()
    {
        contents.Sort((a, b) => a.Id.CompareTo(b.Id));
    }

    public T this[int index]
    {
        get => GetComponent(index);
        set => SetComponent(index, value);
    }

    private T GetComponent(int i)
    {
        return contents[i];
    }

    private T SetComponent(int i, T val)
    {
        contents[i] = val;
        return contents[i];
    }

    public T GetByID(int entityID)
    {
        // Binary search for component by its ID
        int i = GetEntityIndexByID(entityID, 0, Count - 1);
        return contents[i];
    }

    public T SetByID(int entityID, T val)
    {
        // Binary search for component by its ID
        int i = GetEntityIndexByID(entityID, 0, Count - 1);
        contents[i] = val;
        return contents[i];
    }

    private int GetEntityIndexByID(int id, int start, int end)
    {
        // Binary search implementation
        int pivot = (start + end) / 2;

        if (contents[pivot].Id == id)
        {
            return pivot;
        }
        else if (contents[pivot].Id < id)
        {
            return GetEntityIndexByID(id, pivot + 1, end);
        }
        else if (contents[pivot].Id > id)
        {
            return GetEntityIndexByID(id, start, pivot - 1);
        }
        return -1;
    }

    internal override void DestroyByID(int entityID)
    {
        // Binary search for component by its ID
        int i = GetEntityIndexByID(entityID, 0, Count - 1);
        if (i != -1)
            contents.RemoveAt(i);
    }
}
