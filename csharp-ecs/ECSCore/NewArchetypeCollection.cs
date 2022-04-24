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
}

public class GenericComponentArray
{
    public Type ComponentType { get; protected init; }
}
public class NewComponentArray<T> : GenericComponentArray where T : IComponent
{
    public int Count { get => contents.Count(); }
    private List<T> contents = new();
    // TODO: At end of each frame, ensure lists are still sorted by ID. (only sort if changed this frame)

    internal NewComponentArray()
    {
        ComponentType = typeof(T);
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

    internal void DestroyByID(int entityID)
    {
        // Binary search for component by its ID
        int i = GetEntityIndexByID(entityID, 0, Count - 1);
        if (i != -1)
            contents.RemoveAt(i);
    }
}
