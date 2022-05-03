using System.Collections.Generic;

namespace CombatGame
{
    public static class PathGen
    {
        public const int BASE_COST = 10, DIAGONAL_COST = 14;
        private const int MAX_ITERATIONS = 600;
        //TODO i dont like this being public, but right now the constructor for Path needs it. Will need some refactoring
        public class Node
        {
            public Node parent;
            public Vec2Int position;

            /// <summary>Cost from beninning of path.</summary>
            public int gCost => parent != null ? parent.gCost + CalcCost(parent.position, position) : 0;

            /// <summary>Distance to end of path.</summary>
            public int hCost;

            public int FullCost => gCost + hCost;

            public int PathSize => (parent?.PathSize ?? 0) + 1;

            public Node(Vec2Int position, Vec2Int end, Node parent)
            {
                this.position = position;
                this.parent = parent;

                //Unsure if this is how I should calc this but ok
                Vec2Int distance = end - position;
                hCost = (distance.x * distance.x) + (distance.y * distance.y);
                //sqrt is probably unecessary?
                //hCost = Godot.Mathf.Sqrt(hCost);
            }

            private Node(){}

            public static Node ShallowCopy(Node node) => new Node() { parent = node.parent, position = node.position, hCost = node.hCost };

            public static Node DeepCopy(Node node)
            {
                Node copyEnd = ShallowCopy(node);
                node = copyEnd;
                while (node.parent != null)
                {
                    Node parentCopy = ShallowCopy(node.parent);
                    node.parent = parentCopy;
                    node = parentCopy;
                }
                return copyEnd;
            }

            public override string ToString() => base.ToString() + $"({position.ToString()})";
        }

        private static int CalcCost(Vec2Int a, Vec2Int b)
        {
            // return a.IntDistanceTo(b);
            if (a.IsDiagonallyAdjacentTo(b)){
                return DIAGONAL_COST;
            }
            return BASE_COST;
        }

        public static bool IsPassableFor(Entity entity, Vec2Int tile, Map map)
        {
            Terrain terrain = map.TerrainAt(tile);
            if (!terrain.Passable){
                return false;
            }
            Entity entityOnTile = map.GetAt<Entity>(tile);
            if (entityOnTile != null && entityOnTile.Faction != entity.Faction){
                return false;
            }
            Structure structureOnTile = map.GetAt<Structure>(tile);
            if (structureOnTile != null && !structureOnTile.Base.Passable){
                return false;
            }
            return true;
        }

        public static bool IsStandableFor(Entity entity, Vec2Int tile, Map map)
        {
            if (!IsPassableFor(entity, tile, map)){
                return false;
            }
            if (map.GetAt<Entity>(tile) != null || map.GetAt<Structure>(tile) != null){
                return false;
            }
            return true;
        }

        //This cache doesnt considers different maps and will need updating in case of accessibility changes
        private static Dictionary<(Vec2Int start, Vec2Int end), Path> pathCache = new Dictionary<(Vec2Int start, Vec2Int end), Path>();

        private static Dictionary<Vec2Int, Node> createdNodes = new Dictionary<Vec2Int, Node>();
        private static List<Node> openSet = new List<Node>();
        private static HashSet<Vec2Int> closedSet = new HashSet<Vec2Int>();
        private static Node PopLowestCostNode()
        {
            if (openSet.Count == 0) {
                throw new System.Exception("Tried to pop lowest cost node with empty openSet.");
            }

            int index = 0;
            Node bestNode = openSet[0];
            for (int i = 0; i < openSet.Count; i++)
            {
                Node node = openSet[i];
                if (openSet[i].FullCost < bestNode.FullCost || (openSet[i].FullCost == bestNode.FullCost && openSet[i].hCost < bestNode.hCost))
                {
                    index = i;
                    bestNode = openSet[i];
                }
            }
            openSet.RemoveAt(index);
            return bestNode;
        }
        public static Path FindPath(Vec2Int start, Vec2Int end, Map map, Entity entity)
        {
            if (pathCache.TryGetValue((start, end), out Path cachedPath)){
                return cachedPath;
            }

            if (!IsStandableFor(entity, end, map)){
                return null;
            }

            createdNodes.Clear();
            openSet.Clear();
            closedSet.Clear();

            Node startNode = new Node(start, end, null);
            createdNodes.Add(start, startNode);
            openSet.Add(startNode);


            int iterations = 0;
            while (openSet.Count > 0)
            {
                iterations++;
                if (iterations > MAX_ITERATIONS)
                {
                    Godot.GD.PushWarning("FindPath went over max iterations.");
                    break;
                }
                Node current = PopLowestCostNode();
                closedSet.Add(current.position);

                if (current.position == end){
                    Path path = new Path(current);
                    pathCache.Add((start, end), path);
                    return path;
                }

                foreach (Vec2Int neighbor in map.GetNeighbors(current.position))
                {
                    if (closedSet.Contains(neighbor) || !IsPassableFor(entity, neighbor, map)){
                        continue;
                    }
                    if (!createdNodes.TryGetValue(neighbor, out Node neighborNode))
                    {
                        neighborNode = new Node(neighbor, end, current);
                        createdNodes.Add(neighbor, neighborNode);
                        openSet.Add(neighborNode);
                        continue;
                    }
                    int gCost = current.gCost + CalcCost(current.position, neighbor);
                    if (gCost < neighborNode.gCost){
                        neighborNode.parent = current;
                    }
                }
            }
            return null;
        }
    }
}