# How to create .NET8 CRUD WebAPI Azure Cache for Redis Microservice

## 1. Create Azure Cache for Redis in Azure Portal




## 2. appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "RedisCache": "myrediscache1974.redis.cache.windows.net:6380,password=vFuzCF5i81hLvJmmvCPTBnaoN17HMYJCyAzCaHzbkb0=,ssl=True,abortConnect=False"
  }
}
```

## 3. We set the Middleware (Program.cs)

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using AzureCacheforRedis.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger service
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Azure Cache for Redis API", Version = "v1" });
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisCache")));
builder.Services.AddScoped<RedisCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Enable middleware to serve generated Swagger as a JSON endpoint
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Azure Cache for Redis API V1"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
```

## 4. We create the Service (RedisCacheService.cs)

```csharp
using StackExchange.Redis;
using System.Threading.Tasks;


namespace AzureCacheforRedis.Services
{
    public class RedisCacheService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task SetAsync(string key, string value)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.StringSetAsync(key, value);
        }

        public async Task<string> GetAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            return await db.StringGetAsync(key);
        }

        public async Task DeleteAsync(string key)
        {
            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        // Other CRUD operations as needed

    }
}
```

## 5. We create the Controller (CacheController.cs)

```csharp
using AzureCacheforRedis.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace AzureCacheforRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CacheController : ControllerBase
    {
        private readonly RedisCacheService _cacheService;

        public CacheController(RedisCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpPost]
        public async Task<IActionResult> Set(string key, string value)
        {
            await _cacheService.SetAsync(key, value);
            return Ok();
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var value = await _cacheService.GetAsync(key);
            return Ok(value);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            await _cacheService.DeleteAsync(key);
            return Ok();
        }

        // Add other endpoints for CRUD operations

    }
}
```

## 6. Verify the application

https://localhost:7182/swagger/index.html

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-Cache-for-Redis/assets/32194879/e4908e38-a9c8-4414-9ce5-b73c99dbb9f5)

We send a POST request

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-Cache-for-Redis/assets/32194879/d3fd3fbc-7faa-43fb-b8a5-2d7c26c8c234)

We send a GET request

![image](https://github.com/luiscoco/MicroServices_dotNET8_CRUD_WebAPI-Azure-Cache-for-Redis/assets/32194879/3e603593-77a1-4389-9e5b-da7c06dbc27d)






