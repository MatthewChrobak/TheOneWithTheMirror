using System;

namespace Game
{
    public static class RNG
    {
        private static Random _rng = new Random();

        public static int Next(int upperbound) {
            return _rng.Next(upperbound);
        }
    }
}
