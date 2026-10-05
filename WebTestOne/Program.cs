public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

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

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // QUAN TRỌNG: Lệnh này giúp web đọc được file CSS, JS, Hình ảnh trong thư mục wwwroot
        app.UseStaticFiles();

        app.UseRouting();

        // Bật Middleware Xác nhận danh tính (Bắt buộc phải đứng trước UseAuthorization)
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}