using Godot;

namespace CombatGame
{
    public class ShootAction : Action
    {
        private Vec2Int target;
        private WeaponBase weapon;
        public ShootAction(Entity shooter, WeaponBase weapon, Vec2Int target, HitChanceReport hitChanceReport) : base(shooter, weapon.ActionPointCost)
        {
            this.weapon = weapon;
            this.target = target;
            hitTile = CalcHitTile(hitChanceReport);
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

            //If will miss, might be stopped by any cover in the path.
            Vec2Int currentTile = Map.GetTile(bullet.GlobalPosition);
            Structure structureOnTile = Map.GetAt<Structure>(currentTile);
            if (hitTile != target && structureOnTile != null && Random.Chance(structureOnTile.Base.CoverStrength)){
                hitTile = currentTile;
            }

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
        private Vec2Int CalcHitTile(HitChanceReport hitChanceReport)
        {
            //If should miss, miss anywhere
            if (!Random.Chance(hitChanceReport.PastCoverHitChance))
            {
                return AttackUtility.CalculateMissedHitTile(Doer.Position, target);
            }

            //If stopped by cover, hit random cover provider
            if (Random.Chance(hitChanceReport.totalCoverStrength))
            {
                return hitChanceReport.covers.RandomElementByWeight(c => c.coverStrength).coverProvider.Position;
            }

            return target;
        }
    }
}