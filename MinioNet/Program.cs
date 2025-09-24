using Microsoft.OpenApi.Models;
using Minio;
using MinioNet.Config;
using MinioNet.Services;

var builder = WebApplication.CreateBuilder(args);


#region setting Minio
var minioConfig = new MinioConfiguration
{
    Endpoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT"),
    AccessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY"),
    SecretKey = Environment.GetEnvironmentVariable("MINIO_SECRETKEY"),
    UseSSL =  bool.Parse(Environment.GetEnvironmentVariable("MINIO_USESSL"))
};

builder.Services.AddSingleton(minioConfig);
builder.Services.AddSingleton<MinioService>();
#endregion
// Add services to the container.

builder.Services.AddLogging();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
    c.OperationFilter<FileUploadOperation>(); // Áp dụng bộ lọc cho file upload
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
