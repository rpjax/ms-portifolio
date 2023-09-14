namespace ModularSystem.Core
{
    public class AspNetServer
    {
        //public void Run()
        //{
        //    var builder = WebApplication.CreateBuilder();

        //    // Add services to the container.

        //    builder.Services.AddControllers();
        //    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //    builder.Services.AddEndpointsApiExplorer();
        //    builder.Services.AddSwaggerGen();

        //    var app = builder.Build();

        //    // Configure the HTTP request pipeline.
        //    if (app.Environment.IsDevelopment())
        //    {
        //        app.UseSwagger();
        //        app.UseSwaggerUI();
        //    }

        //    app.UseHttpsRedirection();

        //    app.UseAuthorization();

        //    app.MapControllers();

        //    app.Run();

        //}

        //internal class Startup
        //{
        //    public Startup(IConfiguration configuration)
        //    {
        //        Configuration = configuration;
        //    }

        //    public IConfiguration Configuration { get; }

        //    // This method gets called by the runtime. Use this method to add services to the container.
        //    public void ConfigureServices(IServiceCollection services)
        //    {
        //        services.AddMvcCore();
        //        services.AddControllers();
        //        services.AddCors();
        //        services.AddSwaggerGen();
        //    }

        //    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        //    {
        //        app.UseDeveloperExceptionPage();
        //        //app.UseHttpsRedirection();
        //        app.UseRouting();
        //        app.UseAuthorization();
        //        app.UseSwagger();
        //        app.UseSwaggerUI(options =>
        //        {
        //            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        //            options.RoutePrefix = "";
        //        });
        //        app.UseEndpoints(endpoints =>
        //        {
        //            endpoints.MapControllers();
        //            endpoints.MapSwagger();
        //        });

        //    }
        //}
    }


}
