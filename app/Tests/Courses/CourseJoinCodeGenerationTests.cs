using app.Utils;
using NUnit.Framework;

namespace app.Tests.Courses;

public class CourseJoinCodeGenerationTests
{
    
    [Test]
    public void EnsureJoinCodesIsOfCorrectLength()
    {
        Dictionary<string,Guid> joinCodes = new();
        var code = JoinCodeUtils.GenerateNonRandomJoinCode(joinCodes.Keys,4);
        
        Assert.That(code.Length,Is.EqualTo(4));
    }
    
}