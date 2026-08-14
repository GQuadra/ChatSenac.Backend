using System.Security.Claims;

namespace ChatSENAC.Backend.Extensions;

public static class ClaimsPrincipalExtension
{
    public static int? ObterUsuarioId(this ClaimsPrincipal user)
    {
        var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                      ?? user.FindFirst("sub")?.Value;

        if (int.TryParse(idClaim, out var usuarioId))
            return usuarioId;

        return null;
    }

}