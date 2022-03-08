using Godot;
namespace CombatGame
{
    public class MapObjectResource : Resource
    {
        [Export]
        public string Label { get; private set; }

        [Export]
        public Texture Sprite { get; private set; }
    }
}