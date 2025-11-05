using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 讀取 server.pfx
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.ServerCertificate = new System.Security.Cryptography.X509Certificates.X509Certificate2("server.pfx", "1234");
        httpsOptions.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.RequireCertificate;
    });
});

// 註冊 Certificate Authentication
builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate(options =>
    {
        options.AllowedCertificateTypes = Microsoft.AspNetCore.Authentication.Certificate.CertificateTypes.All;
        options.Events = new CertificateAuthenticationEvents
        {
            OnCertificateValidated = context =>
            {
                var cert = context.ClientCertificate;
                Console.WriteLine("✅ OnCertificateValidated: " + cert.Subject);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, cert.Thumbprint),
                    new Claim(ClaimTypes.Name, cert.Subject)
                };
                context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));

                context.Success();
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/test", (HttpContext ctx) =>
{
    var cert = ctx.Connection.ClientCertificate;
    return Results.Json(new
    {
        CertSubject = cert?.Subject,
        UserName = ctx.User.Identity?.Name
    });
}).RequireAuthorization();

app.Run();
