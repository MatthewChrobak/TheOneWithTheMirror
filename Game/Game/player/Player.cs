using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;

public class Player : IDrawableObject
{
    public Vector Position;
    //private readonly TextureContext _sprite;
    private readonly SpriteSheetContext _sprite;

    private int framesBeforeSlowingDown = 5;
    private long slowedFrameIntervals = 50;

    private int jumpFrames = 25;
    private int jumpFrameIntervals = 5;
    private float delayBetweenJumps = 750;

    private int jumpCount = 0;
    private long lastTimeMoved;

    public Player()
    {
        this.Position = Vector.Create();
        /*
        this._sprite = new TextureContext("Clawdia_FacingUpUp.png")
        {
            SourceTextureRect = new IntRect(0, 0, 32, 32)
        };
        */

        this._sprite = new SpriteSheetContext("player.png", 4, 4)
        {
            RenderPosition = this.Position
        };

        EventManager.Singleton.AddEvent(PriorityType.INPUT, HandlePlayerInput, 10, 0, "KeyboardInput");
    }

    public void Draw(ICanvas canvas)
    {
        canvas.Draw(this._sprite);
    }

    private ControlEvent HandlePlayerInput()
    {
        long time = EventManager.CurrentTime;

        if (time > lastTimeMoved + delayBetweenJumps)
        {
            this.jumpCount = 0;
            var events = EventManager.Singleton;
            events.AddEvent(PriorityType.ANIMATION, Jump, jumpFrameIntervals, 0,"Jump");
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
            //var events = Events.GetEvent("Jump");
            var e = EventManager.Singleton.GetEvent("Jump");
            e.SetInterval(slowedFrameIntervals);
        }

        var canvas = GameWindow.Singleton.Canvas;
        float speed = 1;
        if (canvas.IsKeyDown(KeyboardKey.Up))
        {
            this.Position.Add(0, -speed);
        }
        if (canvas.IsKeyDown(KeyboardKey.Down))
        {
            this.Position.Y += speed;
        }
        if (canvas.IsKeyDown(KeyboardKey.Left))
        {
            this.Position.X -= speed;
        }
        if (canvas.IsKeyDown(KeyboardKey.Right))
        {
            this.Position.X += speed;
        }
        return ControlEvent.NONE;
    }
}