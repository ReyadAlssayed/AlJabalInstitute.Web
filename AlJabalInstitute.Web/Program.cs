using AlJabalInstitute.Web.Components;
using AlJabalInstitute.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Radzen;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ===== Authentication =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
       options.LoginPath = "/";
       options.AccessDeniedPath = "/not-found";
       options.Cookie.Name = "AlJabal.Student.Auth";
       options.SlidingExpiration = true;
       options.ExpireTimeSpan = TimeSpan.FromDays(7);

       // ✅ نشر عملي: Production = HTTPS + SameSite=None
       // ✅ تطوير: لا نكسر cookie على localhost
       if (builder.Environment.IsDevelopment())
       {
           options.Cookie.SameSite = SameSiteMode.Lax;
           options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
       }
       else
       {
           options.Cookie.SameSite = SameSiteMode.None;
           options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
       }
   });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<Supabase.Client>(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();

    var url = cfg["Supabase:Url"]!;
    var key = cfg["Supabase:ServiceRoleKey"]!; // ✅ هنا

    var client = new Supabase.Client(url, key);
    client.InitializeAsync().GetAwaiter().GetResult();
    return client;
});


// ===== Needed services =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

builder.Services.AddScoped<StudentAuthService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddRadzenComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// ✅ مهم لطلبات POST في Blazor/Minimal APIs (ونحن مستثنين login)
app.UseAntiforgery();

// ===== Login API (مستثنى من Anti-Forgery) =====
app.MapPost("/api/student/login",
    [IgnoreAntiforgeryToken] async (HttpContext http, StudentAuthService auth, StudentLoginDto dto) =>
    {
        var result = await auth.Login(dto.NationalId, dto.Password);

        if (!result.Success || result.StudentId == null)
            return Results.BadRequest(result.Message);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, result.StudentId.Value.ToString()),
            new Claim(ClaimTypes.Name, result.StudentName ?? "")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await http.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            });

        return Results.Ok();
    });

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public record StudentLoginDto(string NationalId, string Password);
