
using OpenAI.Embeddings;
using OpenAI.Responses;
using Qdrant.Client;
using SchoolERPAIChatbot.API.Services;
using SchoolERPAIChatbot.API.Services.Interfaces;
using SchoolERPAIChatbot.API.Services.Prompts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

string? apiKey = builder.Configuration["OpenAI:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException(
        "OpenAI API key is missing.");
}

string embeddingModel = builder.Configuration["OpenAI:EmbeddingModel"]
    ?? throw new InvalidOperationException(
        "Embedding model is not configured.");
#pragma warning disable OPENAI001

builder.Services.AddSingleton(
    new ResponsesClient(apiKey));

builder.Services.AddSingleton(
    new EmbeddingClient(embeddingModel, apiKey));

builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();

#pragma warning restore OPENAI001

builder.Services.AddScoped<IAIService, AIService>();

string qdrantHost = builder.Configuration["Qdrant:Host"]?? "localhost";

int qdrantPort = builder.Configuration.GetValue<int>( "Qdrant:GrpcPort");

builder.Services.AddSingleton(
    new QdrantClient(qdrantHost, qdrantPort));

builder.Services.AddScoped<IVectorStoreService, QdrantVectorStoreService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

