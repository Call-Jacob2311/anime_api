using anime_api.Services;
using anime_api_shared.Models.ModelValidations;
using anime_api_shared.Repositories;
using anime_api_shared.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;

namespace anime_api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Configure services here
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure MVC
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = null;
                        options.JsonSerializerOptions.WriteIndented = true;
                    });

            // Configure database context (example with Entity Framework Core)
            //services.AddDbContext<YourDbContext>(options =>
            //    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Register application services
            services.AddScoped<IAnimeRepository, AnimeRepository>();
            services.AddScoped<IAnimeService, AnimeService>();
            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

            // Register FluentValidation validators
            services.AddValidatorsFromAssemblyContaining<AnimePostModelValidator>();
            services.AddValidatorsFromAssemblyContaining<AnimePutModelValidator>();

            // Configure FluentValidation with MVC
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Anime API",
                    Version = "v1",
                    Description = "API for C# solution related to anime/manga/etc.",
                    Contact = new OpenApiContact
                    {
                        Name = "Jacob Call",
                        Email = "dehholio@gmail.com",
                        Url = new Uri("https://yourwebsite.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license")
                    }
                });
            });

            // Configure CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("https://specific-origin.com")
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            // Configure authentication (if needed)
            // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //         .AddJwtBearer(options =>
            //         {
            //             options.TokenValidationParameters = new TokenValidationParameters
            //             {
            //                 ValidateIssuer = true,
            //                 ValidateAudience = true,
            //                 ValidateLifetime = true,
            //                 ValidateIssuerSigningKey = true,
            //                 ValidIssuer = Configuration["Jwt:Issuer"],
            //                 ValidAudience = Configuration["Jwt:Audience"],
            //                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
            //             };
            //         });

            // Configure other services
            // services.AddHttpClient();
            // services.AddMemoryCache();
            // services.AddHealthChecks();
        }

        // Configure the HTTP request pipeline here
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Using middleware for global exception handling to avoid repeating try-catch blocks.
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");

            // app.UseAuthentication();  // Uncomment if authentication is configured
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // endpoints.MapHealthChecks("/health"); // Uncomment if health checks are configured
            });
        }
    }
}