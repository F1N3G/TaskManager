namespace TaskManager.Core.Services;

public class MatrixPlanner
{
    // 7 zile x 24 ore: numărul de task-uri planificate în acea oră
    private readonly int[,] _weekLoad = new int[7, 24];

    public void AddLoad(int dayIndex0To6, int hour0To23, int amount = 1)
    {
        if (dayIndex0To6 < 0 || dayIndex0To6 > 6) throw new ArgumentOutOfRangeException(nameof(dayIndex0To6));
        if (hour0To23 < 0 || hour0To23 > 23) throw new ArgumentOutOfRangeException(nameof(hour0To23));
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));

        _weekLoad[dayIndex0To6, hour0To23] += amount;
    }

    public int GetLoad(int dayIndex0To6, int hour0To23)
        => _weekLoad[dayIndex0To6, hour0To23];

    public int GetDailyTotal(int dayIndex0To6)
    {
        int sum = 0;
        for (int h = 0; h < 24; h++)
            sum += _weekLoad[dayIndex0To6, h];
        return sum;
    }
}
