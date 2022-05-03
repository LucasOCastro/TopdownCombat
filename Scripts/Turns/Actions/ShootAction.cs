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

			bullet = new Bullet(weapon.BulletData, Doer.Position, hitTile);
			bullet.onBulletLand += OnBulletLand;
		}

		private Vec2Int hitTile;
		private Bullet bullet;
		public override bool Tick(float delta)
		{
			if (bullet == null){
				return true;
			}
			return false;
		}

		private void OnBulletLand()
		{
			Entity entity = GameManager.Instance.LoadedMap.GetAt<Entity>(hitTile);
			if (entity != null)
			{
				entity.Damage(weapon.Damage);
			}
			bullet.onBulletLand -= OnBulletLand;
			bullet = null;
		}

		//On miss, may be stopped by any cover along the bullets path and may hit another tile.
		//On a hit, may only be stopped by the cover in use by the target entity.
		//I'm not a fan of this anymore, I think I should be satisfied with random hit tile.
		private Vec2Int CalcHitTile(HitChanceReport hitChanceReport)
		{
			//If should miss, miss anywhere
			if (!Random.Chance(hitChanceReport.DistanceHitChance))
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
