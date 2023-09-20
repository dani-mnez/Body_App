using Web_BodyApp.Data;
using Web_BodyApp.Data.ServiceClasses;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<ReloadService>();
builder.Services.AddScoped<ModalService>();
builder.Services.AddScoped<Calculus>();
builder.Services.AddScoped<Utils>();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = false;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
