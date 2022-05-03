using Godot;

namespace CombatGame
{
    public class Bullet : Node2D
    {
        public delegate void BulletLandEvent();
        public BulletLandEvent onBulletLand;

        private BulletData data;
        private Vector2 hitPos;
        public Bullet(BulletData bulletData, Vec2Int spawnTile, Vec2Int hitTile)
        {
            data = bulletData;
            hitPos = GameManager.Instance.LoadedMap.GetTileCenter(hitTile);
            GlobalPosition = GameManager.Instance.LoadedMap.GetTileCenter(spawnTile);

            Sprite sprite = new Sprite
            {
                Texture = bulletData.Texture,
                Scale = Vector2.One * bulletData.Scale
            };
            AddChild(sprite);
            sprite.LookAt(hitPos);

            GameManager.Instantiate(this);
        }

        public override void _Process(float delta)
        {
            GlobalPosition = GlobalPosition.MoveToward(hitPos, data.Speed * delta);
            if (GlobalPosition == hitPos)
            {
                onBulletLand?.Invoke();
                this.QueueFree();
            }
        }
    }
}