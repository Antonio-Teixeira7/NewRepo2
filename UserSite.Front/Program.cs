using UserSite.Front.Components;
using UserSite.Front.Configuration;
using UserSite.Front.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
	?? "http://localhost:5208/";

var authApiSettings = builder.Configuration
	.GetSection(AuthApiSettings.SectionName)
	.Get<AuthApiSettings>() ?? new AuthApiSettings();

builder.Services.AddHttpClient<UserApiClient>(client =>
{
	client.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.Configure<AuthApiSettings>(builder.Configuration.GetSection(AuthApiSettings.SectionName));
builder.Services.AddScoped<UserSessionService>();

builder.Services.AddHttpClient<AuthApiClient>(client =>
{
	client.BaseAddress = new Uri(authApiSettings.BaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
