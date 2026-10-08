using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using WebTestOne.Models;
using WebTestOne.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("QLBanSach")));
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = ".BookOnline.Cart";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.IdleTimeout = TimeSpan.FromHours(2);
        });
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<CartService>();
        builder.Services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            var vietnameseCulture = CultureInfo.GetCultureInfo("vi-VN");
            options.DefaultRequestCulture = new RequestCulture(vietnameseCulture);
            options.SupportedCultures = [vietnameseCulture];
            options.SupportedUICultures = [vietnameseCulture];
        });

        // --- BẮT ĐẦU CẤU HÌNH AUTHENTICATION BẰNG COOKIE ---
        builder.Services.AddAuthentication("MyCookieAuth")
            .AddCookie("MyCookieAuth", options =>
            {
                // Đường dẫn hệ thống sẽ chuyển hướng tới khi người dùng chưa đăng nhập
                options.LoginPath = "/Account/Login";

                // Đường dẫn khi người dùng đăng nhập rồi nhưng không có quyền (VD: User vào trang Admin)
                options.AccessDeniedPath = "/Account/AccessDenied";

                // Thời gian sống của Cookie đăng nhập (VD: 30 phút)
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });
        // --- KẾT THÚC CẤU HÌNH AUTHENTICATION ---

        var app = builder.Build();

        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                var contentType = context.Response.ContentType;
                var isTextResponse = contentType?.StartsWith("text/", StringComparison.OrdinalIgnoreCase) == true
                    || contentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true;

                if (isTextResponse
                    && contentType is not null
                    && !contentType.Contains("charset=", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.ContentType = $"{contentType}; charset=utf-8";
                }

                return Task.CompletedTask;
            });

            await next();
        });

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseStaticFiles();

        app.UseRequestLocalization();
        app.UseRouting();

        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
