using System.Security.Claims;
using System.Text;
using Library_Management_System.Data;
using Library_Management_System.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCustomServices();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

//jwt Authentication 
//creating key
var key =Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException());
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            IssuerSigningKey = new SymmetricSecurityKey(key),

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role,

            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

//making security strict for the form by including csrf for preventing xss.

builder.Services.AddAntiforgery(options=>options.HeaderName = "X-CSRF-TOKEN"); 

builder.Services.AddAuthorization(); //it comes first for using [authorize]
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
//these come before authorization and authentication
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
//must use useAuthentication before useAuthorization
app.UseAuthentication();
app.UseAuthorization();




app.MapControllerRoute(
    name: "Home",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapRazorPages();

app.Run();