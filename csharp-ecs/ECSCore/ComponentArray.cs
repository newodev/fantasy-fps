﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore.Exceptions;
using System.Reflection;

namespace CSharp_ECS.ECSCore;

// We use an abstract subclass as the ArchetypeCollection cannot interact with the generic types of the ComponentArrays
public abstract class GenericComponentArray
{
    internal static readonly Type Generic = typeof(ComponentArray<>);
    internal static GenericComponentArray FromComponentType(Type componentType)
    {
        object[] constructorArgs = Array.Empty<object>();
        // Convert the generic ComponentArray<> type to a ComponentArray<C>
        Type componentArrayType = Generic.MakeGenericType(componentType);

        // Invoke the constructor of this ComponentArray<C> to create our collection
        // Use the binding flags as the constructor is internal (can't be instantiated by other assemblies)
        var constructor = componentArrayType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, Array.Empty<Type>());
        GenericComponentArray? collection = constructor.Invoke(constructorArgs) as GenericComponentArray;

        if (collection == null)
            throw new Exception($"Error creating new ComponentArray with type {componentType.FullName}: constructor invocation returned null");

        return collection;
    }

    public Type ComponentType { get; protected init; }
    internal abstract void DestroyByID(int entityID);
    internal abstract void ClearSpawnBuffer();
    internal abstract void ClearDestroyBuffer(List<int> entitiesToDestroy);
    internal abstract void AddToSpawnBuffer(IComponent component);
}

public class ComponentArray<T> : GenericComponentArray where T : IComponent
{
    public int Count { get => contents.Count(); }
    private List<T> contents = new();

    private List<T> spawnBuffer = new();

    internal ComponentArray()
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
        int i = GetComponentIndexByID(entityID, 0, Count - 1);
        return contents[i];
    }

    public T SetByID(int entityID, T val)
    {
        // Binary search for component by its ID
        int i = GetComponentIndexByID(entityID, 0, Count - 1);
        contents[i] = val;
        return contents[i];
    }

    internal override void DestroyByID(int entityID)
    {
        // Binary search for component by its ID
        int i = GetComponentIndexByID(entityID, 0, Count - 1);
        if (i != -1)
            contents.RemoveAt(i);
    }

    public int GetComponentIndexByID(int id, int start, int end)
    {
        // Binary search implementation
        int pivot = (start + end) / 2;

        if (contents[pivot].Id == id)
        {
            return pivot;
        }
        else if (contents[pivot].Id < id)
        {
            return GetComponentIndexByID(id, pivot + 1, end);
        }
        else if (contents[pivot].Id > id)
        {
            return GetComponentIndexByID(id, start, pivot - 1);
        }
        return -1;
    }

    internal override void AddToSpawnBuffer(IComponent component)
    {
        T castedComponent = (T)component;
        spawnBuffer.Add(castedComponent);
    }
}