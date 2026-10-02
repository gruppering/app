namespace app.Utils;

public static class JoinCodeUtils
{
    public static string GenerateNonRandomJoinCode(Dictionary<string,Guid>.KeyCollection overview,int length)
    {
        string joinCode;
        do
        {
            joinCode = "";
            for (int i = 0; i < length; i++)
            {
                joinCode += (char)Random.Shared.Next(65, 90);
            }
        } while (overview.Contains(joinCode));
        
        return joinCode;
    }
}