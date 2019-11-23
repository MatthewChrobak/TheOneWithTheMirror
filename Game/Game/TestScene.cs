using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;

public class TestScene : Scene
{
    private readonly Player _player;
    /*
    private int framesBeforeSlowingDown = 5;
    private long slowedFrameIntervals = 50;

    private int jumpFrames = 25;
    private int jumpFrameIntervals = 5;
    private float delayBetweenJumps = 750;

    private int jumpCount = 0;
    private long lastTimeMoved;
    */
    public TestScene()
    {
        this._player = new Player();

        var camera = GameWindow.Singleton.Canvas.GetCamera();
        //camera.Follow(this._player.Position);        

        //lastTimeMoved = EventManager.CurrentTime;
    }

    public override void HandleCloseButtonPressed()
    {
        SceneManager.Singleton.LoadScene<GameClosing>();
    }

    public override void Draw(ICanvas canvas)
    {
        // It's important that the player gets drawn /after/ the plain does.
        // Otherwise, the plain would be drawn over the player.
        this._player.Draw(canvas);
    }

    /*
    public override void HandleJoystickMoved(JoystickMovedEvent e)
    {
        base.HandleJoystickMoved(e);
        
        switch (e.Axis)
        {
            case JoystickAxis.X:
                break;
            case JoystickAxis.Y:
                break;
            case JoystickAxis.Z:
                break;
            case JoystickAxis.R:
                break;
            case JoystickAxis.U:
                break;
            case JoystickAxis.V:
                break;
            case JoystickAxis.PovX:
                break;
            case JoystickAxis.PovY:
                break;
        }
    }
    
    private ControlEvent HandlePlayerInput()
    {
        long time = EventManager.CurrentTime;

        if (time > lastTimeMoved + delayBetweenJumps)
        {
            this.jumpCount = 0;
            this.Events.AddEvent("Jump", PriorityType.ANIMATION, Jump, jumpFrameIntervals);
            lastTimeMoved = time;
        }

        return ControlEvent.NONE;
    }

    private ControlEvent Jump()
    {
        jumpCount++;

        if (jumpCount == jumpFrames)
        {
            return ControlEvent.REMOVE;
        }
        else if (jumpCount >= jumpFrames - framesBeforeSlowingDown)
        {
            var e = Events.GetEvent("Jump");
            e.SetInterval(slowedFrameIntervals);
        }

        var canvas = GameWindow.Singleton.Canvas;
        float speed = 1;
        if (canvas.IsKeyDown(KeyboardKey.Up))
        {
            this._player.Position.Add(0, -speed);
        }
        if (canvas.IsKeyDown(KeyboardKey.Down))
        {
            this._player.Position.Y += speed;
        }
        if (canvas.IsKeyDown(KeyboardKey.Left))
        {
            this._player.Position.X -= speed;
        }
        if (canvas.IsKeyDown(KeyboardKey.Right))
        {
            this._player.Position.X += speed;
        }
        return ControlEvent.NONE;
    }
    */
}