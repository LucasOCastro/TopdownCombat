using Godot;

namespace CombatGame
{
    public class EntityBase : Resource
    {
        [Export]
        public string Label {get; private set;}

        [Export]
        public Texture Texture {get; private set;}

        [Export]
        public int MaxActionPoints { get; private set; } = 100;

        [Export]
        public int BaseHealth { get; private set; }
    }
}