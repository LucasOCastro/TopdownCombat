using Godot;
using System.Collections.Generic;
using System.Linq;

namespace CombatGame
{
    public abstract class Turn
    {
        public Faction Faction { get; }

        protected Action CurrentAction { get; private set; }
        public Entity CurrentEntity { get; protected set; }

        protected List<Entity> AvailableFactionEntities => GameManager.Instance.LoadedMap.EntitiesOfFaction(Faction);
        protected IEnumerable<Entity> AvailableEntitiesWithAP => AvailableFactionEntities.Where(e => e.ActionPoints > 0);

        private Dictionary<Entity, List<ActionAimer>> entityActionsCache = new Dictionary<Entity, List<ActionAimer>>();
        protected List<ActionAimer> CurrentEntityActions
        {
            get
            {
                if (CurrentEntity == null){
                    return null;
                }
                if (!entityActionsCache.TryGetValue(CurrentEntity, out var actionAimers)){
                    actionAimers = CurrentEntity.GenerateActions();
                    entityActionsCache.Add(CurrentEntity, actionAimers);
                }
                return actionAimers;
            }
        }

        public Turn(Faction faction)
        {
            Faction = faction;

        }

        public virtual void Start()
        {
            entityActionsCache.Clear();
            foreach (var unit in AvailableFactionEntities){
                unit.MaxOutActionPoints();
            }
        }

        public virtual void End()
        {
            CurrentEntity = null;
            CurrentAction = null;
            entityActionsCache.Clear();
        }

        /// <returns>True if turn should end.</returns>
        public virtual bool Tick(float delta)
        {
            if (CurrentAction != null)
            {
                bool shouldEnd = CurrentAction.Tick(delta);
                if (shouldEnd){
                    StartAction(null);
                }
            }
            return false;
        }
        /// <returns>True if turn should end.</returns>
        public virtual bool Input(InputEvent @event)
        {
            return false;
        }


        protected virtual void StartAction(Action action)
        {
            if (action == null){
                CurrentAction = null;
                return;
            }

            if (action.Cost > CurrentEntity.ActionPoints)
            {
                throw new System.Exception($"Tried to start action of type {action.GetType().Name} that is more expensive than available action points ({action.Cost}>{CurrentEntity.ActionPoints}).");
            }
            CurrentAction = action;
            CurrentEntity.ActionPoints -= action.Cost;
        }
    }
}