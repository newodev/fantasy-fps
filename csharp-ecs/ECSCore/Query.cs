using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.Exceptions;
using System.Reflection;

namespace CSharp_ECS;


internal static class QueryFactory
{
    // Info of the Query<>'s constructor used to generate generics at runtime
    public static readonly Type Generic = typeof(Query<>);
    public static readonly Type[] ConstructorParams = new Type[] { typeof(ArchetypeCollection[]), typeof(Region) };

    // Generate an array of Queries, based on the input component types
    public static object[] ConstructQueries(ParameterInfo[] parameters, ArchetypeCollection[] matches, Region region)
    {
        object[] collections = new object[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            collections[i] = ConstructQuery(parameters[i], matches, region);
        }

        return collections;
    }

    // Generate a Query of the type specified in parameter, using reflection
    private static object ConstructQuery(ParameterInfo parameter, ArchetypeCollection[] matches, Region region)
    {
        object[] constructorArgs = new object[] { matches, region };

        // Convert the generic Query<> type to a Query<C>
        Type queryType = Generic
            .MakeGenericType(parameter.ParameterType.GenericTypeArguments[0]);

        // Invoke the constructor of this Query<C> to create our query
        // Use the binding flags as the constructor is internal (can't be instantiated by other assemblies)
        var constructor = queryType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, ConstructorParams);
        object query = constructor
            .Invoke(constructorArgs);

        return query;
    }
}
public class Query<T> where T : IComponent
{
    public int Count;
    private ComponentArray<T>[] matches;
    private Region region { get; init; }
    // An instance of T so that it doesn't have to be repeatedly instantiated to query
    private Type typeInstance = typeof(T);

    internal Query(ArchetypeCollection[] _matches, Region r)
    {
        matches = new ComponentArray<T>[_matches.Length];
        region = r;

        Count = 0;
        for (int i = 0; i < matches.Length; i++)
        {
            ArchetypeCollection a = _matches[i];

            Count += a.EntityCount;
            
            // TODO: Error checking here
            // TODO: Extract method probably
            for (int j = 0; j < a.ComponentTypes.Length; j++)
            {
                if (a.ComponentTypes[j] == typeInstance)
                {
                    ComponentArray<T>? ca = a.GetSegment(j) as ComponentArray<T>;
                    if (ca != null)
                        matches[i] = ca;
                }
            }
        }
    }

    public ref T GetRef(int i)
    {
        int match = FindEntityArchetype(i);
        int entityIndex = FindEntityIndexInArchetype(i);

        ComponentArray<T> a = matches[match];

        return ref a.GetRef(entityIndex);
    }

    public T this[int index]
    {
        get => GetComponent(index);
        set => SetComponent(index, value);
    }

    public T GetComponent(int i)
    {
        int match = FindEntityArchetype(i);
        int entityIndex = FindEntityIndexInArchetype(i);

        ComponentArray<T> a = matches[match];
        
        return a[entityIndex];
    }

    public ref T GetRefByID(int entityID)
    {
        ArchetypeCollection match = region.FindArchetypeFromKey(IDRegistry.GetArchetypeKeyFromID(entityID));
        
        ComponentArray<T> componentArray = match.GetSegmentFromType(typeInstance) as ComponentArray<T>;

        return ref componentArray.GetRefByID(entityID);
    }

    public void SetComponent(int i, T val)
    {
        int match = FindEntityArchetype(i);
        int entityIndex = FindEntityIndexInArchetype(i);

        ComponentArray<T> a = matches[match];

        // Maintain the current component's Id
        val.Id = a[entityIndex].Id;
        // Apply the value changes
        a[entityIndex] = val;
    }


    // Finds the index in matches of the ArchetypeCollection of the entity at i in this array
    public int FindEntityArchetype(int totalIndex)
    {
        // TODO: This is a point that could use a lot of optimisation. Could possibly cache ranges and just go straight to the correct Collection
        if (matches.Length == 1)
            return 0;
        for (int i = 0; i < matches.Length; i++)
        {
            ComponentArray<T> a = matches[i];
            if (totalIndex >= a.Count)
            {
                totalIndex -= a.Count;
            }
            else
            {
                return i;
            }
        }
        throw new IndexOutOfRangeException("Index out of bounds in Query");
    }

    public int FindEntityIndexInArchetype(int totalIndex)
    {
        int entityIndex = totalIndex;
        for (int i = 0; i < matches.Length; i++)
        {
            ComponentArray<T> a = matches[i];
            if (entityIndex >= a.Count)
            {
                entityIndex -= a.Count;
            }
            else
            {
                return entityIndex;
            }
        }
        throw new IndexOutOfRangeException($"Index out of bounds in Query<{typeInstance.Name}> : index was {totalIndex}");
    }
}
