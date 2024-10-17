using Minio;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region setting Io
var minioEndPoint = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
var minioAccessKey = Environment.GetEnvironmentVariable("MINIO_ACCESSKEY");
var minioScretKey = Environment.GetEnvironmentVariable("MINIO_SCRETKEY");


// Add Minio using the custom endpoint and configure additional settings for default MinioClient initialization
builder.Services.AddMinio(configureClient => configureClient
    .WithEndpoint(minioEndPoint)
    .WithCredentials(minioAccessKey, minioScretKey)
.Build());


#endregion



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
