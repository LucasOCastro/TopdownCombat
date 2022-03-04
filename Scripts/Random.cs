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

        public static float Randf() => RNG.Randf();
        /// <returns>Returns random in range (inclusive).</returns>
        public static float RandfRange(float min, float max) => RNG.RandfRange(min, max);

        public static uint Randi() => RNG.Randi();
        /// <returns>Returns random in range (inclusive).</returns>
        public static int RandiRange(int min, int max) => RNG.RandiRange(min, max);

        public static bool RandBool() => RNG.RandiRange(0, 1) == 0;

        /// <returns>Returns 1 or -1.</returns>
        public static int RandSign() => RandBool() ? 1 : -1;

        /// <returns>Returns 0, 1 or -1.</returns>
        public static int RandSignZero() => RandBool() ? 0 : RandSign();

        /// <returns>Returns the value with the same or opposite sign.</returns>
        public static float RandomlySigned(float val) => val * RandSign();

        
    }
}