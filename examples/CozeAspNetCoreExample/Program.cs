using Coze.Sdk;
using Coze.Sdk.Extensions;
using CozeAspNetCoreExample.Services;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();

// 添加 Coze SDK 服务
// 方法 1: 使用配置文件中的Token认证
builder.Services.AddCozeSdk(builder.Configuration);

// 验证必需的配置项
var cozeToken = builder.Configuration["Coze:Token"];
if (string.IsNullOrEmpty(cozeToken))
{
    throw new InvalidOperationException("Coze:Token configuration is required. Please set it in appsettings.json or environment variables.");
}

// 方法 2: 如果使用 JWT OAuth，取消注释下面的代码
// builder.Services.AddCozeSdkWithJwtOAuth(builder.Configuration, "Coze:Jwt");

// 方法 3: 使用自定义配置
/*
builder.Services.AddCozeSdk(options =>
{
    options.Auth = new Coze.Sdk.Authentication.TokenAuth(builder.Configuration["Coze:Token"]);
    options.BaseUrl = builder.Configuration["Coze:BaseUrl"] ?? "https://api.coze.cn";
});
*/

// 注册自定义服务
builder.Services.AddScoped<ICozeService, CozeService>();

// 添加 Swagger/OpenAPI 服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Coze ASP.NET Core Example API",
        Version = "v1",
        Description = "An example ASP.NET Core Web API for integrating with Coze SDK"
    });
});

// 添加 CORS 支持
// 注意：生产环境应配置具体的允许源
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentCors", policy =>
    {
        // 开发环境允许本地前端访问
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:8080"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// 配置HTTP请求管道
// 为了示例目的，在所有环境中都启用Swagger
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coze ASP.NET Core Example API V1");
    c.RoutePrefix = "swagger"; // 访问地址为 /swagger
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("DevelopmentCors");

app.MapControllers();

app.Run();