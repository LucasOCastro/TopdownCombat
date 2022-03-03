namespace CombatGame
{
    public class EnemyTurn : Turn
    {
        public EnemyTurn() : base(Faction.Enemy){}

        public override void End()
        {
            base.End();
            timer = 0;
        }

        float timer = 0;
        public override bool Tick(float delta)
        {
            base.Tick(delta);

            timer += delta;
            return timer >= 1.3f;
        }
    }
}