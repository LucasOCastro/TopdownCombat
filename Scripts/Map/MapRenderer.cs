using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public class MapRenderer : MeshInstance2D
    {

        public new ShaderMaterial Material => base.Material as ShaderMaterial;
        

        public void Init(int tileTextureSize, ImageTexture tileTextureAtlas, TextureArray tileTextureArray)
        {
            Material.SetShaderParam("tileTextureSize", tileTextureSize);
            Material.SetShaderParam("tileTextureCount", tileTextureArray.GetDepth());
            Material.SetShaderParam("textureArray", tileTextureArray);
            Texture = tileTextureAtlas;
        }

        public void Draw(Map map)
        {
            if (!(Mesh is QuadMesh quad))
            {
                quad = new QuadMesh();
                Mesh = quad;
            }

            quad.Size = Vector2.One * map.Size;
            quad.CenterOffset = new Vector3(quad.Size.x * .5f, quad.Size.y * .5f, 0);

            Texture indexTexture = MapRenderingUtility.TerrainTextureFor(map);
            Material.SetShaderParam("mapSize", map.Size);
            Material.SetShaderParam("terrainIndexTexture", indexTexture);

            //ResourceSaver.Save("res://mapIndexTexture.tres", indexTexture);

            if (!Visible) Visible = true;
        }
    }
}