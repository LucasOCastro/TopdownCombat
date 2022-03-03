using Godot;

namespace CombatGame
{
    public abstract class ActionAimer
    {
        public Entity ActionDoer { get; }
        public ActionAimer(Entity doer)
        {
            ActionDoer = doer;
        }

        public abstract Action Tick(float delta);
        public abstract Action Input(InputEvent @event);
        public abstract void Clear();

        public abstract bool CanCurrentlyBeSelected();
    }
}