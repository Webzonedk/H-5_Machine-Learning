


using ForexAnalyzer.Interfaces;
using ForexAnalyzer.Interfaces.Base;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Dependency injections
builder.Services.AddTransient<ITrainer, Trainer>();
builder.Services.AddTransient<IPredictor, Predictor>();
builder.Services.AddSingleton<IBaseML, BaseML>();




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
