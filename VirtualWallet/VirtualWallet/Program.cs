using DataAccess.Repositories.Data;
using Microsoft.EntityFrameworkCore;
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

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
           
            builder.Services.AddAutoMapper(typeof(CustomAutoMapper).Assembly);
            builder.Services.AddSwaggerGen();
            var app = builder.Build();


            // Configure the HTTP request pipeline.

            //if (!app.Environment.IsDevelopment())
            //{
            //    app.UseExceptionHandler("/Error");
            //}

            app.UseDeveloperExceptionPage();
            app.UseRouting();
          //  app.UseSession();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseAuthorization();
            app.MapDefaultControllerRoute();
            app.MapRazorPages();
            app.Run();

        }
    }
}
