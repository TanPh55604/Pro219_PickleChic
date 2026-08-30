using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using PickleChic.API.Options;
using PickleChic.API.Services;
using PickleChic.DAL.Context;
using PickleChic.DAL.Repositories;
using Hangfire;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var jwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer");
var jwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience") ?? jwtIssuer;
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");

if (string.IsNullOrWhiteSpace(jwtIssuer))
    throw new InvalidOperationException("Missing JWT config: Jwt:Issuer");

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException("Missing JWT config: Jwt:Key");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtAudience,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
         ClockSkew = TimeSpan.Zero
     };
 });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo { Title = "PickleChic API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddDbContext<PickleChicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=localhost;Initial Catalog=PickleChic;TrustServerCertificate=True;User Id=sa; Password=123456"));

builder.Services.AddScoped<AddressRepository>();
builder.Services.AddScoped<ProvinceRepository>();
builder.Services.AddScoped<DistrictRepository>();
builder.Services.AddScoped<WardRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<RankRepository>();
builder.Services.AddScoped<StaffRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<BrandRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductVariantRepository>();
builder.Services.AddScoped<ProductVariantImageRepository>();
builder.Services.Configure<FileStorageOptions>(
    builder.Configuration.GetSection(FileStorageOptions.SectionName));
builder.Services.AddSingleton<LocalImageFileService>();
builder.Services.AddScoped<ProductAttributeRepository>();
builder.Services.AddScoped<AttributeValueRepository>();
builder.Services.AddScoped<ProductVariantAttributeRepository>();
builder.Services.AddScoped<CartItemRepository>();
builder.Services.AddScoped<WishlistRepository>();
builder.Services.AddScoped<PointHistoryRepository>();
//builder.Services.AddScoped<PromotionRepository>();
//builder.Services.AddScoped<PromotionDetailRepository>();
builder.Services.AddScoped<VoucherRepository>();
builder.Services.AddScoped<OrderRepository>();
builder.Services.AddScoped<OrderItemRepository>();
builder.Services.AddScoped<PaymentMethodRepository>();
builder.Services.AddScoped<ReviewRepository>();
builder.Services.AddScoped<PagePermissionRepository>();

builder.Services.AddScoped<PickleChic.API.Services.OrderManagerService>();
builder.Services.AddScoped<PickleChic.API.Services.OrderStockService>();

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new Net.payOS.PayOS(
        config["PayOS:ClientId"] ?? "467ba155-36d1-4e4f-b99e-a3dd39c8b12e",
        config["PayOS:ApiKey"] ?? "305f881f-1747-402c-8e9b-131eb34160c3",
        config["PayOS:ChecksumKey"] ?? "0040e95d9b9e1c6353630b697e10d0e7e4facb59e33aabe92490e244c6773f92"
    );
});

builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=localhost;Initial Catalog=PickleChic;TrustServerCertificate=True;User Id=sa; Password=123456"));
builder.Services.AddHangfireServer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var addressRepository = scope.ServiceProvider.GetRequiredService<AddressRepository>();
    await addressRepository.EnsureSystemPickupAsync(-1);
}

var webRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(Path.Combine(webRootPath, "uploads", "products"));
Directory.CreateDirectory(Path.Combine(webRootPath, "uploads", "categories"));

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PickleChic API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRootPath),
    RequestPath = ""
});
app.UseHangfireDashboard();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
