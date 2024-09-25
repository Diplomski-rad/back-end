using Courses_app;
using Courses_app.Repository;
using Courses_app.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Courses_app.Models;
using Courses_app.Services.PayPalService;
using Microsoft.Extensions.FileProviders;
using Quartz;
using Courses_app.WebSocket;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();  

    // Definišite Quartz posao
    var jobKey = new JobKey("PayoutJob");
    q.AddJob<PayoutJob>(opts => opts.WithIdentity(jobKey));

    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("PayoutJob-trigger")
        //.WithCronSchedule("0 0 9 ? * MON *")); // Monday at 9AM
        //.WithCronSchedule("0/5 * * * * ?")); // Every 5 seconds
        //.WithCronSchedule("0 35 19 ? * * *")); // Every day at 07:35PM
        //.WithCronSchedule("0 43 19 ? * * *")); 0 41 11 ? * WED *
        //.WithCronSchedule("0 41 11 ? * WED *")); // Wednesday at 11:41AM
        //.WithCronSchedule("0 45 10 ? * THU *")); // Thurstday at 10:45AM
        .WithCronSchedule("0 03 18 ? * WED *")); // Wednesday at 11:41AM

});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


builder.Services.AddSignalR();

builder.Services.Configure<FormOptions>(options =>
{
    // Set the maximum allowed size for form body
    options.MultipartBodyLengthLimit = 524288000; // 500 MB, adjust this size as needed
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient",

        builder =>
        {
            builder.WithOrigins("http://localhost:3000") // Replace with your React app's origin
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()  // Ovo je važno za SignalR
                   .WithExposedHeaders("Access-Control-Allow-Origin") // Za sigurnosne svrhe
                   .SetIsOriginAllowed((host) => true); // Dopušta sve dozvoljene izvore
        });
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CoursesAppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Authentication

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero

    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(UserRole.Admin.ToString()));
    options.AddPolicy("AuthorOnly", policy => policy.RequireRole(UserRole.Author.ToString()));
    options.AddPolicy("UserOnly", policy => policy.RequireRole(UserRole.User.ToString()));
});


//Services
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IDailymotionAuthService, DailymotionAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IPayoutService, PayoutService>();

builder.Services.AddScoped<IPayPalService,PayPalService>();
builder.Services.AddScoped<IVideoUploadService, VideoUploadService>();


//Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IPayoutRepository, PayoutRepository>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    // Apply request body size limit (optional)
    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = 524288000; // 500 MB, adjust as needed
    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowReactClient");

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/images"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapHub<CourseHub>("/courseHub");

app.Run();
