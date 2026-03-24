using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MyApp.Data;
using hw_2_2_3_26.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
   options.UseSqlServer(builder.Configuration.GetConnectionString("SqlClient"));
});

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();

builder.Services.AddAutoMapper(cfg => {}, typeof(Program));

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.EnableAnnotations();
    // options.SwaggerDoc("v1", new OpenApiInfo { Title = "API V1", Version = "v1" });
    // options.SwaggerDoc("v2", new OpenApiInfo { Title = "API V2", Version = "v2" });
    options.SwaggerDoc("v3", new OpenApiInfo { Title = "API V3", Version = "v3" });
    options.SwaggerDoc("v4", new OpenApiInfo { Title = "API V4", Version = "v4" });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToLowerInvariant());
        }
    });
}

app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.Run();
