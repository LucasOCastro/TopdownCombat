using Godot;

namespace CombatGame
{
    public class Weapon : Resource
    {
        [Export]
        public string Label { get; private set; }

        [Export]
        public int ActionPointCost { get; private set; } = 50;

        [Export]
        public Texture BulletTexture { get; private set; }
        [Export]
        public float BulletScale { get; private set; } = 1;
        [Export]
        public float BulletSpeed { get; private set; } = 10;

        [Export]
        public int Damage { get; private set; }

        [Export]
        public float Accuracy { get; private set; } = 1f;

        [Export]
        public int MinDistanceForFallof { get; private set; }
        [Export]
        public int MaxDistanceForFallof { get; private set; } = 1000;
        [Export]
        public float DistanceAccuracyFallof { get;  private set; }
    }
}