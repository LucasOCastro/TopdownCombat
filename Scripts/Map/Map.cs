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

        private Entity[] entityGrid;
        private Terrain[] terrainGrid;
        public Terrain TerrainAt(Vec2Int xy) => terrainGrid[XYToIndex(xy)];
        public Entity EntityAt(Vec2Int xy) => entityGrid[XYToIndex(xy)];

        public List<Entity> AllEntities { get; } = new List<Entity>();
        private Dictionary<Faction, List<Entity>> entitiesByFaction = new Dictionary<Faction, List<Entity>>();
        public List<Entity> EntitiesOfFaction(Faction faction) => entitiesByFaction.GetOrAddNew(faction);

        public void UpdateEntityPosition(Entity entity, Vec2Int from, Vec2Int to)
        {
            int fromIndex = XYToIndex(from);
            if (entityGrid[fromIndex] == entity) entityGrid[fromIndex] = null;
            entityGrid[XYToIndex(to)] = entity;

            if (entity.Position != to) entity.Position = to;
        }

        public Vector2 GetTileCenter(Vec2Int tile) => ((Vector2)tile + Vector2.One * .5f) * GameManager.GameScale;

        public Vec2Int GetTile(Vector2 worldPos) => Vec2Int.Floor(worldPos / GameManager.GameScale);

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
            int area = Size * Size;
            entityGrid = new Entity[area];
            terrainGrid = new Terrain[area];

            FillTerrain();
        }

        public void SpawnEntity(Entity entity, Vec2Int position)
        {
            entityGrid[XYToIndex(position)] = entity;
            entity.OnSpawn(this, position);

            AllEntities.Add(entity);
            List<Entity> factionList = entitiesByFaction.GetOrAddNew(entity.Faction);
            factionList.Add(entity);
        }

        public void RemoveEntity(Entity entity)
        {
            entityGrid[XYToIndex(entity.Position)] = null;

            AllEntities.Remove(entity);
            List<Entity> factionList = entitiesByFaction[entity.Faction];
            factionList.Remove(entity);
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