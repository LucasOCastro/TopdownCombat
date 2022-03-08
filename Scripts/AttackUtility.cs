using Mathf = Godot.Mathf;
using Vector2 = Godot.Vector2;

namespace CombatGame
{
    public static class AttackUtility
    {
        //Firing accuracy formula from : https://github.com/OpenXcom/OpenXcom/blob/eacdad08e08d2991cbd958c04f02d290e12a8227/src/Savegame/BattleUnit.cpp
        //accuracyStat * weaponAccuracy * kneelingbonus(1.15) * one-handPenalty(0.8) * woundsPenalty(% health) * critWoundsPenalty (-10%/wound)

        //Distance fallof formula from https://www.ufopaedia.org/index.php/Chance_to_Hit_(EU2012)
        //float distanceFallof = 42 - (4.5f * distance);

        //TODO study better formulas
        public static float CalculateRangedHitChance(Entity attacker, WeaponBase weapon, Vec2Int target)
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

            int xOffset = Random.RandSign();
            int yOffset = Random.RandSign();
            if (xOffset == 0 && yOffset == 0)
            {
                if (Random.RandBool())
                    xOffset = Random.RandSignNoZero();
                else
                    yOffset = Random.RandSignNoZero();
            }

            return target + new Vec2Int(xOffset, yOffset);
        }

        //TODO rework "casting" to a specialized file later
        private static bool LineCast(Vector2 start, Vector2 end, Map map)
        {
            Vec2Int[] line = Bresenham.Rasterize(start, end, GameManager.GameScale);
            for (int i = 0; i < line.Length; i++)
            {
                Vec2Int pos = line[i];
                if (!map.CanSeeThrough(pos))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsInLineOfSight(Entity entity, Vec2Int tile)
        {
            Map map = entity.CurrentMap;
            Vector2 start = map.GetTileCenter(entity.Position);
            Vector2 end = map.GetTileCenter(tile);
            if (LineCast(start, end, map)){
                return true;
            }

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0){
                        continue;
                    }
                    Vector2 offsetEnd = end + (new Vector2(x, y) * .5f * GameManager.GameScale);
                    if (LineCast(start, offsetEnd, map)){
                        return true;
                    }
                }
            }
            return false;
        }
    }
}