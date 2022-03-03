using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public class PathDrawer : Node
    {
        [Export]
        private Color defaultColor = Colors.SkyBlue;
        [Export]
        private float defaultWidth = 2f;

        private List<Path> drawnPaths = new List<Path>();
        private Dictionary<Path, (Line2D line, bool drawn)> drawnPathLines = new Dictionary<Path, (Line2D line, bool drawn)>();

        //possible pooling here
        private (Line2D line, bool drawn) DrawPathInternal(Path path, Vector2[] points, Color color, float width)
        {
            if (!drawnPathLines.TryGetValue(path, out var tuple))
            {
                tuple = (new Line2D(), true);
                tuple.line.JointMode = Line2D.LineJointMode.Round;
                tuple.line.Antialiased = true;
                AddChild(tuple.line);
                drawnPathLines.Add(path, tuple);
                drawnPaths.Add(path);
            }
            tuple = (tuple.line, true);
            drawnPathLines[path] = tuple;

            if (tuple.line.DefaultColor != color){
                tuple.line.DefaultColor = color;
            }

            if (tuple.line.Width != width){
                tuple.line.Width = width;
            }

            if (tuple.line.Points.Length != points.Length){
                tuple.line.Points = points;
            }

            return tuple;
        }

        public void DrawPath(Path path, int startingIndex = 0, Color? color = null, float? width = null)
        {
            if (startingIndex < 0 || startingIndex >= path.TileCount){
                throw new System.ArgumentOutOfRangeException(nameof(startingIndex));
            }

            Vector2[] points = new Vector2[path.TileCount - startingIndex];
            for (int i = 0; i < points.Length; i++){
                points[i] = GameManager.Instance.LoadedMap.GetTileCenter(path[i + startingIndex]);
            }
            DrawPathInternal(path, points, color ?? defaultColor, width ?? defaultWidth);
        }

        public void DrawConnectedPaths(Path a, Path b, int startingIndex = 0, Color? color = null, float? width = null)
        {
            if (a == null && b == null) {
                throw new System.Exception("Passed null path into " + nameof(DrawConnectedPaths));
            }

            if (a == null || b == null) {
                DrawPath(a ?? b, startingIndex, color, width);
                return;
            }

            DrawPath(a, startingIndex, color, width);

            Map map = GameManager.Instance.LoadedMap;
            Vector2[] points = new Vector2[b.TileCount + 1];
            points[0] = map.GetTileCenter(a.End);
            for (int i = 0; i < b.TileCount; i++){
                points[i + 1] = map.GetTileCenter(b[i]);
            }
            DrawPathInternal(b, points, color ?? defaultColor, width ?? defaultWidth);
        }

        private void ClearAt(int index)
        {
            Path path = drawnPaths[index];
            Line2D line = drawnPathLines[path].line;
            if (GDScript.IsInstanceValid(line) && !line.IsQueuedForDeletion()){
                RemoveChild(line);
                drawnPathLines[path].line.QueueFree();
            }
            
            drawnPathLines.Remove(path);
            drawnPaths.RemoveAt(index);
        }

        public override void _Process(float delta)
        {
            for (int i = drawnPaths.Count-1; i >= 0; i--)
            {
                var tuple = drawnPathLines[drawnPaths[i]];
                if (tuple.drawn){
                    drawnPathLines[drawnPaths[i]] = (tuple.line, false);
                    continue;
                }
                ClearAt(i);
            }
        }
    }
}