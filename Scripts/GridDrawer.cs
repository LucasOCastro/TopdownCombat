using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public class GridDrawer : Control
    {
        [Export]
        private Color externalColor = Colors.White, internalColor = Colors.White;
        [Export]
        private float externalWidth = 1f, internalWidth = 1f;


        //TODO link this with a delegate instead of calling directly (?)
        public void OnMapChanged(MapRenderer mapRenderer, Map newMap)
        {
            RectGlobalPosition = mapRenderer.Position;
            RectSize = Vector2.One * GameManager.GameScale * newMap.Size;
            Update();
        }
        #region TILE_HIGHLIGHTS
        private List<(Vec2Int position, Color color, float width)> highlightedTiles = new List<(Vec2Int position, Color color, float width)>();
        private bool drawnHighlights = false;
        private void HighlightTileWithoutUpdating(Vec2Int tile, Color? color = null, float? width = null)
        {
            highlightedTiles.Add((tile, color ?? internalColor, width ?? internalWidth));
        }
        /// <summary>Highlights a tile for the next Draw call. Call every frame to keep the tile highlighted.</summary>
        public void HighlightTile(Vec2Int tile, Color? color = null, float? width = null)
        {
            HighlightTileWithoutUpdating(tile, color ?? internalColor, width ?? internalWidth);
            Update();
        }
        /// <summary>Highlights multiple tiles for the next Draw call. Call every frame to keep the tiles highlighted.</summary>
        public void HighlightTiles(IEnumerable<Vec2Int> tiles, Color? color = null, float? width = null)
        {
            foreach (Vec2Int tile in tiles){
                HighlightTileWithoutUpdating(tile, color ?? internalColor, width ?? internalWidth);
            }
            Update();
        }
#endregion TILE_HIGHLIGHTS


        //Cleaning up of highlights and paths is done in physics process to ensure they are cleared AFTER drawing them.
        public override void _PhysicsProcess(float delta)
        {
            if (drawnHighlights)
            {
                highlightedTiles.Clear();
                Update();
                drawnHighlights = false;
            }
        }

        public override void _Draw()
        {
            Map map = GameManager.Instance?.LoadedMap;
            if (map == null){
                return;
            }

            Vector2 scale = Vector2.One * GameManager.GameScale;
            Vector2 startPos = Vector2.Zero;
            //TODO this is ugly
            for (int i = 0; i < map.Size - 1; i++)
            {
                Vector2 startV = new Vector2((i+1) * scale.x, startPos.y);
                Vector2 endV = startV + Vector2.Down * map.Size * scale;
                DrawLine(startV, endV, internalColor, internalWidth);

                Vector2 startH = new Vector2(startPos.x, (i+1) * scale.y);
                Vector2 endH = startH + Vector2.Right * map.Size * scale;
                DrawLine(startH, endH, internalColor, internalWidth);
            }

            Rect2 rect = new Rect2(startPos, map.Size * scale);
            DrawRect(rect, externalColor, filled: false, width: externalWidth);

            for (int i = 0; i < highlightedTiles.Count; i++)
            {
                drawnHighlights = true;
                var tuple = highlightedTiles[i];
                Rect2 tileRect = new Rect2((Vector2)tuple.position * scale, scale);
                DrawRect(tileRect, tuple.color, filled: false, tuple.width);
            }
        }
    }
}