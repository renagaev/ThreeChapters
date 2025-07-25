namespace UseCases.Services;

public class HabitPowerCalculator
{
    public (DateOnly Date, decimal Value)[] GetPowerGraph(ICollection<DateOnly> positiveDates, DateOnly today)
    {
        positiveDates = positiveDates.Where(x => x <= today).OrderBy(x => x).ToHashSet();
        if (positiveDates.Count == 0)
        {
            return [(today, 0)];
        }

        const decimal alfa = 0.07m;

        var currDate = positiveDates.Min();

        var res = new List<(DateOnly, decimal)>();
        var ewma = 0m;
        while (currDate <= today)
        {
            var read = positiveDates.Contains(currDate) ? 1 : 0;
            ewma = alfa * read + (1 - alfa) * ewma;
            res.Add((currDate, Math.Round(ewma, 4)));

            currDate = currDate.AddDays(1);
        }

        if (res.Count > 1 && res[^1].Item2 < res[^2].Item2)
        {
            res[^1] = (res[^1].Item1, res[^2].Item2);
        }

        return res.ToArray();
    }
}