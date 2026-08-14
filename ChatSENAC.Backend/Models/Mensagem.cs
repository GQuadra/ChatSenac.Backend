using ChatSENAC.Backend.Exceptions;

namespace ChatSENAC.Backend.Models;

public class Mensagem
{
    protected Mensagem()
    {
    }

    public Mensagem(string conteudo, int? usuarioId)
    {
        Validate(conteudo, usuarioId);
        Conteudo = conteudo;
        EnviadoEm = DateTime.UtcNow;
        UsuarioId = usuarioId ?? throw new DomainException("O usuário é obrigatório.");
    }

    public int Id { get; private set; }
    public string Conteudo { get; private set; } = string.Empty;
    public DateTime EnviadoEm { get; private set; }
    public Usuario Usuario { get; private set; } = null!;
    public int UsuarioId { get; private set; }

    private static void Validate(string content, int? usuarioId)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("O conteudo é obrigatório.");

        if (usuarioId < 1)
            throw new DomainException("O id do usuário é inválido.");
    }
}