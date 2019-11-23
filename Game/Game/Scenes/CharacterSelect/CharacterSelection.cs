using Annex.Data.Shared;
using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Contexts;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Scenes.CharacterSelect
{
    class CharacterSelection : Scene
    {
        private float controllerDeadzone = 50f;
        private SpriteSheetContext[] characters = new SpriteSheetContext[4];
        
        private int index = 0;
        public SpriteSheetContext selectedCharacter;        

        public Player EditingPlayer;

        public CharacterSelection()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                characters[i] = new SpriteSheetContext("smushy.png", 1, 8)
                {
                    //RenderPosition = Vector.Create(GameWindow.RESOLUTION_WIDTH/2, GameWindow.RESOLUTION_HEIGHT/2),
                    RenderSize = Vector.Create(25, 50)
                };
            }
            characters[1] = new SpriteSheetContext("player.png", 4, 4)
            {
                //RenderPosition = Vector.Create(GameWindow.RESOLUTION_WIDTH / 2, GameWindow.RESOLUTION_HEIGHT / 2),
                RenderSize = Vector.Create(96, 96)
            };

            characters[2] = new SpriteSheetContext("Clawdia_FacingUpUp.png", 1, 1)
            {
                //RenderPosition = Vector.Create(GameWindow.RESOLUTION_WIDTH / 2, GameWindow.RESOLUTION_HEIGHT / 2),
                RenderSize = Vector.Create(32, 32)
            };
            
            this.Events.AddEvent("ChooseCharacter", PriorityType.INPUT, ChooseCharacter, 500);
        }

        public ControlEvent ChooseCharacter()
        {
            var canvas = GameWindow.Singleton.Canvas;            

            var characterNotification = this.GetElementById(CharacterSelect.Elements.CharacterSelect.ID);
            if (characterNotification == null)
            {
                this.AddChild(new CharacterSelect.Elements.CharacterSelect());
            }
            
            var dx = canvas.GetJoystickAxis(EditingPlayer._joystickID, JoystickAxis.X);

            if (dx > 0 + controllerDeadzone)
            {
                index = (index + 1) % characters.Length;

            }
            else if (dx < 0 - controllerDeadzone)
            {
                index = (index + characters.Length - 1) % characters.Length;
            }            

            return ControlEvent.NONE;
        }        

        public override void HandleCloseButtonPressed()
        {
            SceneManager.Singleton.LoadScene<GameClosing>();
        }

        public override void HandleJoystickButtonPressed(JoystickButtonPressedEvent e)
        {
            if (e.Button == JoystickButton.A)
            {
                selectedCharacter = characters[index];
                EditingPlayer._sprite = new SpriteSheetContext(selectedCharacter.SourceTextureName, (uint)selectedCharacter.NumRows, (uint)selectedCharacter.NumColumns)
                {
                    RenderPosition = EditingPlayer.Position,
                    RenderSize = Vector.Create(25, 50)
                };
                SceneManager.Singleton.LoadScene<Stage1.Stage1>();
            }
            if (e.Button == JoystickButton.Back)
            {
                this.HandleCloseButtonPressed();
            }
        }

        public override void Draw(ICanvas canvas)
        {
            canvas.Draw(characters[index]);                
            base.Draw(canvas);
        }
    }
}
