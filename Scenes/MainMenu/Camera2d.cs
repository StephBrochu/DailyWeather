using Godot;
using System;

public partial class Camera2d : Camera2D
{

    /// <summary>
    /// Create for a tilemap background, but can be altered to anything with a rect size
    /// </summary>
    //[Export]
    //public TileMap BoundsRef { get; set; }

    public static Vector2 MainCameraZoom = Vector2.One;

    private Vector2 _startPos;
    private Vector2 _mousePos;
    private Vector2 _minZoom = new Vector2(0.4f, 0.4f);
    private Vector2 _maxZoom = new Vector2(1f, 1f);
    private Vector2 _zoomAmount = new Vector2(0.05f, 0.05f);

    /// <summary>
    /// These inputs need to be set up in project settings
    /// </summary>
    private const string ScreenDragInput = "drag";
    /// <summary>
    /// These inputs need to be set up in project settings
    /// </summary>
    private const string ScrollWheelUpInput = "scroll_up";
    /// <summary>
    /// These inputs need to be set up in project settings
    /// </summary>
    private const string ScrollWheelDownInput = "scroll_down";

    bool _dragging = false;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(ScreenDragInput))
        {
            GD.Print("Dragging triggered");
            if (@event is InputEventMouseButton eventMouse)
            {
                if (eventMouse.IsPressed())
                {
                    _startPos = Position;
                    _mousePos = eventMouse.Position;
                    _dragging = true;
                }
                else _dragging = false;
            }
        }
        else if (@event is InputEventMouseMotion eventMouseM && _dragging)
        {
            Vector2 newPos = (((_mousePos - eventMouseM.Position) / Zoom) + _startPos);
            //ClampCameraBounds(newPos);
        }

        if (@event.IsActionReleased(ScrollWheelUpInput))
        {
            Zoom += _zoomAmount;
            Zoom = Zoom.Clamp(_minZoom, _maxZoom);
            MainCameraZoom = Zoom;
            //ClampCameraBounds(Position);
        }
        else if (@event.IsActionReleased(ScrollWheelDownInput))
        {
            Zoom -= _zoomAmount;
            Zoom = Zoom.Clamp(_minZoom, _maxZoom);
            MainCameraZoom = Zoom;
            //ClampCameraBounds(Position);
        }
    }

    /*private void ClampCameraBounds(Vector2 position)
    {
        Vector2 newPos = position;

        if (BoundsRef != null)
        {
            Rect2 bounds = BoundsRef.GetUsedRect();
            Vector2 cameraCenterOffset = ((GetViewportRect().Size / 2) / Zoom);
            newPos = newPos.Clamp(bounds.Position * BoundsRef.CellQuadrantSize + cameraCenterOffset, (bounds.End * BoundsRef.CellQuadrantSize) - (GetViewportRect().End / Zoom) + cameraCenterOffset);
        }

        Position = newPos;
    }*/
}