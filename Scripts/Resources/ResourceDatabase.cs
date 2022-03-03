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
    // public static class ResourceDatabase
    // {
    //     public static List<Terrain> AllTerrain {get;} = new List<Terrain>();
    //     private static Dictionary<string, Terrain> terrainByName = new Dictionary<string, Terrain>();

    //     public static Terrain TerrainByName(string name, bool throwOnFail = false)
    //     {
    //         Terrain terrain = terrainByName.GetOrDefault(name);
    //         if (terrain == null)
    //         {
    //             throw new System.Exception("No terrain found with name "+name);
    //         }
    //         return terrain;
    //     }
    //     public static Terrain AnyTerrain => AllTerrain.FirstOrDefault();

    //     public static void RegisterTerrain(Terrain terrain)
    //     {
    //         AllTerrain.Add(terrain);
    //         terrainByName.Add(terrain.Label, terrain);
    //     }
    // }
}