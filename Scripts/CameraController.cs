using Godot;
using System;

public class CameraController : Camera2D
{
    [Export]
    private float movementSpeed = 10;

    [Export]
    private float deltaZoom = .05f;

    [Export]
    private float minZoom = 0.1f, maxZoom = 2f, startingZoom = 1;

    private float currentZoom;
    
    public override void _Ready()
    {
        SetZoom(startingZoom);
    }

    public override void _Process(float delta)
    {
        float x = Input.GetAxis("left", "right");
        float y = Input.GetAxis("up", "down");
        Vector2 translation = new Vector2(x,y) * movementSpeed * delta;
        Transform = Transform.Translated(translation);
    }

    public void SetZoom(float zoom)
    {
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        Zoom = Vector2.One * zoom;
        currentZoom = zoom;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        ProcessZoomInput(@event);
    }

    private void ProcessZoomInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton button)
        {
            if (button.IsActionReleased("zoom_up")){
                SetZoom(currentZoom - deltaZoom);
            }
            else if (button.IsActionReleased("zoom_down")){
                SetZoom(currentZoom + deltaZoom);
            }
            return;
        }

        if (@event is InputEventKey)
        {
            if (@event.GetActionStrength("zoom_up") > 0){
                SetZoom(currentZoom - deltaZoom);
            }
            else if (@event.GetActionStrength("zoom_down") > 0){
                SetZoom(currentZoom + deltaZoom);
            }
        }
    }
}
