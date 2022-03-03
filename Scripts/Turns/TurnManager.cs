namespace CombatGame
{
    public class TurnManager
    {
        private Turn playerTurn;
        private Turn enemyTurn;

        private Turn currentTurn;
        public TurnManager(Turn playerTurn, Turn enemyTurn)
        {
            this.playerTurn = playerTurn;
            this.enemyTurn = enemyTurn;
        }

        public void DebugStartPlayerTurn()
        {
            StartTurn(playerTurn);
        }

        private void StartTurn(Turn turn)
        {
            Godot.GD.Print("starting turn: " + turn.GetType().Name);
            currentTurn?.End();
            currentTurn = turn;
            turn?.Start();
        }

        private void CycleTurns()
        {
            if (currentTurn == playerTurn) StartTurn(enemyTurn);
            else StartTurn(playerTurn);
        }

        public void Tick(float delta)
        {
            bool ended = currentTurn.Tick(delta);
            if (ended){
                CycleTurns();
            }
        }

        public void Input(Godot.InputEvent @event)
        {
            bool ended = currentTurn.Input(@event);
            if (ended){
                CycleTurns();
            }
        }
    }
}