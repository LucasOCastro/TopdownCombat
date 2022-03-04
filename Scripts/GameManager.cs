using Godot;
using System.Linq;

namespace CombatGame
{
	public class GameManager : Node
	{
        public static GameManager Instance { get; private set; }


        [Export]
		private float gameScale 
		{
			get => GameScale.x;
			set => GameScale = new Vector2(value, value);
        }
        public static Vector2 GameScale { get; private set; } = new Vector2(10, 10);

		[Export]
		private int mapSize;
        public Map LoadedMap { get; private set; }

        [Export]
		private NodePath mapRendererPath;
        public MapRenderer MapRenderer { get; private set; }

        [Export]
		private NodePath gridDrawerPath;
		public GridDrawer GridDrawer { get; private set; }
		[Export]
		private NodePath pathDrawerPath;
		public PathDrawer PathDrawer { get; private set; }

		[Export]
		public OpenSimplexNoise MapGenSimplex { get; private set; }

        [Export]
        private NodePath canvasPath;
		public CanvasLayer Canvas { get; private set; }

        public void LoadMap (Map map)
		{
			LoadedMap = map;
			MapRenderer.Draw(map);
			GridDrawer.OnMapChanged(MapRenderer, map);
		}

		public static void Instantiate(Node node)
		{
			//TODO needs fleshing out
            Instance.AddChild(node);
        }

		public static void InstantiateUI(Control node)
		{
            Instance.Canvas.AddChild(node);
        }

        private TurnManager turnManager;
        public override void _Ready()
		{
			Instance = this;
            ResourceLoader.LoadAll();

			MapRenderer = GetNode<MapRenderer>(mapRendererPath);
			MapRenderer.Scale = GameScale;
			//TODO hardcoded value
			MapRenderer.Init(32, ResourceLoader.GenTerrainsTexture(), ResourceLoader.GenTerrainsTextureArray());

			GridDrawer = GetNode<GridDrawer>(gridDrawerPath);
            PathDrawer = GetNode<PathDrawer>(pathDrawerPath);
            Canvas = GetNode<CanvasLayer>(canvasPath);

            LoadMap(new Map(mapSize));

			var entityBase = ResourceDatabase<EntityBase>.GetAny();
			Entity playerEntity = new Entity(entityBase, Faction.Player);
            playerEntity.EquippedWeapon = ResourceDatabase<Weapon>.GetAny();
            LoadedMap.SpawnEntity(playerEntity, new Vec2Int(0, 0));

            Entity enemyEntity = new Entity(entityBase, Faction.Enemy);
            LoadedMap.SpawnEntity(enemyEntity, new Vec2Int(5, 5));
            enemyEntity.Renderer.Modulate = Colors.Red;

            PlayerTurn playerTurn = new PlayerTurn();
            EnemyTurn enemyTurn = new EnemyTurn();
            turnManager = new TurnManager(playerTurn, enemyTurn);
            turnManager.DebugStartPlayerTurn();
        }

		public override void _Process(float delta)
		{
			turnManager.Tick(delta);
		}
		public override void _UnhandledInput(InputEvent @event)
		{
            turnManager.Input(@event);
		}
	}
}