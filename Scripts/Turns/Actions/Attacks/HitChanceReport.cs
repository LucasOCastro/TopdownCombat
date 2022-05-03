using System.Collections.Generic;
using Mathf = Godot.Mathf;

namespace CombatGame
{
    public struct HitChanceReport
    {
        public WeaponBase weapon;
        public float distanceFallof;

        public List<CoverInfo> covers;
        public float totalCoverStrength;

        public float DistanceHitChance 
        {
            get
            {
                return Mathf.Clamp(weapon.Accuracy - distanceFallof, 0, 1);
            }
        }

        public float EstimatedAbsoluteHitChance
        {
            get 
            {
                return (1 - totalCoverStrength) * DistanceHitChance;
            }
        }
    }
}