
using System.Collections.Generic;

namespace CombatGame
{
    public class Path
    {
        private List<PathGen.Node> internalPath;
        public Vec2Int this[int index] => internalPath[index].position;

        public Vec2Int End => this[internalPath.Count - 1];
        public Vec2Int Start => this[0];

        public int TileCount => internalPath.Count;

        public int PathCost => internalPath[internalPath.Count - 1].gCost;

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

        public static Path DeepCopy(Path path) => new Path(PathGen.Node.DeepCopy(path.internalPath[path.internalPath.Count - 1]));

        /// <summary>Removes the last 'amount' tiles of the path.</summary>
        public static Path TrimFromEnd(Path path, int amount)
        {
            if (amount >= path.TileCount){
                return null;
            }
            Path clone = DeepCopy(path);
            clone.internalPath.RemoveRange(path.internalPath.Count - amount, amount);
            return clone;
        }

        /// <summary>Removes the first 'amount' tiles of the path.</summary>
        public static Path TrimFromStart(Path path, int amount)
        {
            if (amount >= path.TileCount){
                return null;
            }
            Path clone = DeepCopy(path);
            clone.internalPath.RemoveRange(0, amount);
            clone.internalPath[0].parent = null;
            return clone;
        }

        public static Path LimitCost(Path path, int maxCost)
        {
            PathGen.Node node = path.internalPath[path.internalPath.Count - 1];
            while (node != null)
            {
                if (node.gCost <= maxCost)
                {
                    return new Path(PathGen.Node.DeepCopy(node));
                }
                node = node.parent;
            }
            return null;
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
                throw new System.Exception("Paths are not adjacent in " + nameof(MergePaths));
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