using System.Text.RegularExpressions;
using ChatSENAC.Backend.Exceptions;

namespace ChatSENAC.Backend.Models;

public class Usuario
{
    protected Usuario()
    {
    }

    public Usuario(string nome, string email, string senha)
    {
        Validate(nome, email, senha);
        Nome = nome;
        Email = email;
        SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha);
    }

    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;

    private static void Validate(string nome, string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome é obrigatório.");

        if (nome.Length is < 2 or > 100)
            throw new DomainException("O nome deve ter entre 2 e 100 caracteres.");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("O e-mail é obrigatório.");

        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("O e-mail fornecido é inválido.");

        if (string.IsNullOrWhiteSpace(senha))
            throw new DomainException("A senha é obrigatório.");
    }
}