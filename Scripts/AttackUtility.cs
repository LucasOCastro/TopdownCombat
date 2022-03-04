using Mathf = Godot.Mathf;

namespace CombatGame
{
    public static class AttackUtility
    {
        //Firing accuracy formula from : https://github.com/OpenXcom/OpenXcom/blob/eacdad08e08d2991cbd958c04f02d290e12a8227/src/Savegame/BattleUnit.cpp
        //accuracyStat * weaponAccuracy * kneelingbonus(1.15) * one-handPenalty(0.8) * woundsPenalty(% health) * critWoundsPenalty (-10%/wound)

        //Distance fallof formula from https://www.ufopaedia.org/index.php/Chance_to_Hit_(EU2012)
        //float distanceFallof = 42 - (4.5f * distance);

        //TODO study this better
        public static float CalculateRangedHitChance(Entity attacker, Weapon weapon, Vec2Int target)
        {
            float distance = attacker.Position.IntDistanceTo(target);
            // float distanceAccuracy = Mathf.Clamp(1f - (distance / attacker.MaximumEfficientShootDistance), 0, 1);
            float distanceAccuracy = 1f;
            Godot.GD.Print($"{distance}\\{attacker.MaximumEfficientShootDistance}");
            
            if (distance >= weapon.MinDistanceForFallof && distance <= weapon.MaxDistanceForFallof){
                float fallof = weapon.DistanceAccuracyFallof * distance;
                distanceAccuracy -= fallof;
                distanceAccuracy = Mathf.Clamp(distanceAccuracy, 0, 1);
            }
            
            float hitChance = weapon.Accuracy * distanceAccuracy;
            return hitChance;
        }

        public static Vec2Int CalculateMissedHitTile(Vec2Int origin, Vec2Int target, float hitChance, float rolledHit)
        {
            // float missStrength = rolledHit - hitChance;

            // float meanX = Random.RandomlySigned(missStrength);
            // var randX = Random.RNG.Randfn(meanX, 1);
            // int xOffset = Mathf.CeilToInt(Mathf.Abs(randX)) * Mathf.Sign(randX);

            // float meanY = Random.RandomlySigned(missStrength);
            // var randY = Random.RNG.Randfn(meanY, 1);
            // int yOffset = Mathf.CeilToInt(Mathf.Abs(randY)) * Mathf.Sign(randY);

            int xOffset = Random.RandSignZero();
            int yOffset = Random.RandSignZero();
            if (xOffset == 0 && yOffset == 0)
            {
                if (Random.RandBool())
                    xOffset = Random.RandSign();
                else
                    yOffset = Random.RandSign();
            }

            return target + new Vec2Int(xOffset, yOffset);
        }
    }
}