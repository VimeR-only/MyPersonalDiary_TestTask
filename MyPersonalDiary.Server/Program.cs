using Microsoft.EntityFrameworkCore;
using MyPersonalDiary.Server.Data;
using MyPersonalDiary.Server.Middleware;
using MyPersonalDiary.Server.Repositories;
using MyPersonalDiary.Server.Services.Admin;
using MyPersonalDiary.Server.Services.Auth;
using MyPersonalDiary.Server.Services.Background;
using MyPersonalDiary.Server.Services.captcha;
using MyPersonalDiary.Server.Services.Email;
using MyPersonalDiary.Server.Services.invite;
using MyPersonalDiary.Server.Services.record;
using MyPersonalDiary.Server.Services.user;
using System;

namespace MyPersonalDiary.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Services.AddScoped<ICaptchaService, CaptchaService>();
            //builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IInviteService, InviteService>();
            builder.Services.AddScoped<IInviteRepository, InviteRepository>();
            builder.Services.AddScoped<ICaptchaService, CaptchaService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IRecordsService, RecordsService>();
            builder.Services.AddScoped<IRecordRepository, RecordRepository>();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
                options.SchemaName = "dbo";
                options.TableName = "SessionCache";
            });

            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("https://localhost:55040")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddHostedService<AccountDeletionBackgroundService>();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var initializer = new DbInitializer(
                        services.GetRequiredService<AppDbContext>(),
                        services.GetRequiredService<IUserService>()
                    );
                await initializer.InitializeAsync();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowReactApp");

            app.UseSession();
            app.UseAuthorization();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/admin"), appBuilder =>
            {
                appBuilder.UseMiddleware<RequireAdminMiddleware>();
            });

            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
