using System.Security.Claims;
using System.Text;
using ChatSENAC.Backend.Dados;
using ChatSENAC.Backend.DTOs;
using ChatSENAC.Backend.Extensions;
using ChatSENAC.Backend.Models;
using ChatSENAC.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ChatSenacContext>(options =>
{
    var textoConexao = Environment.GetEnvironmentVariable("MYSQL_URL") ??
                       throw new Exception("A url do banco de dados não foi configurado corretamente");
    options.UseMySql(textoConexao, ServerVersion.AutoDetect(textoConexao));
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ??
                     throw new InvalidOperationException("A chave JWT não foi configurada!");

        var key = Encoding.UTF8.GetBytes(jwtKey);

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<UsuarioServico>();
builder.Services.AddScoped<MensagemServico>();

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("status"));

app.MapGet("/status", () => Results.Ok(new { Message = "Rodando" }));

app.MapPost("/login", async (LoginDto modelo, UsuarioServico usuarioServico) =>
{
    try
    {
        var user = await usuarioServico.ProcuraPorEmailAsync(modelo.Email);
        if (!BCrypt.Net.BCrypt.Verify(modelo.Senha, user.SenhaHash))
            return Results.BadRequest(new
            {
                Message = "Email e/ou senha inválidos!"
            });
        var token = TokenServico.CriarToken(user);
        return Results.Ok(new { NomeUsuario = user.Nome, token.Token, ExpiraEm = token.ExpiresAt });
    }
    catch (Exception e)
    {
        return Results.BadRequest(new
        {
            e.Message
        });
    }
});

app.MapPost("/cadastro", async (CadastroDto modelo, UsuarioServico usuarioServico) =>
{
    try
    {
        var user = new Usuario(modelo.Nome, modelo.Email, modelo.Senha);
        await usuarioServico.AdicionaAsync(user);
        return Results.Created();
    }
    catch (Exception e)
    {
        return Results.BadRequest(new
        {
            e.Message
        });
    }
});

app.MapPost("/mensagem/nova", async (NovaMensagemDto modelo, ClaimsPrincipal user, UsuarioServico usuarioServico,
    MensagemServico mensagemServico) =>
{
    var usuarioId = user.ObterUsuarioId();
    if (usuarioId is null) return Results.Unauthorized();

    try
    {
        var mensagem = new Mensagem(modelo.Conteudo, usuarioId);
        await mensagemServico.AdicionaMensagemAsync(mensagem);
        return Results.Created();
    }
    catch (Exception e)
    {
        return Results.BadRequest(new { e.Message });
    }
}).RequireAuthorization();

app.MapGet("/mensagem/pegar_recentes", async (ClaimsPrincipal user, MensagemServico mensagemServico) =>
{
    var usuarioId = user.ObterUsuarioId();
    if (usuarioId is null) return Results.Unauthorized();

    var mensagens = await mensagemServico.PegarRecentesAsync();
    return Results.Ok(mensagens);
}).RequireAuthorization();

app.Run();