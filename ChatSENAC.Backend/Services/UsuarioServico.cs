using ChatSENAC.Backend.Dados;
using ChatSENAC.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatSENAC.Backend.Services;

public class UsuarioServico(ChatSenacContext context)
{
    public async Task<Usuario> ProcuraPorEmailAsync(string email)
    {
        var usuarioExiste = await context.Usuarios.FirstOrDefaultAsync(r => r.Email == email);
        return usuarioExiste ?? throw new Exception("Usuario não existe");
    }
    
    public async Task<Usuario> ProcuraPorIdAsync(int usuarioId)
    {
        var usuarioExiste = await context.Usuarios.FirstOrDefaultAsync(r => r.Id == usuarioId);
        return usuarioExiste ?? throw new Exception("Usuario não existe");
    }
    
    public async Task AdicionaAsync(Usuario usuario)
    {
        var usuarioExiste = await context.Usuarios.FirstOrDefaultAsync(r => r.Email == usuario.Email);
        if (usuarioExiste is not null) throw new Exception("Usuário já existe");
        await context.Usuarios.AddAsync(usuario);
        await context.SaveChangesAsync();
    }
}