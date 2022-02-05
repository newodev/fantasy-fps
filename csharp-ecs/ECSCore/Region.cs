using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_ECS.ECSCore.Exceptions;
using System.Reflection;

namespace CSharp_ECS.ECSCore
{
    sealed class Region
    {
        public List<ArchetypeCollection> Archetypes = new List<ArchetypeCollection>();

        // Finds the archetype in this region that matches the key
        public ArchetypeCollection? FindArchetypeFromKey(byte key)
        {
            for (int i = 0; i < Archetypes.Count; i++)
            {
                if (Archetypes[i].Key == key)
                    return Archetypes[i];
            }

            return null;
        }

        public void Query(Delegate action)
        {
            ParameterInfo[] parameters = action.Method.GetParameters();
            Type[] componentTypes = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                // TODO: add guards here for if the type requested is not a ComponentCollection
                componentTypes[i] = parameters[i].ParameterType.GetGenericArguments()[0];
            }

            List<ArchetypeCollection> matches = GetMatchingArchetypes(componentTypes.ToHashSet());

            // The array of ComponentCollections the delegate is called with
            object[] args = new object[parameters.Length];
            // Info of the ComponentCollection's constructor used to generate generics at runtime (with Reflection)
            Type genericCollection = typeof(ComponentCollection<>);
            Type[] constructorParameters = new Type[] { typeof(List<ArchetypeCollection>) };

            // TODO: refactor this away... possibly into a static factory method in ComponentCollection?
            for (int i = 0; i < parameters.Length; i++)
            {
                object[] constructorArgs = new object[] { matches };
                args[i] = genericCollection
                    .MakeGenericType(parameters[i].ParameterType)
                    .GetConstructor(constructorParameters)
                    .Invoke(constructorArgs);
            }
            action.DynamicInvoke(args);
        }

        public void Query<T>(Action<ComponentCollection<T>> operation) 
            where T : IComponent
        {
            List<ArchetypeCollection> matches = GetMatchingArchetypes(new HashSet<Type> { typeof(T) });

            ComponentCollection<T> collection = new ComponentCollection<T>(matches);
            operation(collection);
        }

        public void Query<T1, T2>(Action<ComponentCollection<T1>, ComponentCollection<T2>> operation) 
            where T1: IComponent
            where T2 : IComponent
        {
            List<ArchetypeCollection> matches = GetMatchingArchetypes(new HashSet<Type> { typeof(T1), typeof(T2) });

            ComponentCollection<T1> collection1 = new ComponentCollection<T1>(matches);
            ComponentCollection<T2> collection2 = new ComponentCollection<T2>(matches);

            operation(collection1, collection2);
        }

        public void Query<T1, T2, T3>(Action<ComponentCollection<T1>, ComponentCollection<T2>, ComponentCollection<T3>> operation)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
        {
            List<ArchetypeCollection> matches = GetMatchingArchetypes(new HashSet<Type> { typeof(T1), typeof(T2), typeof(T3) });

            ComponentCollection<T1> collection1 = new ComponentCollection<T1>(matches);
            ComponentCollection<T2> collection2 = new ComponentCollection<T2>(matches);
            ComponentCollection<T3> collection3 = new ComponentCollection<T3>(matches);

            operation(collection1, collection2, collection3);
        }

        public void Query<T1, T2, T3, T4>(Action<ComponentCollection<T1>, ComponentCollection<T2>, ComponentCollection<T3>, ComponentCollection<T4>> operation)
            where T1 : IComponent
            where T2 : IComponent
            where T3 : IComponent
            where T4 : IComponent
        {
            List<ArchetypeCollection> matches = GetMatchingArchetypes(new HashSet<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });

            ComponentCollection<T1> collection1 = new ComponentCollection<T1>(matches);
            ComponentCollection<T2> collection2 = new ComponentCollection<T2>(matches);
            ComponentCollection<T3> collection3 = new ComponentCollection<T3>(matches);
            ComponentCollection<T4> collection4 = new ComponentCollection<T4>(matches);

            operation(collection1, collection2, collection3, collection4);
        }

        private List<ArchetypeCollection> GetMatchingArchetypes(HashSet<Type> query)
        {
            List<ArchetypeCollection> subset = Archetypes.Where(x => x.Contains(query)).ToList();

            return subset;
        }

        // Resolves all the Spawn/Destroy buffers within each ArchetypeCollection
        public void ResolveBuffers()
        {
            for (int i = 0; i < Archetypes.Count; i++)
            {
                Archetypes[i].ResolveBuffers();
            }
        }

        // TODO: Add DestroyEntity function
        public void DestroyEntity(int entityID)
        {
            byte key = IDRegistry.GetArchetypeKeyFromID(entityID);
            ArchetypeCollection? match = FindArchetypeFromKey(key);

            if (match != null)
            {
                match.DestroyEntityByID(entityID);
            }
            else
            {
                throw new ECSException($"Cannot find archetype with key {key} in function Region.DestroyEntity({entityID})");
            }
        }

        // 'Spawns' an entity by adding its components into the collection
        public void SpawnEntity(List<IComponent> components)
        {
            // TODO: this is allocating multiple lists... and a complex expression.... Look to simplify

            // Sort components by type name to match with an archetype
            components.Sort((x, y) => x.GetType().FullName.CompareTo(y.GetType().FullName));

            List<Type> key = new List<Type>();
            foreach (var component in components)
            {
                key.Add(component.GetType());
            }

            // Find an archetype that matches this new entity
            List<ArchetypeCollection> a = Archetypes.Where(x => x.Archetype.SequenceEqual(key)).ToList();
            // If this entity doesn't match an archetype, create a new one to match it
            if (a.Count() == 0)
            {
                ArchetypeCollection newArchetype = new ArchetypeCollection(key, IDRegistry.GetArchetypeKey(key));
                Archetypes.Add(newArchetype);
                newArchetype.SpawnEntity(components);
            }
            // Else add the entity to its matching archetype
            else
            {
                a.First().SpawnEntity(components);
            }
        }

        // Overload for convenience with a single component
        public void SpawnEntity(IComponent component)
        {
            SpawnEntity(new List<IComponent> { component });
        }
    }
}
