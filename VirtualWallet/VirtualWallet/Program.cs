using Business.Services.Contracts;
using Business.Services.Models;
using DataAccess.Repositories.Contracts;
using DataAccess.Repositories.Data;
using DataAccess.Repositories.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.Helpers;
using System.Text;
using VirtualWallet.Models;

namespace VirtualWallet
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.EnableSensitiveDataLogging();
            });


            builder.Services.AddRazorPages();
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(CustomAutoMapper).Assembly);
            builder.Services.AddAuthorization();

            //Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            //Services
            builder.Services.AddScoped<IUserService, UserService>();

            //Helpers
            builder.Services.AddScoped<CustomAutoMapper>();
            builder.Services.AddScoped<IAuthManager, AuthManager>();
            builder.Services.AddScoped<AuthManager>();
            builder.Services.AddSwaggerGen();

            //ToDo: Ask Kaloyan -> Monday -> then Delete
           // builder.Services.AddSwaggerGen(option=>
           // {
           //     option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
           //     option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
           //     {
           //         In = ParameterLocation.Header,
           //         Description = "Please enter a valid token",
           //         Name = "Authorization",
           //         Type = SecuritySchemeType.Http,
           //         BearerFormat = "JWT",
           //         Scheme = "Bearer"
           //     });
           //     option.AddSecurityRequirement(new OpenApiSecurityRequirement
           //     {
           //         {
           //              new OpenApiSecurityScheme
           //              {
           //                  Reference = new OpenApiReference
           //              {
           //                  Type=ReferenceType.SecurityScheme,
           //                  Id="Bearer"
           //              }
           //         },
           //          new string[]{}
           //     }
           //   });
           //});

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = builder.Configuration["Jwt:Issuer"],
                     ValidAudience = builder.Configuration["Jwt:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                 };
                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context => {
                         context.Token = context.Request.Cookies["Cookie_JWT"];
                         return Task.CompletedTask;
                     }
                 };
             });

            var app = builder.Build();


            // Configure the HTTP request pipeline.

            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            app.UseDeveloperExceptionPage();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute().RequireAuthorization();
            });
          //  app.UseSession();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
           
            app.MapDefaultControllerRoute();
            app.MapRazorPages();
            app.Run();
        }
    }
}
