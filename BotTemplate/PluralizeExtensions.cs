namespace BotTemplate;

public static class PluralizeExtensions
{
    public static string Pluralize(this int count, string oneTwoManyPipeSeparated)
    {
        return ((long) count).PluralizeLong(oneTwoManyPipeSeparated);
    }

    public static string Pluralize(this int count, string one, string two, string many)
    {
        return ((long) count).PluralizeLong(one, two, many);
    }
    
    public static string PluralizeLong(this long count, string oneTwoManyPipeSeparated)
    {
        var parts = oneTwoManyPipeSeparated.Split("|");
        return count.PluralizeLong(parts[0], parts[1], parts[2]);
    }

    public static string PluralizeLong(this long count, string one, string two, string many)
    {
        if (count <= 0 || (count % 100 >= 10 && count % 100 <= 20) || count % 10 > 4)
            return count + " " + many;
        if (count % 10 == 1) return count + " " + one;
        return count + " " + two;
    }
}