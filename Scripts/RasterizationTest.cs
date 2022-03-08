using Godot;

namespace CombatGame
{
    public class RasterizationTest : Node
    {
        //https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm#Method
        private static Vec2Int[] Rasterize(Vector2 start, Vector2 end, float scale)
        {
            start /= scale;
            end /= scale;
            Vector2 difference = end - start;

            Vector2 differenceAbs = difference.Abs();
            bool wide = differenceAbs.x > differenceAbs.y;

            //TODO dividing by zero sometimes :(
            float slope = (float)difference.y / (float)difference.x;
            float intercept = start.y - (slope * start.x);
            float lineFunction(float x, float y){
                return (slope * x) + intercept - y;
            }

            int size = Mathf.CeilToInt(wide ? differenceAbs.x : differenceAbs.y) + 1;
            Vec2Int[] result = new Vec2Int[size];
            if (size == 0){
                return result;
            }
            result[0] = (Vec2Int)start;

            
            Vec2Int differenceSign = (Vec2Int)difference.Sign();
            Vector2 midpointOffset = (Vector2)differenceSign * new Vector2(wide ? 1f : .5f, wide ? .5f : 1f);
            for (int i = 1; i < size; i++)
            {
                Vec2Int previous = result[i - 1];
                Vector2 previousCenter = (Vector2)previous + (Vector2.One * .5f);
                Vector2 midpoint = previousCenter + midpointOffset;
                float evaluated = lineFunction(midpoint.x, midpoint.y);

                Vec2Int offset = new Vec2Int(wide ? differenceSign.x : 0, wide ? 0 : differenceSign.y);
                bool changeLane = Mathf.Sign(evaluated) == Mathf.Sign(slope) * differenceSign.x;
                if (!wide){
                    changeLane = !changeLane;
                }

                if (changeLane)
                {
                    if (wide) offset.y = differenceSign.y;
                    else offset.x = differenceSign.x;
                }

                result[i] = previous + offset;
            }
            return result;
        }

        private Vec2Int? start = null;
        private Vec2Int? end = null;

        [Export]
        private NodePath linePath;
        private Line2D line;

        public override void _Ready()
        {
            line = GetNode<Line2D>(linePath);
        }

        public override void _Process(float delta)
        {
            //line.ClearPoints();
            if (start == null) return;



            Vec2Int end = this.end ?? GameManager.Instance.LoadedMap.GetMousePosition(); 
            if (GameManager.Instance.LoadedMap.InBounds(end) && start != end){
                DrawTo(end);
            }
        }

        private void DrawTo(Vec2Int end)
        {
            // Vec2Int[] rasterized = Rasterize(start.Value, end);
            Map map = GameManager.Instance.LoadedMap;
            Vec2Int[] rasterized = Rasterize(map.GetTileCenter(start.Value), map.GetTileCenter(end), GameManager.GameScale);
            GameManager.Instance.GridDrawer.HighlightTiles(rasterized, Colors.HotPink, 1.8f);
            line.Points = new Vector2[]{map.GetTileCenter(start.Value), map.GetTileCenter(end)};
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton mouse) || !mouse.Pressed) return;
            if (mouse.ButtonIndex == (int)ButtonList.Left)
            {
                if (start != null && end != null){
                    start = null;
                    end = null;
                }
                Vec2Int tile = GameManager.Instance.LoadedMap.GetMousePosition();
                if (start == null && GameManager.Instance.LoadedMap.InBounds(tile)){
                    start = tile;
                }
                else if (end == null && GameManager.Instance.LoadedMap.InBounds(tile)){
                    end = tile;
                }
                return;
            }

            void doTest(Vector2 pos)
            {

                Vector2 start = GameManager.Instance.LoadedMap.GetTileCenter(this.start.Value) / GameManager.GameScale;
                Vector2 end = GameManager.Instance.LoadedMap.GetTileCenter(this.end.Value) / GameManager.GameScale;
                Vector2 difference = end - start;
                float slope = (float)difference.y / (float)difference.x;
                float intercept = start.y - (slope * start.x);
                GD.Print(slope * pos.x + intercept - pos.y);
            }

            if (mouse.ButtonIndex == (int)ButtonList.Right && start != null && end != null)
            {
                Vec2Int tile = GameManager.Instance.LoadedMap.GetMousePosition();
                Vector2 center = GameManager.Instance.LoadedMap.GetTileCenter(tile) / GameManager.GameScale;
                doTest(center);
                return;
            }

            if (mouse.ButtonIndex == (int)ButtonList.Middle && start != null && end != null){
                Vec2Int tile = GameManager.Instance.LoadedMap.GetMousePosition();
                Vector2 center = GameManager.Instance.LoadedMap.GetTileCenter(tile) / GameManager.GameScale;
                Vector2 midpointWithAbove = center + new Vector2(0, -.5f);
                doTest(midpointWithAbove);
            }
        }
    }
}