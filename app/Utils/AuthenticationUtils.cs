using System.Security.Claims;

namespace app.Utils;

public class AuthenticationUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    public static ClaimsPrincipal GenerateClaimsPrincipal(List<Claim> claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "cookie"));
        
    }
}