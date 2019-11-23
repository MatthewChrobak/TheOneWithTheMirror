using System;
using Annex;

namespace Game
{
    public class Program
    {
        public static void Main(string[] args) {
            var game = new AnnexGame();

            game.Start<TestScene>();
        }
    }
}
