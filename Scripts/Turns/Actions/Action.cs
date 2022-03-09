namespace CombatGame
{
    public abstract class Action
    {
        public Entity Doer { get; }
        public Map Map => Doer.CurrentMap;
        //TODO unsure about cost being passed in the constructor of only this and not derived
        public int Cost { get; }
        public Action(Entity doer, int cost)
        {
            Doer = doer;
            Cost = cost;
        }

        /// <summary>Ticks while the action has already been started.</summary>
        /// <returns>True if action should end.</returns>
        public abstract bool Tick(float delta);
    }
}