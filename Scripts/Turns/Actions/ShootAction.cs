using Godot;

namespace CombatGame
{
    public class ShootAction : Action
    {
        private Vec2Int target;
        private Weapon weapon;
        public ShootAction(Entity shooter, Weapon weapon, Vec2Int target) : base(shooter, weapon.ActionPointCost)
        {
            this.weapon = weapon;
            this.target = target;
            hitTile = CalcHitTile();
            hitTileCenter = GameManager.Instance.LoadedMap.GetTileCenter(hitTile);
        }

        private Vec2Int hitTile;
        private Vector2 hitTileCenter;
        private Sprite bullet;
        public override bool Tick(float delta)
        {
            if (bullet == null){
                InstantiateBullet();
            }

            bullet.GlobalPosition = bullet.GlobalPosition.MoveToward(hitTileCenter, weapon.BulletSpeed * delta);

            if (bullet.GlobalPosition == hitTileCenter){
                OnBulletLand();
                return true;
            }
            return false;
        }

        private void OnBulletLand()
        {
            bullet.QueueFree();
            Entity entity = GameManager.Instance.LoadedMap.EntityAt(hitTile);
            if (entity != null)
            {
                entity.Damage(weapon.Damage);
            }
        }

        private void InstantiateBullet()
        {
            bullet = new Sprite();
            bullet.Texture = weapon.BulletTexture;
            bullet.Scale = Vector2.One * weapon.BulletScale;
            bullet.Position = GameManager.Instance.LoadedMap.GetTileCenter(Doer.Position);
            GameManager.Instantiate(bullet);
            bullet.LookAt(hitTileCenter);
        }

        private Vec2Int CalcHitTile()
        {
            float hitChance = AttackUtility.CalculateRangedHitChance(Doer, weapon, target);
            float random = Random.Randf();
            return (random < hitChance) ? target : AttackUtility.CalculateMissedHitTile(Doer.Position, target, hitChance, random);
        }
    }
}