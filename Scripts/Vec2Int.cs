using Mathf = Godot.Mathf;
using Vector2 = Godot.Vector2;

namespace CombatGame
{
    public struct Vec2Int : System.IEquatable<Vec2Int>
    {
        public static Vec2Int Zero => new Vec2Int(0,0);
        public static Vec2Int One => new Vec2Int(1, 1);

        public static Vec2Int Floor(Vector2 vector) => new Vec2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        public Vec2Int Abs() => new Vec2Int(Mathf.Abs(x), Mathf.Abs(y));

        public int x;
        public int y;
        

        public Vec2Int(int x, int y)
        {
            this.x  = x;
            this.y = y;
        }

        public bool AdjacentTo(Vec2Int tile)
        {
            Vec2Int absDifference = (tile - this).Abs();
            return absDifference.x <= 1 && absDifference.y <= 1;
        }

        public bool DiagonallyAdjacentTo(Vec2Int tile)
        {
            Vec2Int absDifference = (tile - this).Abs();
            return absDifference.x == 1 && absDifference.y == 1;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }

        public override bool Equals(object obj) => obj is Vec2Int other && Equals(other);
        public bool Equals(Vec2Int other) => x == other.x && y == other.y;

        public override int GetHashCode() => (x,y).GetHashCode();

        public static explicit operator Vector2(Vec2Int intVec) => new Vector2(intVec.x, intVec.y);
        public static explicit operator Vec2Int(Vector2 vector) => new Vec2Int((int)vector.x, (int)vector.y);

        public static Vec2Int operator +(Vec2Int a, Vec2Int b){
            return new Vec2Int(a.x + b.x, a.y + b.y);
        }
        public static Vec2Int operator -(Vec2Int a, Vec2Int b){
            return new Vec2Int(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(Vec2Int a, Vec2Int b){
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Vec2Int a, Vec2Int b){
            return a.x != b.x || a.y != b.y;
        }
    }
}

