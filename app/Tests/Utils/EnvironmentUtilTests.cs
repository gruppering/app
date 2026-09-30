using app.Utils;
using NUnit.Framework;

namespace app.Tests.Utils;

public class EnvironmentUtilTests
{

    [Test]
    public void EnsureUtilSetsVariableCorrectly()
    {
        EnvironmentUtils.SetVariableIfNotPresent("BLAHBLAHTEST", "YES");

        Assert.That(Environment.GetEnvironmentVariable("BLAHBLAHTEST"), Is.EqualTo("YES"));
    }
    
    [Test]
    public void EnsureUtilDoesNotOverrideVariable()
    {
        EnvironmentUtils.SetVariableIfNotPresent("BLAHBLAHTEST2", "YES");
        
        EnvironmentUtils.SetVariableIfNotPresent("BLAHBLAHTEST2", "NO");
        
        Assert.That(Environment.GetEnvironmentVariable("BLAHBLAHTEST"), Is.Not.EqualTo("NO"));
    }
}