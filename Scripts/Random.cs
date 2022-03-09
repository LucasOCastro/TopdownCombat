using RNG = Godot.RandomNumberGenerator;

namespace CombatGame
{
    public static class Random
    {
        private static RNG _randomNumberGenerator;
        public static RNG RNG
        {
            get
            {
                if (_randomNumberGenerator == null)
                {
                    _randomNumberGenerator = new RNG();
                    _randomNumberGenerator.Randomize();
                }
                return _randomNumberGenerator;
            }
        }

        /// <returns>Returns random in range 0~1 (inclusive).</returns>
        public static float Randf() => RNG.Randf();
        /// <returns>Returns random in range (inclusive).</returns>
        public static float RandfRange(float min, float max) => RNG.RandfRange(min, max);

        /// <returns>Returns random in range 0~1 (inclusive).</returns>
        public static uint Randi() => RNG.Randi();
        /// <returns>Returns random in range (inclusive).</returns>
        public static int RandiRange(int min, int max) => RNG.RandiRange(min, max);

        public static bool Bool() => RNG.RandiRange(0, 1) == 0;
        public static bool Chance(float chance) => Randf() < chance;

        /// <returns>Returns 1 or -1.</returns>
        public static int RandSignNoZero() => Bool() ? 1 : -1;
        /// <returns>Returns 0, 1 or -1.</returns>
        public static int RandSign() => Bool() ? 0 : RandSignNoZero();        
    }
}