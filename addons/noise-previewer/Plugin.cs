using Godot;

namespace NoisePreviewer
{
    [Tool]
    public class Plugin : EditorPlugin
    {
        NoiseInspector inspector = new NoiseInspector();
        public override void _EnterTree()
        {
            AddInspectorPlugin(inspector);
        }

        public override void _ExitTree()
        {
            RemoveInspectorPlugin(inspector);
        }
    }
}
    