
using System.Collections.Generic;

namespace CombatGame
{
    public class Path
    {
        private List<PathGen.Node> internalPath;
        public Vec2Int this[int index] => internalPath[index].position;

        public Vec2Int End => this[TileCount - 1];
        public Vec2Int Start => this[0];

        public int TileCount => internalPath.Count;

        /// <summary>
        /// The number of tiles ignoring the starting tile.
        /// </summary>
        public int PathLength => TileCount - 1;

        public bool IsBefore(Path path) => End == path.Start || End.AdjacentTo(path.Start);
        public bool IsAfter(Path path) => path.IsBefore(this);

        public Path(PathGen.Node endNode)
        {
            internalPath = new List<PathGen.Node>(endNode.PathSize);
            PathGen.Node node = endNode;

            while (node != null)
            {
                internalPath.Insert(0, node);
                node = node.parent;
            }
        }

        /// <summary>Removes the last 'amount' tiles of the path.</summary>
        public static void TrimFromEnd(Path path, int amount)
        {
            if (amount <= 0){
                return;
            }
            if (amount >= path.TileCount){
                path.internalPath.Clear();
                return;
            }
            int index = path.internalPath.Count - amount;
            path.internalPath.RemoveRange(index, amount);
        }

        /// <summary>Removes the first 'amount' tiles of the path.</summary>
        public static void TrimFromStart(Path path, int amount)
        {
            if (amount <= 0){
                return;
            }
            if (amount >= path.TileCount){
                path.internalPath.Clear();
                return;
            }
            path.internalPath.RemoveRange(0, amount);
            if (path.TileCount > 0){
                path.internalPath[0].parent = null;
            }
        }

        /// <summary>Copies both paths and return a new path from their copies.</summary>
        public static Path MergePaths(Path a, Path b)
        {
            if (a == null) return b;
            if (b == null) return a;

            PathGen.Node aEnd = a.internalPath[a.TileCount - 1];
            PathGen.Node bEnd = b.internalPath[b.TileCount - 1];
            if (a.End == b.Start){
                aEnd = aEnd.parent;
            }

            if (!a.End.AdjacentTo(b.Start)){
                throw new System.ArgumentException("Paths are not adjacent in " + nameof(MergePaths), nameof(a));
            }

            PathGen.Node aEndCopy = PathGen.Node.DeepCopy(aEnd);
            PathGen.Node bEndCopy = PathGen.Node.DeepCopy(bEnd);

            PathGen.Node node = bEndCopy;
            while (node != aEndCopy && node != null)
            {
                if (node.parent == null){
                    node.parent = aEndCopy;
                }
                node = node.parent;
            }


            return new Path(bEndCopy);
        }
    }
}