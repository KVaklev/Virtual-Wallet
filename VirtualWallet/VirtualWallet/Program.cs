using VirtualWallet.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

builder.Services.AddAutoMapper(typeof(CustomAutoMapper).Assembly);

// Configure the HTTP request pipeline.

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//}

app.UseDeveloperExceptionPage();
app.UseRouting();
app.UseSession();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
