using LibraryManagementSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services.
builder.Services.AddControllersWithViews();

// Connect to MongoDB. LibraryContext wraps the MongoDB client/collections.
// Registered as a singleton because MongoClient/collections are thread-safe
// and meant to be reused for the lifetime of the app.
builder.Services.AddSingleton<LibraryContext>();

var app = builder.Build();

// Note: unlike EF's EnsureCreated(), MongoDB creates the database and
// collections automatically the first time data is written, so no
// explicit setup step is needed here.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
