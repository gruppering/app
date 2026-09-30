namespace app.Utils;

public static class EnvironmentUtils
{
    public static bool IsInDocker()
    {
        var inDockerString = Environment.GetEnvironmentVariable("INDOCKER");
        var inDocker = false;
        if (inDockerString is not null)
        {
            inDocker = Boolean.Parse(inDockerString);
        }
        return inDocker;
    }

    public static bool SetVariableIfNotPresent(string key,string value)
    {
        if (Environment.GetEnvironmentVariable(key) is not null)
            return false;
        Environment.SetEnvironmentVariable(key, value);
        return true;
    }
}