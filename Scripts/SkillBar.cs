using Godot;
using System.Collections.Generic;

namespace CombatGame
{
    public class SkillBar : Control
    {
        private class ActionCallbackHandler : ColorRect
        {
            private ActionAimer aimer;
            private ActionClickedCallback callback;
            public ActionCallbackHandler(ActionAimer aimer, ActionClickedCallback clickCallback){
                this.aimer = aimer;
                callback = clickCallback;
            }

            public void GuiInput(InputEvent @event)
            {
                if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == (int)ButtonList.Left)
                {
                    callback?.Invoke(aimer);
                }
            }
        }

        [Export]
        private NodePath containerPath;
        private HBoxContainer container;

        public delegate void ActionClickedCallback(ActionAimer selectedAction);
        public ActionClickedCallback onActionClicked;

        public override void _Ready()
        {
            container = GetNode<HBoxContainer>(containerPath);
            SetActions(null, null);
        }

        private List<ActionAimer> currentActions;
        private Entity currentEntity;
        public void SetActions(List<ActionAimer> actions, Entity entity)
        {
            currentActions = actions;
            currentEntity = entity;
            Clear();
            if (actions == null){
                return;
            }

            for (int i = 0; i < actions.Count; i++)
            {
                ActionAimer actionAimer = actions[i];

                ActionCallbackHandler rect = new ActionCallbackHandler(actionAimer, onActionClicked);
                rect.RectMinSize = new Vector2(64, 64);
                rect.HintTooltip = actionAimer.GetType().Name;
                rect.MouseFilter = MouseFilterEnum.Stop;

                container.AddChild(rect);
                RefreshAt(i);
            }
        }

        private void RefreshAt(int i)
        {
            var action = currentActions[i];

                bool canDo = action.CanCurrentlyBeSelected();
                ActionCallbackHandler colorRect = container.GetChild<ActionCallbackHandler>(i);
                colorRect.Color = canDo ? Colors.Green : Colors.Red;

                //TODO probably allocate this task to the ActionCallbackHandler class?
                bool connected = colorRect.IsConnected("gui_input", colorRect, nameof(colorRect.GuiInput));
                if (connected && !canDo){
                    colorRect.Disconnect("gui_input", colorRect, nameof(colorRect.GuiInput));
                }
                else if (!connected && canDo){
                    colorRect.Connect("gui_input", colorRect, nameof(colorRect.GuiInput));
                }
        }

        //TODO this is horrible
        public void RefreshAll()
        {
            for (int i = 0; i < container.GetChildCount(); i++)
            {
                RefreshAt(i);
            }
        }


        private void Clear()
        {
            for (int i = 0; i < container.GetChildCount(); i++)
            {
                container.GetChild(i).QueueFree();
            }
        }
    }
}