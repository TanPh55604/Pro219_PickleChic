using MudBlazor;
using MudBlazor.Services;
using PickleChic.WEB.Components;
using PickleChic.WEB.Helpers;
using PickleChic.WEB.Services.Admin;
using PickleChic.WEB.Services.Api;
using PickleChic.WEB.Services.Auth;
using PickleChic.WEB.Services.Customer;
using PickleChic.WEB.Services.Storage;

var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["AppSettings:APIBaseURL"] ?? "https://localhost:7001/";
MediaUrl.ApiBaseUrl = apiBaseUrl;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter;
    config.SnackbarConfiguration.RequireInteraction = false;
    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 3000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
});

builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();
builder.Services.AddScoped<IAuthStorageService, AuthStorageService>();
builder.Services.AddScoped<IApiProvider, ApiProvider>();
builder.Services.AddScoped<IAuthService, AuthService>(); 
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IStaffService, StaffService>(); 
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IRankService, RankService>();
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddScoped<PickleChic.WEB.Services.Admin.IProductService, PickleChic.WEB.Services.Admin.ProductService>();
builder.Services.AddScoped<PickleChic.WEB.Services.Admin.IProductVariantService, PickleChic.WEB.Services.Admin.ProductVariantService>();
builder.Services.AddScoped<IProductVariantImageService, ProductVariantImageService>();
builder.Services.AddScoped<PickleChic.WEB.Services.Admin.IVoucherService, PickleChic.WEB.Services.Admin.VoucherService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPosService, PosService>();
builder.Services.AddScoped<IOfflinePosDraftService, OfflinePosDraftService>();
builder.Services.AddScoped<IAdminReviewService, AdminReviewService>();
builder.Services.AddScoped<PickleChic.WEB.Services.Customer.IVoucherService, PickleChic.WEB.Services.Customer.VoucherService>();
builder.Services.AddScoped<ICustomerCategoryService, CustomerCategoryService>();
builder.Services.AddScoped<ICustomerBrandService, CustomerBrandService>();
builder.Services.AddScoped<ICustomerAttributeService, CustomerAttributeService>();
builder.Services.AddScoped<PickleChic.WEB.Services.Customer.IProductService, PickleChic.WEB.Services.Customer.ProductService>();
builder.Services.AddScoped<ICustomerCartService, CustomerCartService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICustomerOrderService, CustomerOrderService>();
builder.Services.AddScoped<ICustomerReviewService, CustomerReviewService>();
builder.Services.AddScoped<ICustomerWishlistService, CustomerWishlistService>();
builder.Services.AddScoped<IPointHistoryService, PointHistoryService>();
builder.Services.AddScoped<PickleChic.WEB.Services.Customer.IProductVariantService, PickleChic.WEB.Services.Customer.ProductVariantService>();

builder.Services.AddScoped(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri(apiBaseUrl)
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/{0}");
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
