using API.Extensions;
using API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://edu-play-three.vercel.app") 
                                .AllowAnyHeader()  
                                .AllowAnyMethod(); 
                      });
});

builder.Services.AddControllers();

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseGlobalExceptionMiddleware();

app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "EduPlay API v1");
        options.RoutePrefix = "swagger";
    });



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();