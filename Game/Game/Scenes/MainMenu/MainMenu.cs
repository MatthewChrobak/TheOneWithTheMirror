﻿using Annex.Events;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Scenes.MainMenu.Elements;

namespace Game.Scenes.MainMenu
{
    public class MainMenu : Scene
    {
        public MainMenu() {
            this.AddChild(new SplashTitle());
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
                SceneManager.Singleton.LoadScene<Stage1.Stage1>();
            }
            if (e.Button == JoystickButton.Back) {
                this.HandleCloseButtonPressed();
            }
        }
    }
}