using System.Collections.Generic;
using Godot;

namespace CombatGame
{
    public static class ResourceDatabase<T> where T: Resource
    {
        public static List<T> AllResources {get;} = new List<T>();

        public static T GetAny(bool throwOnFail = false) => throwOnFail ? AllResources.FirstOrThrow() : AllResources.FirstOrDefault();

        public static void Register(T resource)
        {
            AllResources.Add(resource);
        }
    }
}