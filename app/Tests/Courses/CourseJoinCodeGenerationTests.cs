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

    [Test]
    public void EnsureJoinCodeQrCodeIsSvgHtmlElement()
    {
        string qrCodeUrl = "https://gruppering.no/join?joinCode=TEST";
        var result = JoinCodeUtils.GenerateJoinCodeSvg(qrCodeUrl);
        Assert.That(result,Does.StartWith("<svg").And.EndsWith("</svg>"));
    }
    
    [Test]
    public void EnsureJoinCodeQrCodeGenerationFailsWhenPassingEmptyUrl()
    {
        string qrCodeUrl = "";
        Assert.Throws<ArgumentException>(() => JoinCodeUtils.GenerateJoinCodeSvg(qrCodeUrl));
    }
    
}