using Godot;

namespace CombatGame
{
    public class BulletData : Resource
    {
        [Export]
        public Texture Texture { get; private set; }
        [Export]
        public float Scale { get; private set; } = 1;
        [Export]
        public float Speed { get; private set; } = 10;
    }
}