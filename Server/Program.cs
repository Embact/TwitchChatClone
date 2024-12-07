
using SignalR.HubContext;
using SignalR.Models;
using SignalR.Services;
using System.Collections.Concurrent;
using Timeout = SignalR.Models.Timeout;

namespace SignalR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();
            // Register Chat system services
            builder.Services.AddSingleton<Dictionary<string, Room>>();
            builder.Services.AddSingleton<ConcurrentDictionary<string, Queue<List<Message>>>>();
            builder.Services.AddHostedService<MessageProcessingService>();
            builder.Services.AddHostedService<CheckerBackgroundService>();
            // Register the ChatService
            builder.Services.AddScoped<ChatService>();
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("chatApp", conf =>
                {
                    conf
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .WithOrigins("http://127.0.0.1:5500")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.MapControllers();

            app.MapHub<ChatHub>("chat");

            app.UseCors("chatApp");

            app.Run();
        }
    }
}
