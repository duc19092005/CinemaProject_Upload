﻿using backend.Data;
using backend.Interface.Auth;
using backend.Model.Auth;
using backend.Services.Auth;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using backend.Services.MovieServices;
using backend;
using backend.Helper;
using backend.Hosted;
using backend.Interface.Account;
using backend.Interface.GenericsInterface;
using backend.ModelDTO.MoviesDTO.MovieRequest;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using backend.Interface.MovieInterface;
using backend.Interface.Schedule;
using backend.Services.Schedule;
using backend.Interface.BookingInterface;
using backend.Interface.CinemaInterface;
using backend.Services.BookingServices;
using backend.Interface.CommentInterface;
using backend.Interface.CloudinaryInterface;
using backend.Interface.EmailInterface;
using backend.Interface.FoodInterface;
using backend.Interface.MovieGenreInterface;
using backend.Interface.PDFInterface;
using backend.Interface.PriceInterfaces;
using backend.Interface.RevenueInterface;
using backend.Interface.RoomInferface;
using backend.Interface.StaffInterface;
using backend.Interface.VisualFormatInterface;
using backend.Services.CloudinaryServices;
using backend.Interface.VnpayInterface;
using backend.Services.VnpayServices;
using Microsoft.Extensions.Logging;
using backend.ModelDTO.BookingHistoryDTO.OrderDetailRespond;
using backend.ModelDTO.BookingHistoryDTO.OrderRespond;
using backend.ModelDTO.PDFDTO;
using backend.Services.AccountServices;
using backend.Services.BookingHistoryServices;
using backend.Services.CinemaServices;
using backend.Services.EmailServices;
using backend.Services.FoodServices;
using backend.Services.MovieGenreServices;
using backend.Services.MovieVisualServices;
using backend.Services.PDFServices;
using backend.Services.PriceServices;
using backend.Services.RevenueServices;
using backend.Services.RoomServices;
using backend.Services.StaffService;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestTimeouts();


builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add thêm Policy

builder.Services.AddAuthorization(
    options =>
    {
        options.AddPolicy("Customer", policy =>
        {
            policy.RequireRole("Customer");
        });
    });

builder.Services.AddAuthorization
(options => 
options.AddPolicy
("Director", policy => policy.RequireRole("Director")));

builder.Services.AddAuthorization
    (options =>
    options.AddPolicy("Cashier", policy =>
    policy.RequireRole("Cashier")));

builder.Services.AddAuthorization
    (options =>
    options.AddPolicy("MovieManager", policy =>
    policy.RequireRole("MovieManager")));

builder.Services.AddAuthorization
    (options =>
    options.AddPolicy("TheaterManager", policy =>
    policy.RequireRole("TheaterManager")));


builder.Services.AddAuthorization
    (options =>
    options.AddPolicy("FacilitiesManager", policy =>
    policy.RequireRole("FacilitiesManager")));

// Add thêm JWT services

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Iss"],
            ValidAudience = builder.Configuration["Jwt:Aud"],
            IssuerSigningKey = new SymmetricSecurityKey(UTF8Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
// Add thêm DI của các services
// Auth services
builder.Services.AddScoped<IAuth, AuthService>();

// Add Scoped

builder.Services.AddScoped<ICloudinaryServices, CloudinaryService>();

builder.Services.AddScoped<ICommentServices, CommentServices>();

builder.Services.AddScoped<IRoomService , RoomService>();

// DI của VNpay Services

builder.Services.AddScoped<IVnpayService, VnpayService>();

builder.Services.AddSingleton<BackgroundService , HostedService>();
builder.Services.AddHostedService<HostedService>();

// DI cua Price

// DI cua DIDataProtector

builder.Services.AddDataProtection()
    .SetApplicationName("MyCitizenIdApp"); // Rất khuyến nghị sử dụng

// Bước 2: Đăng ký một IDataProtector cụ thể với một purpose string
// Sử dụng AddSingleton vì IDataProtector thường an toàn cho thread và không cần tạo lại cho mỗi yêu cầu.
builder.Services.AddSingleton<IDataProtector>(serviceProvider => {
    // Lấy IDataProtectionProvider từ serviceProvider (đã được AddDataProtection() đăng ký)
    var dataProtectionProvider = serviceProvider.GetRequiredService<IDataProtectionProvider>();

    // Tạo IDataProtector với chuỗi mục đích cụ thể.
    // Chuỗi này PHẢI DUY NHẤT cho mục đích mã hóa này trong ứng dụng của bạn.
    return dataProtectionProvider.CreateProtector("CitizenIdEncryptionPurpose");
});
// DI cuar Revune
builder.Services.AddScoped<IRevenueService, RevenueService>();
// DI cua Price
builder.Services.AddScoped<IPriceService, PriceService>();

// DI cua Food

builder.Services.AddScoped<IFoodService, FoodService>();

// Add thêm DI của services Movie dạng Scoped

builder.Services.AddScoped<IMovieService, movieServices>();

// Add thêm DI của Cinema

builder.Services.AddScoped<ICinemaService, CinemaService>();
// DI cua Email
builder.Services.AddScoped<IEmailService, EmailService>();
// DI cua Staff
builder.Services.AddScoped<IStaffService, StaffService>();
// DI MovieSchedule

builder.Services.AddScoped<IScheduleServices, ScheduleServices>();

// DI của Booking

builder.Services.AddScoped<IBookingServices, BookingServices>();

// DI của Hash Helper

builder.Services.AddSingleton<HashHelper>();

// DI cua Order

builder.Services.AddScoped<IStaffOrderService , StaffOrderService>();

// DI cua Account Service

builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<VNPAY.NET.IVnpay, VNPAY.NET.Vnpay>();

builder.Services.AddScoped
    <GenericInterface<BookingHistoryRespondList, OrderDetailRespond>, OrderDetailServices>();

builder.Services.AddCors(x => x.AddPolicy("AllowAll", builder =>
{
    builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddScoped<IMovieVisualFormatService, MovieVisualService>();

builder.Services.AddScoped<IMovieGenreService, MovieGenreService>();

builder.Services.AddSingleton<IPDFService<GenerateCustomerBookingDTO, GenerateStaffBookingDTO>, PDFService>();

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));

var app = builder.Build();
app.UseCors("AllowAll");
using (var scoped = app.Services.CreateScope())
{
    var services = scoped.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DataContext>();
        context.Database.Migrate();
    }
    catch (Exception ex) 
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();



