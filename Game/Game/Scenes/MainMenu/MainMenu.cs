﻿using Annex.Events;
using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Scenes.MainMenu.Elements;

namespace Game.Scenes.MainMenu
{
    public class MainMenu : Scene
    {
        public MainMenu() {
            this.ImageTextureName.Set("splash.png");
            this.Size.Set(GameWindow.RESOLUTION_WIDTH, GameWindow.RESOLUTION_HEIGHT);

            var subTitle = new SubTitle();

            this.Events.AddEvent("", PriorityType.ANIMATION, () => {
                subTitle.Visible = !subTitle.Visible;
                return ControlEvent.NONE;                
            }, 500, 500);

            this.AddChild(subTitle);
        }

        public override void HandleCloseButtonPressed() {
            SceneManager.Singleton.LoadScene<GameClosing>();
        }

        public override void HandleJoystickButtonPressed(JoystickButtonPressedEvent e) {
            if (e.Button == JoystickButton.A) {
                SceneManager.Singleton.LoadScene<Stage1.Stage1>(true);
            }
            if (e.Button == JoystickButton.Back) {
                this.HandleCloseButtonPressed();
            }
        }

        public override void HandleKeyboardKeyPressed(KeyboardKeyPressedEvent e) {
            if (e.Key == KeyboardKey.Space) {
                HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.A
                });
            }
            if (e.Key == KeyboardKey.Escape) {
                HandleJoystickButtonPressed(new JoystickButtonPressedEvent() {
                    Button = JoystickButton.Back
                });
            }
        }
    }
}
