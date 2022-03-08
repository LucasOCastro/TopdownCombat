using Godot;

namespace CombatGame
{
    public class EntityBase : MapObjectResource
    {
        [Export]
        public int MaxActionPoints { get; private set; } = 100;

        [Export]
        public int BaseHealth { get; private set; }
    }
}