using ChatSENAC.Backend.Dados;
using ChatSENAC.Backend.DTOs;
using ChatSENAC.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatSENAC.Backend.Services;

public class MensagemServico(ChatSenacContext context)
{
    public async Task AdicionaMensagemAsync(Mensagem mensagem)
    {
        await context.Mensagens.AddAsync(mensagem);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<RespostaMensagemDto>> PegarRecentesAsync()
    {
        return await context.Mensagens
            .OrderByDescending(m => m.Id)
            .Select(m => new RespostaMensagemDto
            {
                Id = m.Id,
                Conteudo = m.Conteudo,
                EnviadoEm = m.EnviadoEm,
                NomeUsuario = m.Usuario.Nome
            })
            .Take(10)
            .ToListAsync();
    }
}