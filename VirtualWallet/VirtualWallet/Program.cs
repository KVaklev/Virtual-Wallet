using Business.Mappers;
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
using VirtualWallet.Helpers;
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
            builder.Services.AddScoped<ICardRepository, CardRepository>();
            builder.Services.AddScoped<ITransferRepository, TransferRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
            builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();


            //Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ICardService, CardService>();
            builder.Services.AddScoped<ITransferService, TransferService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICurrencyService, CurrencyService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();

            //Helpers
            builder.Services.AddScoped<IAuthManager, AuthManager>();
            builder.Services.AddScoped<AuthManager>();
            builder.Services.AddScoped<HelpersApi>();
            builder.Services.AddSwaggerGen();
            

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
