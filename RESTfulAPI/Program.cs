using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

//Alterar em conformidade com nome dos namespaces usados
using RESTfulAPI.Context;
using RESTfulAPI.Repositories;
using RESTfulAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configura a aplicação para autenticar os utilizadores, usando tokens JWT,
// verificando o emissor, a audiência, o tempo de vida e a chave de assinatura do emissor.

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            
            //Define o emissor e a audiência válidas para o token JWT obtidos da aplicação.
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            
            //Define a chave da assinatura usada para assinar e verificar o token JWT.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    // Define um esquema seguro para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Implementa a autenticação em todos os endpoints da API
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

//NOVO  fase 1
var connection = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//NOVO  fase 1
//Permite injetar a instância do contexto nos controladores
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(connection));

//NOVO Fase 3
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<AppDbContext>();

//Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>(); //NOVO fase 2
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>(); //NOVO  fase 2
//builder.Services.AddControllers(); // repetido?

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//NOVO Fase 3
app.MapGroup("/identity").MapIdentityApi<ApplicationUser>();

//Configure the HTTP request pipeline.

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"); //NOVO?
    });
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication(); //NOVO -> é necessário quando se criar/ativar o JWT
app.UseAuthorization();
app.MapControllers();

app.Run();