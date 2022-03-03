using Vector2 = Godot.Vector2;

namespace CombatGame
{
    public class MoveAction : Action
    {
        private int currentTargetTileIndex = 0;
        public int CurrentIndexInPath => Godot.Mathf.Max(0, currentTargetTileIndex-1);

        public Path Path { get; }

        public MoveAction(Entity doer, Path path) : base(doer, path.PathCost)
        {
            if (path == null){
                throw new System.Exception("Null path in " + nameof(MoveAction));
            }
            Path = path;
        }

        public override bool Tick(float delta)
        {
            bool reached = MoveToTarget(delta);
            if (!reached){
                return false;
            }

            Doer.Position = Path[currentTargetTileIndex];
            currentTargetTileIndex++;
            return currentTargetTileIndex >= Path.TileCount;
        }
        
        //TODO temp
        private bool MoveToTarget(float delta)
        {
            Vector2 rendererTarget = Doer.CurrentMap.GetTileCenter(Path[currentTargetTileIndex]);
            Doer.Renderer.GlobalPosition = Doer.Renderer.GlobalPosition.MoveToward(rendererTarget, 50 * delta);

            return Doer.Renderer.GlobalPosition == rendererTarget;
        }
    }
}