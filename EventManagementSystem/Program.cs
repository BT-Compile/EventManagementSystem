var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSession();  // Add session state

var app = builder.Build();

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//}

app.UseDeveloperExceptionPage();

app.UseStaticFiles();

app.UseSession();   // use session state in the pipline

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
