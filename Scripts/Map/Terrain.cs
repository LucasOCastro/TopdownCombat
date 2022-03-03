using Godot;

namespace CombatGame
{
    // public enum BlendRoughness
    // {
    //     TerrainRough = 1,
    //     TerrainSmooth = 0
    // }
    public class Terrain : Resource
    {
        [Export]
        public string Label {get; private set;}
        [Export]
        public Texture Texture {get; private set;}

        // [Export]
        // public BlendRoughness BlendRoughness {get; private set;}
        [Export]
        public int BlendStrength {get; private set;}

        [Export]
        public bool Passable { get; private set; } = true;

        public int terrainIndex;

        public override string ToString()
        {
            return Label;
        }
    }
}