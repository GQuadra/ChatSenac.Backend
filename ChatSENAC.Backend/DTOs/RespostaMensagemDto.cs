namespace ChatSENAC.Backend.DTOs;

public class RespostaMensagemDto
{
    public int Id { get; set; }
    public string Conteudo { get; set; }  = string.Empty;
    public DateTime EnviadoEm { get; set; }
    public string NomeUsuario { get; set; }  = string.Empty;
}