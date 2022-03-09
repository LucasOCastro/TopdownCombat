using Godot;

namespace CombatGame
{
    public class ShootAction : Action
    {
        private Vec2Int target;
        private WeaponBase weapon;
        public ShootAction(Entity shooter, WeaponBase weapon, Vec2Int target) : base(shooter, weapon.ActionPointCost)
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
            Entity entity = GameManager.Instance.LoadedMap.GetAt<Entity>(hitTile);
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

        //On miss, may be stopped by any cover along the bullets path and may hit another tile.
        //On a hit, may only be stopped by the cover in use by the target entity.
        private Vec2Int CalcHitTile()
        {
            var hitInfo = AttackUtility.CalculateRangedHitChance(Doer, weapon, target);
            float random = Random.Randf();
            GD.Print($"COVER: {random}\\{hitInfo.totalCoverStrength}");

            if (random < hitInfo.totalCoverStrength)//(Random.Chance(hitInfo.totalCoverStrength))
            {
                return hitInfo.covers.RandomElementByWeight(c => c.coverStrength).coverProvider.Position;
            }

            random = Random.Randf();
            GD.Print($"HIT: {random}\\{hitInfo.HitChance}");
            return (random <= hitInfo.HitChance) ? target : AttackUtility.CalculateMissedHitTile(Doer.Position, target, hitInfo.HitChance, random);
        }
    }
}