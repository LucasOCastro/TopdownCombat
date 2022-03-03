using System.Collections.Generic;
using Godot;

namespace CombatGame
{
    public class MoveActionAimer : ActionAimer
    {
        private Path fullPreviewPath;
        private Path currentPreviewPath;
        private List<Vec2Int> highlightedPathTiles = new List<Vec2Int>();

        public MoveActionAimer(Entity doer) : base(doer)
        {
        }

        public override bool CanCurrentlyBeSelected() => ActionDoer.ActionPoints > PathGen.BASE_COST;

        private Path CalculatePreviewPath(Vec2Int mouseTile, Map map)
        {
            if (!map.InBounds(mouseTile) || (fullPreviewPath != null && mouseTile == fullPreviewPath.End) || !PathGen.IsStandableFor(ActionDoer, mouseTile, map))
            {
                return null;
            }
            if (currentPreviewPath != null && currentPreviewPath.End == mouseTile)
            {
                return currentPreviewPath;
            }
            int currentFullCost = fullPreviewPath?.PathCost ?? 0;
            if (currentFullCost == ActionDoer.ActionPoints)
            {
                return null;
            }
            Vec2Int pathStart = fullPreviewPath?.End ?? ActionDoer.Position;
            Path path = PathGen.FindPath(pathStart, mouseTile, map, ActionDoer);
            if (path == null)
            {
                return null;
            }

            
            int fullCost = currentFullCost + path.PathCost;
            if (fullCost > ActionDoer.ActionPoints){
                path = Path.LimitCost(path, ActionDoer.ActionPoints - currentFullCost);
            }
            return path;
        }

        public override Action Tick(float delta)
        {
            if (highlightedPathTiles.Count > 0)
            {
                GameManager.Instance.GridDrawer.HighlightTiles(highlightedPathTiles, Colors.LightBlue);
            }
            
            Map map = GameManager.Instance.LoadedMap;
            Vec2Int mouseTile = map.GetMousePosition();
            currentPreviewPath = CalculatePreviewPath(mouseTile, map);

            if (fullPreviewPath != null || currentPreviewPath != null){
                GameManager.Instance.PathDrawer.DrawConnectedPaths(fullPreviewPath, currentPreviewPath);
            }


            if (mouseTile != (currentPreviewPath?.End ?? fullPreviewPath?.End)){
                GameManager.Instance.GridDrawer.HighlightTile(mouseTile, Colors.Red, 1f);
                Vec2Int? actualEnd = currentPreviewPath?.End ?? fullPreviewPath?.End;
                if (actualEnd.HasValue){
                    GameManager.Instance.GridDrawer.HighlightTile(actualEnd.Value, Colors.White, 1f);
                }
            }

            return null;
        }

        public override Action Input(InputEvent @event)
        {
            if (!(@event is InputEventMouseButton mouse) || !mouse.Pressed){
                return null;
            }

            Map map = GameManager.Instance.LoadedMap;
            Vec2Int tile = map.GetMousePosition();
            if (!map.InBounds(tile)){
                return null;
            }

            int button = mouse.ButtonIndex;
            switch (button)
            {
                case (int)ButtonList.Right:
                    if (currentPreviewPath != null)
                    {
                        fullPreviewPath = Path.MergePaths(fullPreviewPath, currentPreviewPath);
                        highlightedPathTiles.Add(currentPreviewPath.End);
                        currentPreviewPath = null;
                    }
                    
                    if (!mouse.Shift && fullPreviewPath != null){
                        MoveAction moveAction = new MoveAction(ActionDoer, fullPreviewPath);
                        return moveAction;
                    }
                    break;
            }
            return null;
        }

        public override void Clear()
        {
            fullPreviewPath = null;
            currentPreviewPath = null;
            highlightedPathTiles.Clear();
        }
    }
}