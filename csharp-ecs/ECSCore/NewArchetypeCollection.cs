using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace CSharp_ECS.ECSCore;

// TODO: Integrate with queries etc
internal class NewArchetypeCollection
{
    // The key generated on a per-archetype basis
    public readonly byte Key;

    private GenericComponentArray[] ComponentArrays;

    private Type[] ComponentTypes;

    // Buffer of entities that will be destroyed at EoF (end of frame)
    // The buffer of entities to spawn is stored in each component array
    private List<int> EntitiesToDestroy = new();

    internal NewArchetypeCollection(Type[] types)
    {
        ComponentTypes = types;
        ComponentArrays = new GenericComponentArray[types.Length];
        for (int i = 0; i < types.Length; i++)
        {
            ComponentArrays[i] = GenericComponentArray.FromComponentType(types[i]);
        }
    }

    public void DestroyEntityByID(int entityID)
    {
        EntitiesToDestroy.Add(entityID);
    }

    public bool Contains(HashSet<Type> query)
    {
        if (query == null)
            throw new ArgumentNullException("query");

        if (query.IsSubsetOf(ComponentTypes))
            return true;
        else
            return false;
    }

    public void Update()
    {
        ClearDestroyBuffer();
        ClearSpawnBuffer();
    }

    public void SpawnEntity(Span<IComponent> components)
    {
        int id = IDRegistry.GetNewID(Key);

        // This assumes that the order of components given matches the order of ComponentArrays
        for (int i = 0; i < components.Length; i++)
        {
            components[i].Id = id;
            ComponentArrays[i].AddToSpawnBuffer(components[i]);
        }
    }

    public void DestroyEntity(int entityID)
    {
        EntitiesToDestroy.Add(entityID);
    }

    public void ClearSpawnBuffer()
    {
        for (int i = 0; i < ComponentArrays.Length; i++)
        {
            ComponentArrays[i].ClearSpawnBuffer();
        }
    }

    private void ClearDestroyBuffer()
    {
        for (int i = 0; i < ComponentArrays.Length; i++)
        {
            ComponentArrays[i].ClearDestroyBuffer(EntitiesToDestroy);
        }
        EntitiesToDestroy.Clear();
    }


}

// We use an abstract subclass as the ArchetypeCollection cannot interact with the generic types of the ComponentArrays
public abstract class GenericComponentArray
{
    public static readonly Type Generic = typeof(ComponentArray<>);
    public static GenericComponentArray FromComponentType(Type componentType)
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

public class NewComponentArray<T> : GenericComponentArray where T : IComponent
{
    public int Count { get => contents.Count(); }
    private List<T> contents = new();

    private List<T> spawnBuffer = new();

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
