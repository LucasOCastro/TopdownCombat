using Mathf = Godot.Mathf;
using Vector2 = Godot.Vector2;

namespace CombatGame
{
    public struct Vec2Int : System.IEquatable<Vec2Int>
    {
        public static Vec2Int Zero => new Vec2Int(0,0);
        public static Vec2Int One => new Vec2Int(1, 1);
        public static Vec2Int Right => new Vec2Int(1, 0);
        public static Vec2Int Left => new Vec2Int(-1, 0);
        public static Vec2Int Up => new Vec2Int(0, -1);
        public static Vec2Int Down => new Vec2Int(0, 1);


        public static Vec2Int Floor(Vector2 vector) => new Vec2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
        public static Vec2Int Ceil(Vector2 vector) => new Vec2Int(Mathf.CeilToInt(vector.x), Mathf.CeilToInt(vector.y));
        public Vec2Int Abs() => new Vec2Int(Mathf.Abs(x), Mathf.Abs(y));
        public Vec2Int Sign() => new Vec2Int(Mathf.Sign(x), Mathf.Sign(y));

        public int x;
        public int y;
        

        public Vec2Int(int x, int y)
        {
            this.x  = x;
            this.y = y;
        }

        public bool IsAdjacentTo(Vec2Int tile)
        {
            Vec2Int absDifference = (tile - this).Abs();
            return absDifference.x <= 1 && absDifference.y <= 1;
        }

        public bool IsDiagonallyAdjacentTo(Vec2Int tile)
        {
            Vec2Int absDifference = (tile - this).Abs();
            return absDifference.x == 1 && absDifference.y == 1;
        }

        public int IntDistanceTo(Vec2Int tile)
        {
            Vec2Int difference = (tile - this).Abs();
            // return Mathf.FloorToInt(Mathf.Sqrt(difference.x * difference.x + difference.y * difference.y));
            int directCount = Mathf.Abs(difference.x - difference.y);
            int diagonalCount = Mathf.Abs(Mathf.Max(difference.x, difference.y) - directCount);
            return (directCount * PathGen.BASE_COST) + (diagonalCount * PathGen.DIAGONAL_COST);
        }

        public float DistanceTo(Vec2Int tile)
        {
            Vec2Int dif = tile - this;
            return Mathf.Sqrt((dif.x * dif.x) + (dif.y * dif.y));
        }

        public override string ToString() => $"({x},{y})";
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

        public static Vec2Int operator *(Vec2Int a, int b){
            return new Vec2Int(a.x * b, a.y * b);
        }

        public static bool operator ==(Vec2Int a, Vec2Int b){
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(Vec2Int a, Vec2Int b){
            return a.x != b.x || a.y != b.y;
        }
    }
}

