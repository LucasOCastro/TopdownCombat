using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public static class MapRenderingUtility
    {
        private static Dictionary<int, ArrayMesh> sizeMeshCache = new Dictionary<int, ArrayMesh>();
        public static Mesh MeshFor(Map map)
        {
            int size = map.Size;
            if (!sizeMeshCache.TryGetValue(size, out ArrayMesh mesh))
            {
                mesh = MeshGen.GenerateTiledPlane(size);
                sizeMeshCache.Add(size, mesh);
            }
            return mesh;
        }

        private static Dictionary<Map, ImageTexture> mapTextureDict = new Dictionary<Map, ImageTexture>();
        public static Texture TerrainTextureFor(Map map)
        {
            if (!mapTextureDict.TryGetValue(map, out ImageTexture texture))
            {
                texture = GenTerrainTexture(map);
                mapTextureDict.Add(map, texture);
            }
            return texture;
        }
        //TODO i could also generate a blend texture I think, and remove the blend calculations from the shader
        private static ImageTexture GenTerrainTexture(Map map)
        {
            Image image = new Image();
            image.Create(map.Size, map.Size, false, Image.Format.Rgb8);
            image.Lock();
            for (int x = 0; x < map.Size; x++)
            {
                for (int y = 0; y < map.Size; y++)
                {
                    Terrain terrain = map.TerrainAt(new Vec2Int(x,y));
                    image.SetPixel(x, y, new Color(terrain.terrainIndex, terrain.BlendStrength, 0));
                }
            }
            image.Unlock();
            
            ImageTexture texture = new ImageTexture();
            texture.CreateFromImage(image, 0);
            return texture;
        }
    }
}