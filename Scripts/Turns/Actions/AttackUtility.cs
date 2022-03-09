using Mathf = Godot.Mathf;
using Vector2 = Godot.Vector2;
using System.Collections.Generic;

namespace CombatGame
{
    
    
    public static class AttackUtility
    {
        //Firing accuracy formula from : https://github.com/OpenXcom/OpenXcom/blob/eacdad08e08d2991cbd958c04f02d290e12a8227/src/Savegame/BattleUnit.cpp
        //accuracyStat * weaponAccuracy * kneelingbonus(1.15) * one-handPenalty(0.8) * woundsPenalty(% health) * critWoundsPenalty (-10%/wound)

        //Distance fallof formula from https://www.ufopaedia.org/index.php/Chance_to_Hit_(EU2012)
        //this aint even a formula dumbass
        //float distanceFallof = 42 - (4.5f * distance);

        //TODO study better formulas
        public static HitChanceReport CalculateRangedHitChance(Entity attacker, WeaponBase weapon, Vec2Int target)
        {
            // float distanceAccuracy = Mathf.Clamp(1f - (distance / attacker.MaximumEfficientShootDistance), 0, 1);

            HitChanceReport hitChance = new HitChanceReport();
            hitChance.weapon = weapon;

            float distance = attacker.Position.DistanceTo(target);
            if (distance >= weapon.MinDistanceForFallof && distance <= weapon.MaxDistanceForFallof){
                hitChance.distanceFallof = weapon.DistanceAccuracyFallof * distance;
            }

            hitChance.covers = CoverUtility.GetCovers(target, attacker.Position, attacker.CurrentMap, out hitChance.totalCoverStrength);

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
                if (Random.Bool())
                    xOffset = Random.RandSignNoZero();
                else
                    yOffset = Random.RandSignNoZero();
            }

            return target + new Vec2Int(xOffset, yOffset);
        }

        //TODO rework "casting" to a specialized file later
        private static bool LineCast(Vec2Int startTile, Vec2Int endTile, Map map, System.Func<Vec2Int, bool> hitValidator, out Vec2Int hit)
            => LineCast(map.GetTileCenter(startTile), map.GetTileCenter(endTile), hitValidator, out hit);
        private static bool LineCast(Vector2 start, Vector2 end, System.Func<Vec2Int, bool> hitValidator, out Vec2Int hit)
        {
            Vec2Int[] line = Bresenham.Rasterize(start, end, GameManager.GameScale);
            for (int i = 0; i < line.Length; i++)
            {
                Vec2Int pos = line[i];
                if (hitValidator(pos))
                {
                    hit = pos;
                    return true;
                }
            }
            hit = Vec2Int.One * -1;
            return false;
        }

        public static bool IsInLineOfSight(Entity entity, Vec2Int tile)
        {
            Map map = entity.CurrentMap;
            Vector2 start = map.GetTileCenter(entity.Position);
            Vector2 end = map.GetTileCenter(tile);
            bool hitValidator(Vec2Int t) => map.CanSeeThrough(t);
            if (LineCast(start, end, hitValidator, out _)){
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
                    if (LineCast(start, offsetEnd, hitValidator, out _)){
                        return true;
                    }
                }
            }
            return false;
        }
    }
}