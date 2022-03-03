namespace CombatGame
{
    public static class TextUtility
    {
        public static string ToStringSafe<T>(this T t) => t?.ToString() ?? "Null";
    }
}