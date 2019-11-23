using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Scenes;
using Game.Models.Chunks;
using System;

public class Player : IDrawableObject
{
    public MapChunk currentChunk;
    public event Action<Player, int, int> OnPlayerMovedToNewChunk;

    public Vector Position;
    private readonly SpriteSheetContext _sprite;

    private int framesBeforeSlowingDown = 5;
    private long slowedFrameIntervals = 50;

    private int jumpFrames = 25;
    private int jumpFrameIntervals = 5;
    private const float delayBetweenJumps = 500;
    private float dx;
    private float dy;
    private int jumpCount = 0;
    private long lastTimeMoved;

    private uint _joystickID;

    public Player(uint joystickID) {
        this._joystickID = joystickID;
        this.Position = Vector.Create();
        /*
        this._sprite = new TextureContext("Clawdia_FacingUpUp.png")
        {
            SourceTextureRect = new IntRect(0, 0, 32, 32)
        };
        */

        this._sprite = new SpriteSheetContext("smushy.png", 1, 8) {
            RenderPosition = this.Position,
            RenderSize = Vector.Create(25, 50)
        };

        EventManager.Singleton.AddEvent(PriorityType.INPUT, HandlePlayerInput, 10, 0, "KeyboardInput");
    }

    public void SetCurrentChunk(MapChunk chunk) {
        currentChunk = chunk;
    }

    public void Draw(ICanvas canvas)
    {
        canvas.Draw(this._sprite);
    }

    private ControlEvent HandlePlayerInput() {
        var canvas = GameWindow.Singleton.Canvas;

        var dx = canvas.GetJoystickAxis(this._joystickID, JoystickAxis.X);
        var dy = canvas.GetJoystickAxis(this._joystickID, JoystickAxis.Y);
        float magX = Math.Abs(dx);
        float magY = Math.Abs(dy);

        if (magX < 50 && magY < 50) {
            return ControlEvent.NONE;
        }

        long time = EventManager.CurrentTime;

        if (time > lastTimeMoved + delayBetweenJumps) {
            this.dx = dx;
            this.dy = dy;
            this.jumpCount = 0;
            var events = EventManager.Singleton;
            events.AddEvent(PriorityType.ANIMATION, Jump, jumpFrameIntervals, 0, "Jump");
            lastTimeMoved = time;

            events.AddEvent(PriorityType.ANIMATION, UpdateAnimation, (int)delayBetweenJumps / 10);
        }

        return ControlEvent.NONE;
    }

    private ControlEvent Jump() {
        jumpCount++;

        if (jumpCount == jumpFrames) {
            return ControlEvent.REMOVE;
        } else if (jumpCount >= jumpFrames - framesBeforeSlowingDown) {
            //var events = Events.GetEvent("Jump");
            var e = EventManager.Singleton.GetEvent("Jump");
            e.SetInterval(slowedFrameIntervals);
        }

        var canvas = GameWindow.Singleton.Canvas;
        float speed = 2;
        float signX = (this.dx / 100) * speed;
        float signY = (this.dy / 100) * speed;
        this.Position.Add(signX * speed, signY * speed);
        HasMovedToNewChunk();
        return ControlEvent.NONE;
    }

    private ControlEvent UpdateAnimation() {
        this._sprite.StepColumn();

        if (this._sprite.Column == 0) {
            return ControlEvent.REMOVE;
        }
        return ControlEvent.NONE;
    }

    public void HasMovedToNewChunk()
    {
        int curChunkX = (int)MathF.Floor(Position.X / (MapChunk.ChunkWidth));
        int curChunkY = (int)MathF.Floor(Position.Y / (MapChunk.ChunkHeight));

        if(curChunkX != currentChunk?.X || curChunkY != currentChunk?.Y)
        {
            OnPlayerMovedToNewChunk?.Invoke(this, curChunkX, curChunkY);
        }        
    }
}