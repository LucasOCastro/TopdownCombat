using Mathf = Godot.Mathf;
using Vector2 = Godot.Vector2;
using System.Collections.Generic;

namespace CombatGame
{
    public static class CoverUtility
    {
        //For reference, RimWorld's Verse.CoverUtility.TryFindAdjustedCoverInCell
        private static CoverInfo GenCoverInfo(Structure cover, Vec2Int target, Vec2Int attackerPos)
        {
            Vector2 targetToAttacker = ((Vector2)attackerPos) - ((Vector2)target);
            Vector2 targetToCover = ((Vector2)cover.Position) - ((Vector2)target);
            float angleRad = targetToCover.AngleTo(targetToAttacker);
            float angle = Mathf.Rad2Deg(angleRad);

            // float coverStrength;
            // if (angle <= 30){
            //     coverStrength = 1f;
            // }
            // else if (angle <= 50){
            // }
            float angleFactor = 1f - Mathf.Clamp(angleRad / (Mathf.Pi / 2f), 0f, 1f);
            float coverStrength = cover.Base.CoverStrength * angleFactor;
            return new CoverInfo() { coverProvider = cover, coverStrength = coverStrength };
        }

        //For reference, Rimworld's Verse.CoverUtility.CalculateOverallBlockChance
        public static List<CoverInfo> GetCovers(Vec2Int tile, Vec2Int attackerPos, Map map, out float totalCoverStrength)
        {
            totalCoverStrength = 0f;
            List<CoverInfo> covers = new List<CoverInfo>();
            foreach (Vec2Int neighbor in map.GetNeighbors(tile))
            {
                Structure structure = map.GetAt<Structure>(neighbor);
                if (structure == null) continue;

                CoverInfo cover = GenCoverInfo(structure, tile, attackerPos);
                if (cover.coverStrength > 0){
                    totalCoverStrength = (1 - totalCoverStrength) * cover.coverStrength;
                    covers.Add(cover);
                }
            }
            return covers;
        }
    }
}