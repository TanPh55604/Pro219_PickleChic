using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PickleChic.DAL.Context;
using PickleChic.DAL.Repositories;
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
//builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddScoped<BrandRepository>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<ProductVariantRepository>();
builder.Services.AddScoped<ProductVariantImageRepository>();
builder.Services.AddScoped<ProductAttributeRepository>();
builder.Services.AddScoped<AttributeValueRepository>();
builder.Services.AddScoped<ProductVariantAttributeRepository>();
builder.Services.AddScoped<CartItemRepository>();
//builder.Services.AddScoped<WishlistRepository>();
//builder.Services.AddScoped<PointHistoryRepository>();
//builder.Services.AddScoped<PromotionRepository>();
//builder.Services.AddScoped<PromotionDetailRepository>();
builder.Services.AddScoped<VoucherRepository>();
builder.Services.AddScoped<OrderRepository>();
//builder.Services.AddScoped<OrderItemRepository>();
//builder.Services.AddScoped<PaymentMethodRepository>();

var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
