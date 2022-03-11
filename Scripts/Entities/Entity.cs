using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public class Entity : MapObjectInstance<EntityBase>
    {
        public Faction Faction { get; }
        public bool IsPlayer => Faction == Faction.Player;
        public bool IsEnemy => Faction == Faction.Enemy;

        //TODO temp
        public int MaximumSeeDistance { get; } = 200;
        public int MaximumEfficientShootDistance { get; } = 114;

        public WeaponBase EquippedWeapon { get; set; }

        private int _actionPoints;
        public int ActionPoints
        {
            get => _actionPoints;
            set => _actionPoints = Mathf.Clamp(value, 0, Base.MaxActionPoints);
        }
        public void MaxOutActionPoints() => _actionPoints = Base.MaxActionPoints;

        private int _health;
        public int Health
        {
            get => _health;
            set 
            {
                _health = Mathf.Clamp(value, 0, Base.BaseHealth);
                if (_health == 0)
                {
                    Die();
                }
            } 
        }
        public void MaxOutHealth() => _health = Base.BaseHealth;


        private void Die()
        {
            Renderer.QueueFree();
            CurrentMap.Despawn(this);
        }

        public void Damage(int damage)
        {
            Health -= damage;
        }

        public bool CanSeeTile(Vec2Int tile)
        {
            if (AttackUtility.IsInLineOfSight(this, tile)){
                return true;
            }
            return false;
        }

        public Entity(EntityBase entityBase, Faction faction) : base(entityBase)
        {
            Faction = faction;
            MaxOutActionPoints();
            MaxOutHealth();
        }

        public override void OnSpawn(Map map, Vec2Int position)
        {
            base.OnSpawn(map, position);
            map.RegisterInFaction(this);
        }

        public override void OnDespawn(Map map, Vec2Int position)
        {
            base.OnDespawn(map, position);
            map.UnregisterFromFaction(this);
        }

        public List<ActionAimer> GenerateActions()
        {
            List<ActionAimer> result = new List<ActionAimer>();

            result.Add(new MoveActionAimer(this));

            if (EquippedWeapon != null){
                result.Add(new ShootActionAimer(this, EquippedWeapon));
            }

            return result;
        }
    }
}