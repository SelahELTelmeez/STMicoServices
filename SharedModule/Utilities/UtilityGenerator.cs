using System.Text;

namespace SharedModule.Extensions;

public static class UtilityGenerator
{
    public static int GetOTP(int digits)
    {
        if (digits < 3)
            return Random.Shared.Next(10, 99);
        else
            return Random.Shared.Next(MultiplyNTimes(digits), MultiplyNTimes(digits + 1) - 1);
    }
    public static string GetUniqueDigits()
    {
        StringBuilder builder = new StringBuilder();
        Enumerable
           .Range(65, 26)
            .Select(e => ((char)e).ToString())
            .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
            .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
            .OrderBy(e => Guid.NewGuid())
            .Take(11)
            .ToList().ForEach(e => builder.Append(e));
        return builder.ToString();
    }
    private static int MultiplyNTimes(int n)
    {
        if (n == 1)
            return 1;
        else
            return 10 * MultiplyNTimes(n - 1);
    }
}
