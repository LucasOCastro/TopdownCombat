using Godot;

namespace CombatGame
{
    public abstract class MapObjectInstanceBase
    {
        public Sprite Renderer {get; private set;}

        protected MapObjectResource resourceBase;
        public MapObjectInstanceBase(MapObjectResource objectBase)
        {
            resourceBase = objectBase;

            Renderer = new Sprite();
            Renderer.Texture = objectBase.Sprite;
            Renderer.Scale = (Vector2.One * GameManager.GameScale) / Renderer.Texture.GetSize();
        }

        public Map CurrentMap { get; private set; }

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

                CurrentMap.UpdatePosition(this, oldPos, _position);
            }
        }

        public virtual void OnSpawn(Map map, Vec2Int position)
        {
            CurrentMap = map;
            GameManager.Instantiate(Renderer);
            Position = position;
        }

        public virtual void OnDespawn(Map map, Vec2Int position)
        {
            CurrentMap = null;
            //This is temporary, later probably wouldnt want to completly free it.
            Renderer.QueueFree();
        }

        public override string ToString()
        {
            return resourceBase.Label;
        }
    }

    public abstract class MapObjectInstance<T> : MapObjectInstanceBase where T: MapObjectResource
    {
        public T Base => (T)resourceBase;

        public MapObjectInstance(T resourceBase) : base(resourceBase)
        {
        }
    }
}