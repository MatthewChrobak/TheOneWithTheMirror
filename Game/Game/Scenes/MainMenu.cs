using Annex.Graphics;
using Annex.Graphics.Events;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models;
using Game.Scenes.Stage1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Annex.Events;
using Game.Scenes.MainMenu.Elements;
using Annex.Audio;

namespace Game.Scenes.MainMenu
{
    public class MainMenu : Scene
    {
        public readonly GrassyPlain plain;
        public readonly Player player;
        public List<Enemy> EnemyList;
        public const int playerDisplacement = 10;
        public const int enemyDisplacement = 1;
        public const int timerInterval = 100;
        public const int maximumEnemy = 100;
        public int enemyCounter = 0;

        public MainMenu()
        {

            var audio = AudioManager.Singleton;
            audio.PlayBufferedAudio("AwesomeMusic.flac", "test", true, 100);

            this.AddChild(new SplashTitle());
            var subTitle = new SubTitle();

            this.Events.AddEvent("", PriorityType.ANIMATION, () =>
            {
                subTitle.Visible = !subTitle.Visible;
                return ControlEvent.NONE;
            }, 500, 500);

            this.AddChild(subTitle);


            this.plain = new GrassyPlain();
            this.player = new Player(5);


            var enemyList = new List<Enemy>();
            
            //Add the first enemy
            var t = new Enemy();
            enemyList.Add(t);
       

            var task = Task.Run(async () =>
            {
                for (; ; )
                {
                    if (enemyCounter < maximumEnemy)
                    {
                        var t = new Enemy();
                        enemyList.Add(t);
                        enemyCounter++;
                    }

                    await Task.Delay(timerInterval);
                    Random random = new Random();
                    int randomNumber = random.Next(0, 3);

                    if (randomNumber == 0 && this.player.Position.X < 600 && this.player.Position.Y < 600)
                    {
                        this.player.Position.X += playerDisplacement;
                        this.player.Position.Y += playerDisplacement;
                    }
                    else if (randomNumber == 1 && this.player.Position.X < 600)
                    {
                        this.player.Position.X += playerDisplacement;
                        this.player.Position.Y -= playerDisplacement;
                    }
                    else if (randomNumber == 2 && this.player.Position.Y < 600)
                    {
                        this.player.Position.X -= playerDisplacement;
                        this.player.Position.Y += playerDisplacement;
                    }
                    else if (randomNumber == 3)
                    {
                        this.player.Position.X -= playerDisplacement;
                        this.player.Position.Y -= playerDisplacement;
                    }
                    else
                    {
                        this.player.Position.X -= playerDisplacement;
                        this.player.Position.Y -= playerDisplacement;
                    }



                    foreach (var enemy in enemyList)
                    {
                        if (player.Position.X > enemy.Position.X)
                        {
                            enemy.Position.X += enemy.enemyMovementSpeed;
                        }

                        if (player.Position.X < enemy.Position.X)
                        {
                            enemy.Position.X -= enemy.enemyMovementSpeed;
                        }

                        if (player.Position.Y > enemy.Position.Y)
                        {
                            enemy.Position.Y += enemy.enemyMovementSpeed;
                        }

                        if (player.Position.Y < enemy.Position.Y)
                        {
                            enemy.Position.Y -= enemy.enemyMovementSpeed;
                        }

                        if(player.Position.Y == enemy.Position.Y && player.Position.X == enemy.Position.X)
                        {
                            audio.PlayAudio("Sharp_Punch.flac");
                        }
                    }
                }
            });

            EnemyList = enemyList;
        }

        public override void HandleCloseButtonPressed()
        {
            SceneManager.Singleton.LoadScene<GameClosing>();

        }

        public override void Draw(ICanvas canvas)
        {
            plain.Draw(canvas);
            player.Draw(canvas);

            foreach (var enemy in EnemyList.ToList())
            {
                enemy.Draw(canvas);
            }
        }

        public override void HandleJoystickButtonPressed(JoystickButtonPressedEvent e)
        {
            if (e.Button == JoystickButton.A)
            {
                SceneManager.Singleton.LoadScene<Stage1.Stage1>();
            }
            if (e.Button == JoystickButton.Back)
            {
                this.HandleCloseButtonPressed();
            }
        }

    }
}