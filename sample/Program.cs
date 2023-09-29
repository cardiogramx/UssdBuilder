using UssdBuilder.Extensions.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

# region ussd-builder-specific-requirements

builder.Services.AddDistributedMemoryCache(); // Any implementation of IDistributedCache works, visit https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed?view=aspnetcore-6.0
builder.Services.AddUssdServer(); //Adds the server for the default ussd request type
// builder.Services.AddUssdServerFor<SampleApplication.Models.CustomRequest>(); //Adds the server for custom ussd request type

# endregion

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();            

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
