using Godot;

namespace CombatGame
{

    //TODO item resource
    public class WeaponBase : Resource
    {
        [Export]
        public int ActionPointCost { get; private set; } = 50;

        [Export]
        public BulletData BulletData { get; private set; }

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