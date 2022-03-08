using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public class Map
    {
        public int Size {get;}

        private int XYToIndex(Vec2Int xy) => XYToIndex(xy.x, xy.y);
        private int XYToIndex(int x, int y) => (y * Size) + x;
        private Vec2Int IndexToXY(int i) => new Vec2Int(Mathf.FloorToInt(i/Size), (i % Size));

        public List<Entity> AllEntities { get; } = new List<Entity>();
        private Dictionary<Faction, List<Entity>> entitiesByFaction = new Dictionary<Faction, List<Entity>>();
        public List<Entity> EntitiesOfFaction(Faction faction) => entitiesByFaction.GetOrAddNew(faction);
        public void RegisterInFaction(Entity entity) => EntitiesOfFaction(entity.Faction).Add(entity);
        public void UnregisterFromFaction(Entity entity) => EntitiesOfFaction(entity.Faction).Remove(entity);

        private Terrain[] terrainGrid;
        public Terrain TerrainAt(Vec2Int xy) => terrainGrid[XYToIndex(xy)];

        private Dictionary<System.Type, MapObjectInstanceBase[]> gridsDict = new Dictionary<System.Type, MapObjectInstanceBase[]>();
        private Dictionary<System.Type, List<MapObjectInstanceBase>> listsDict = new Dictionary<System.Type, List<MapObjectInstanceBase>>();
        private MapObjectInstanceBase[] GetGrid(System.Type type) => gridsDict.GetOrAddNew(type, Size * Size);
        private List<MapObjectInstanceBase> GetList(System.Type type) => listsDict.GetOrAddNew(type);
        public T GetAt<T>(Vec2Int xy) where T : MapObjectInstanceBase => GetGrid(typeof(T))[XYToIndex(xy)] as T;

        public void UpdatePosition(MapObjectInstanceBase obj, Vec2Int from, Vec2Int to)
        {
            GD.Print("Update for " + obj + " from " + from + XYToIndex(from) + " to " + to + XYToIndex(to));
            //FIXME not working
            var grid = GetGrid(obj.GetType());
            int fromIndex = XYToIndex(from);
            if (grid[fromIndex] == obj) grid[fromIndex] = null;
            grid[XYToIndex(to)] = obj;

            if (obj.Position != to) obj.Position = to;
        }

        public Vector2 GetTileCenter(Vec2Int tile) => ((Vector2)tile + Vector2.One * .5f) * GameManager.GameScale;

        public Vec2Int GetTile(Vector2 worldPos) => Vec2Int.Floor(worldPos / GameManager.GameScale);

        public bool CanSeeThrough(Vec2Int tile) => GetAt<Structure>(tile)?.Base.CanSeeThrough ?? true;

        public Vec2Int GetMousePosition()
        {
            Viewport viewport = GameManager.Instance.GetViewport();
            Vector2 position = viewport.CanvasTransform.AffineInverse() * viewport.GetMousePosition();
            return GetTile(position);
        }

        public bool InBounds(Vec2Int tile) => tile.x >= 0 && tile.x < Size && tile.y >= 0 && tile.y < Size;

        

        public IEnumerable<Vec2Int> GetNeighbors(Vec2Int center){
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0){
                        continue;
                    }
                    Vec2Int tile = center + new Vec2Int(x, y);
                    if (InBounds(tile)){
                        yield return tile;
                    }
                }
            }
        }

        public Map(int size)
        {
            Size = size;
            terrainGrid = new Terrain[size * size];

            FillTerrain();

            StructureBase structure = ResourceDatabase<StructureBase>.GetAny();
            var grid = GetGrid(typeof(Structure));
            SpawnAt(new Structure(structure), new Vec2Int(4, 4));
            SpawnAt(new Structure(structure), new Vec2Int(5, 4));
            SpawnAt(new Structure(structure), new Vec2Int(4, 5));
        }

        public void SpawnAt(MapObjectInstanceBase obj, Vec2Int position)
        {
            var grid = GetGrid(obj.GetType());
            grid[XYToIndex(position)] = obj;
            obj.OnSpawn(this, position);

            GetList(obj.GetType()).Add(obj);
        }

        public void Despawn(MapObjectInstanceBase obj)
        {
            GetGrid(obj.GetType())[XYToIndex(obj.Position)] = null;
            GetList(obj.GetType()).Remove(obj);
        }

        private void FillTerrain()
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    //TODO hardcoded noise, this should receive arguments from outside which carry worldgen options
                    Terrain terrain = MapGenerator.GetTerrainAt(new Vec2Int(x,y), GameManager.Instance.MapGenSimplex);
                    terrainGrid[XYToIndex(x,y)] = terrain;
                }
            }
        }
    }
}