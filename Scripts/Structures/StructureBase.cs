using Godot;
namespace CombatGame
{
    public class StructureBase : MapObjectResource
    {
        [Export]
        public bool Passable { get; private set; }

        [Export]
        public bool CanSeeThrough { get; private set; }

        [Export]
        public float CoverStrength { get; private set; }
    }
}