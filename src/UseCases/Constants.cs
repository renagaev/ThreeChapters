namespace UseCases;

public static class Constants
{
    public const string ReadMark = "✅";
    public const string UnreadMark = "❔";

    public static IReadOnlyDictionary<string, int> Months = new Dictionary<string, int>
    {
        ["января"] = 1,
        ["февраля"] = 2,
        ["марта"] = 3,
        ["апреля"] = 4,
        ["мая"] = 5,
        ["июня"] = 6,
        ["июля"] = 7,
        ["августа"] = 8,
        ["сентября"] = 9,
        ["окрября"] = 10,
        ["ноября"] = 11,
        ["декабря"] = 12
    };
}