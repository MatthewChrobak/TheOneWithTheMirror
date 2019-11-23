using Annex.Graphics;
using Annex.Scenes;
using Annex.Scenes.Components;
using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class MainMenu : Scene
{
    public readonly GrassyPlain plain;
    public readonly Player player;
    public static readonly TimeSpan InfiniteTimeSpan;
    public List<Enemy> EnemyList;
    public const int playerDisplacement = 10;
    public const int enemyDisplacement =1;
    public const int timerInterval = 100;

    public MainMenu () {

        this.plain = new GrassyPlain();
        this.player = new Player();


        var enemyList = new List<Enemy>();

        //for (var i = 0; i < 200; i++)
        //{
        //    var enemy = new Enemy();
        //    enemyList.Add(enemy);
        //}

        var task = Task.Run(async () => {
            for (; ; )
            {

                var t = new Enemy();
                enemyList.Add(t);

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

}