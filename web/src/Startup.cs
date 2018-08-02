using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore;
using Serilog.Formatting.Compact;
using Serilog.Events;
using SearchWeb.Middleware;
using idunno.Authentication.Basic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace SearchWeb
{
    public class SearchWeb
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .Build();

        private static string ASPNETCORE_URLS = Environment.GetEnvironmentVariable("SEARCH_WEB_ASPNETCORE_URLS");

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
                Log.Information("Unified search web starting up...");
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
                Log.Fatal(exception, "Unified search web terminated unexpectedly");
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

            _logger.LogInformation("Enabling MVC");
            services.AddMvc(options =>
            {
                if (Environment.GetEnvironmentVariable("WEBSEAL_AUTH_DISABLED") != "true")
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

            _logger.LogInformation("Configuring response compression");
            // Enable GZIP response compression
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _logger.LogInformation("Configuring server");

            app.UseMiddleware<RequestLogger>();
            if (Environment.GetEnvironmentVariable("WEBSEAL_AUTH_DISABLED") != "true")
            {
                app.UseAuthentication();
            }

            app.UseResponseCompression();

            app.UseStatusCodePagesWithReExecute("/Default/Error", "?statusCode={0}");
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute("Client", "{*anything}", defaults: new { controller = "Default", action = "Index" });
            });
        }
    }
}