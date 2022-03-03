using Godot;

namespace CombatGame
{
    public class ShootActionAimer : ActionAimer
    {
        private Weapon weapon;
        public ShootActionAimer(Entity doer, Weapon weapon) : base(doer)
        {
            this.weapon = weapon;
        }

        public override bool CanCurrentlyBeSelected() => ActionDoer.ActionPoints >= weapon.ActionPointCost;

        public override Action Tick(float delta)
        {
            Map map = GameManager.Instance.LoadedMap;
            Vec2Int tile = map.GetMousePosition();
            if (map.InBounds(tile)){
                GameManager.Instance.GridDrawer.HighlightTile(tile, Colors.Yellow, 1f);
            }

            return null;
        }

        public override Action Input(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton mouse) || !mouse.Pressed || mouse.ButtonIndex != (int)ButtonList.Right){
                return null;
            }

            Vec2Int tile = GameManager.Instance.LoadedMap.GetMousePosition();
            if (!CanAimAt(tile)){
                return null;
            }

            return new ShootAction(ActionDoer, weapon, tile);
        }

        private bool CanAimAt(Vec2Int tile)
        {
            return GameManager.Instance.LoadedMap.InBounds(tile) && tile != ActionDoer.Position;
        }

        public override void Clear()
        {
        }
    }
}