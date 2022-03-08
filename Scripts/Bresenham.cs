using Mathf = Godot.Mathf;
using Vector2 = Godot.Vector2;
namespace CombatGame
{
    public static class Bresenham
    {
        //https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm#Method
        public static Vec2Int[] Rasterize(Vector2 start, Vector2 end, float scale)
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
    }
}