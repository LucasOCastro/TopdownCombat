using Godot;

namespace CombatGame
{
    public class ShootActionAimer : ActionAimer
    {
        private WeaponBase weapon;
        public ShootActionAimer(Entity doer, WeaponBase weapon) : base(doer)
        {
            this.weapon = weapon;
        }

        public override bool CanCurrentlyBeSelected() => ActionDoer.ActionPoints >= weapon.ActionPointCost;

        private Vec2Int? currentTargetTile = null;
        private bool canAimAtCurrentTarget;
        public override Action Tick(float delta)
        {
            Map map = GameManager.Instance.LoadedMap;
            Vec2Int tile = map.GetMousePosition();
            if (currentTargetTile == null || tile != currentTargetTile)
            {
                currentTargetTile = tile;
                canAimAtCurrentTarget = CanAimAt(tile);
            }

            if (tile != null && map.InBounds(tile)){
                GameManager.Instance.GridDrawer.HighlightTile(tile, canAimAtCurrentTarget ? Colors.Yellow : Colors.Red, 1f);
            }

            return null;
        }

        public override Action Input(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton mouse) || !mouse.Pressed || mouse.ButtonIndex != (int)ButtonList.Right){
                return null;
            }

            if (currentTargetTile == null || !canAimAtCurrentTarget){
                return null;
            }

            return new ShootAction(ActionDoer, weapon, currentTargetTile.Value);
        }

        private bool CanAimAt(Vec2Int tile)
        {
            if (tile == ActionDoer.Position){
                return false;
            }
            if (!GameManager.Instance.LoadedMap.InBounds(tile)){
                return false;
            }
            if (!ActionDoer.CanSeeTile(tile)){
                return false;
            }
            return true;
        }

        public override void Clear()
        {
            currentTargetTile = null;
        }
    }
}