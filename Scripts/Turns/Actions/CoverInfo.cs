
namespace CombatGame
{
    public struct CoverInfo
    {
        public Structure coverProvider;
        public float coverStrength;
        public CoverInfo(Structure cover, float strength)
        {
            coverProvider = cover;
            coverStrength = strength;
        }
    }
}