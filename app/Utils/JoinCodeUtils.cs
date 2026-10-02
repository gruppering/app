using System.Diagnostics;

namespace app.Utils;

public static class JoinCodeUtils
{
    public static string GenerateNonRandomJoinCode(HashSet<string> overview,int length)
    {
        var stopwatch = Stopwatch.StartNew();
        string joinCode;
        do
        {
            joinCode = "";
            for (int i = 0; i < length; i++)
            {
                joinCode += (char)Random.Shared.Next(65, 90);
            }
        } while (overview.Contains(joinCode));

        overview.Add(joinCode);
        stopwatch.Stop();
        Console.WriteLine(stopwatch.Elapsed);
        return joinCode;
    }
}