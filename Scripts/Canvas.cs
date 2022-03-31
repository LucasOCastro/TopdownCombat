using Godot;

namespace CombatGame
{
    public class Canvas : CanvasLayer
    {
        [Export]
        private NodePath skillbarPath;
        public SkillBar SkillBar => GetNode<SkillBar>(skillbarPath);
    }
}