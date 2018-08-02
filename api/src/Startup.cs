using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SearchApi.Clients;
using SearchApi.Middleware;
using SearchApi.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Sinks.SystemConsole.Themes;
using idunno.Authentication.Basic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;

namespace SearchApi
{
    public class SearchApi
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .Build();

        private static string ASPNETCORE_URLS = Environment.GetEnvironmentVariable("SEARCH_API_ASPNETCORE_URLS");

        private static string ASPNETCORE_ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        private static string SEARCH_LOG_LEVEL = Environment.GetEnvironmentVariable("SEARCH_LOG_LEVEL");

        private static string SEARCH_LOG_FORMAT = Environment.GetEnvironmentVariable("SEARCH_LOG_FORMAT");

        public static int Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Trace();

            if (SEARCH_LOG_LEVEL == "debug")
            {
                loggerConfiguration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug);
            }
            else
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            }

            if (SEARCH_LOG_FORMAT == "json")
            {
                loggerConfiguration.WriteTo.Console(formatter: new RenderedCompactJsonFormatter());
            }
            else
            {
                loggerConfiguration.WriteTo.Console();
            }

            Log.Logger = loggerConfiguration.CreateLogger();

            try
            {
                Log.Information("Unified Search API starting up...");
                var host = WebHost.CreateDefaultBuilder(args)
                    .UseUrls(ASPNETCORE_URLS)
                    .UseStartup<Startup>()
                    .UseConfiguration(Configuration)
                    .UseSerilog()
                    .UseKestrel()
                    .Build();

                host.Run();
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "API terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    public class Startup
    {
        private readonly IConfiguration _configuration;

        private readonly IHostingEnvironment _env;

        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _configuration = configuration;
            _env = env;
            _logger = logger;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogInformation("Configuring services");

            _logger.LogInformation("Enabling MVC");

            if (Environment.GetEnvironmentVariable("WEBSEAL_AUTH_DISABLED") != "true")
            {
                _logger.LogInformation("Enabling authentication");

                services.AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                    .AddBasic(options =>
                    {
                        options.Realm = "ak-dhss-unified-search";
                        options.AllowInsecureProtocol = true;
                        options.Events = new BasicAuthenticationEvents
                        {
                            OnValidateCredentials = context =>
                            {
                                if (context.Username == Environment.GetEnvironmentVariable("WEBSEAL_AUTH_USER") &&
                                    context.Password == Environment.GetEnvironmentVariable("WEBSEAL_AUTH_PASS"))
                                {
                                    var claims = new[]
                                    {
                                    new Claim(
                                        ClaimTypes.NameIdentifier,
                                        context.Username,
                                        ClaimValueTypes.String,
                                        context.Options.ClaimsIssuer
                                    ),
                                    new Claim(
                                        ClaimTypes.Name,
                                        context.Username,
                                        ClaimValueTypes.String,
                                        context.Options.ClaimsIssuer
                                    )
                                    };

                                    context.Principal = new ClaimsPrincipal(
                                        new ClaimsIdentity(claims, context.Scheme.Name)
                                    );
                                    context.Success();
                                }
                                return Task.CompletedTask;
                            }
                        };

                    });
            }

            services.AddMvc(options =>
            {
                options.Filters.Add(new ProducesAttribute("application/json"));

                if (Environment.GetEnvironmentVariable("WEBSEAL_AUTH_ENABLED") == "true")
                {
                    var authPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();

                    options.Filters.Add(new AuthorizeFilter(authPolicy));
                }
            })
                .AddJsonOptions(options =>
                {
                    // Indent the JSON responses for easier reading,
                    // gzip makes the response size difference negligible
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            _logger.LogInformation("Enabling CORS");
            // CORS allows requests from different origins - i.e. the web project
            services.AddCors(options =>
            {
                if (_env.IsDevelopment())
                {
                    options.AddPolicy("CorsPolicy", builder => builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                    );
                }
                else
                {
                    options.AddPolicy("CorsPolicy",
                        builder => builder.WithOrigins(
                            Environment.GetEnvironmentVariable("SEARCH_WEB_URL"),
                            Environment.GetEnvironmentVariable("SEARCH_WEB_SECURE_URL")
                        )
                    );
                }
            });

            _logger.LogInformation("Setting up Swagger Documentation");
            // Set up Swagger for generating OAS documentation
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("searchAPIdocs", new Info
                {
                    Title = "Unified Search API",
                    Version = "v1"
                });

                var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("CorsPolicy"));
            });

            _logger.LogInformation("Configuring response compression");
            // Enable GZIP response compression
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            _logger.LogInformation("Configuring dependency injection services");
            // Add a configuration singleton for accessing environment variables
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddSingleton<IEsbClient, EsbClient>();
            services.AddSingleton<IAriesRepository, AriesRepository>();
            services.AddSingleton<IEisRepository, EisRepository>();
            services.AddSingleton<IMciRepository, MciRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _logger.LogInformation("Configuring API");

            app.UseMiddleware<RequestLogger>();
            if (Environment.GetEnvironmentVariable("WEBSEAL_AUTH_ENABLED") == "true")
            {
                app.UseAuthentication();
            }

            app.UseResponseCompression();

            app.UseExceptionHandler("/error");

            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                _logger.LogInformation("Enabling Swagger Documentation");

                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/searchAPIdocs/swagger.json", "Unified Search API");
                });
            }

            _logger.LogInformation("Adding MVC");

            app.UseMvc();

            _logger.LogInformation("Finished startup configuration");
        }
    }
}