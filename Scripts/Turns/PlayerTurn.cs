using Godot;
using System.Linq;

namespace CombatGame
{
    public class PlayerTurn : Turn
    {
        private ActionAimer selectedActionAimer;

        private ActionAimer currentMoveAimer, currentShootAimer;
        protected override void StartAction(Action action)
        {
            base.StartAction(action);
            SelectAimer(null);
            GameManager.Instance.Canvas.GetNode<SkillBar>("SkillBar").RefreshAll();
        }

        public PlayerTurn() : base(Faction.Player){}


        private void SelectAimer(ActionAimer aimer)
        {
            selectedActionAimer?.Clear();
            selectedActionAimer = aimer;
        }
        public override void Start()
        {
            base.Start();
            //TODO getnode calls bad >:(
            GameManager.Instance.Canvas.GetNode<SkillBar>("SkillBar").onActionClicked += SelectAimer;
        }
        public override void End()
        {
            base.End();
            GameManager.Instance.Canvas.GetNode<SkillBar>("SkillBar").onActionClicked -= SelectAimer;
            GameManager.Instance.Canvas.GetNode<SkillBar>("SkillBar").SetActions(null, null);
        }

        public override bool Tick(float delta)
        {
            if (CurrentAction != null)
            {
                base.Tick(delta);
                return false;
            }

            //TODO i think this check should be in base class?
            if (!AvailableEntitiesWithAP.Any()){
                return true;
            }

            Map map = GameManager.Instance.LoadedMap;
            Vec2Int mouseTile = map.GetMousePosition();
            if (map.InBounds(mouseTile)){
                GameManager.Instance.GridDrawer.HighlightTile(mouseTile, Colors.White);
            }

            if (CurrentEntity != null){
                GameManager.Instance.GridDrawer.HighlightTile(CurrentEntity.Position, CurrentEntity.IsPlayer ? Colors.Blue : Colors.DarkRed, 2f);
            }
            
            if (selectedActionAimer != null)
            {
                Action actionToStart = selectedActionAimer.Tick(delta);
                if (actionToStart != null){
                    StartAction(actionToStart);
                }
            }

            if (CurrentAction is MoveAction moveAction)
            {
                GameManager.Instance.PathDrawer.DrawPath(moveAction.Path, moveAction.CurrentIndexInPath + 1);
            }
            return false;
        }

        private void Select(Entity entity)
        {
            SelectAimer(null);
            if (entity == CurrentEntity){
                return;
            }
            CurrentEntity = entity;

            var actionBar = GameManager.Instance.Canvas.GetNode<SkillBar>("SkillBar");
            if (CurrentEntity != null && CurrentEntity.IsPlayer)
            {
                actionBar.SetActions(CurrentEntityActions, entity);
                foreach (var action in CurrentEntityActions)
                {
                    if (action is MoveActionAimer moveAimer) currentMoveAimer = moveAimer;
                    else if (action is ShootActionAimer shootAimer) currentShootAimer = shootAimer;
                }
            }
            else
            {
                actionBar.SetActions(null, null);
                currentMoveAimer = null;
                currentShootAimer = null;
            }
        }

        public override bool Input(InputEvent @event)
        {
            if (CurrentAction != null) return false;

            //TODO hardcoded strings
            if (@event.IsActionPressed("end_turn")){
                return true;
            }

            if (selectedActionAimer != null){
                Action actionToStart = selectedActionAimer.Input(@event);
                if (actionToStart != null){
                    StartAction(actionToStart);
                }
            }

            if (@event is InputEventMouseButton mouse && mouse.Pressed)
            {
                Map map = GameManager.Instance.LoadedMap;
                Vec2Int tile = map.GetMousePosition();
                if (map.InBounds(tile)){
                    HandleClick(tile, mouse, map);
                }
            }
            return false;
        }
        
        //TODO possibly turn the index check into InputMap buttons
        private void HandleClick(Vec2Int tile, InputEventMouseButton buttonEvent, Map map)
        {
            int button = buttonEvent.ButtonIndex;
            switch (button)
            {
                case (int)ButtonList.Left:
                    Entity entity = map.GetAt<Entity>(tile);
                    Select(entity);
                    break;
                case (int)ButtonList.Right:
                    if (CurrentEntity == null || selectedActionAimer != null || CurrentEntity.Faction != Faction.Player){
                        break;
                    }

                    Entity entityAt = map.GetAt<Entity>(tile);
                    if (entityAt != null && currentShootAimer != null && currentShootAimer.CanCurrentlyBeSelected()){
                        selectedActionAimer = currentShootAimer;
                    }
                    if (entityAt == null && currentMoveAimer != null && PathGen.IsStandableFor(CurrentEntity, tile, map) && currentMoveAimer.CanCurrentlyBeSelected()){
                        selectedActionAimer = currentMoveAimer;
                    }
                    break;
            }
        }
    }
}