using QRCoder;

namespace app.Utils;

public static class JoinCodeUtils
{
    
    /**
     * Generates a random, and unique join code, consisting of letters A-Z with the given length.
     * It takes in a collection of keys to check if the generated key is unique, if not, it retries until it succeeds.
     * One MAJOR drawback of this is when we have many join codes in the dict,
     * statistically in that scenario, this method is going to take a long ass time to finish,
     * therefore, a better solution should be implemented...
     */
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

    public static string GenerateJoinCodeSvg(string url)
    {
        using var qrCodeData = QRCodeGenerator.GenerateQrCode(new PayloadGenerator.Url(url));
        var svgRenderer = new SvgQRCode(qrCodeData);
        return svgRenderer.GetGraphic();

    }
}