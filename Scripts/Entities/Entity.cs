using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public class Entity
    {
        public EntityBase Base {get;}

        public Map CurrentMap {get; private set;}

        private Vec2Int _position;
        public Vec2Int Position
        {
            get => _position;
            set
            {
                if (CurrentMap == null) return;

                Vec2Int oldPos = _position;
                _position = value;
                Renderer.Position = CurrentMap.GetTileCenter(_position);

                CurrentMap.UpdateEntityPosition(this, oldPos, _position);
            }
        }

        public Sprite Renderer {get; private set;}

        public Faction Faction { get; }
        public bool IsPlayer => Faction == Faction.Player;
        public bool IsEnemy => Faction == Faction.Enemy;

        public Weapon EquippedWeapon { get; set; }

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
            CurrentMap.RemoveEntity(this);
        }

        public void Damage(int damage)
        {
            Health -= damage;
        }

        public Entity(EntityBase entityBase, Faction faction)
        {
            Base = entityBase;
            Faction = faction;

            Renderer = new Sprite();
            Renderer.Texture = entityBase.Texture;
            Renderer.Scale = GameManager.GameScale / Renderer.Texture.GetSize();

            ActionPoints = Base.MaxActionPoints;
            Health = Base.BaseHealth;
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

        public void OnSpawn(Map map, Vec2Int position)
        {
            CurrentMap = map;
            GameManager.Instantiate(Renderer);
            Position = position;
        }
    }
}