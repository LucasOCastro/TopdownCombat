using Godot;

namespace CombatGame
{
    public static class MapGenerator
    {
        //TODO hardcoded values, must study possible terraingen solutions
        public static Terrain GetTerrainAt(Vec2Int tile, OpenSimplexNoise simplex)
        {
            float noise = simplex.GetNoise2d(tile.x, tile.y);
            return noise < .2f ? ResourceDatabase<Terrain>.AllResources[0] : ResourceDatabase<Terrain>.AllResources[1];
        }
    }
}