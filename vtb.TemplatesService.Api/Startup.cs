using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using vtb.Auth.Jwt;
using vtb.Auth.Permissions;
using vtb.Auth.Tenant;
using vtb.TemplatesService.BusinessLogic;
using vtb.TemplatesService.BusinessLogic.Managers;
using vtb.TemplatesService.DataAccess.Repositories;
using vtb.TemplatesService.DataAccess.Seed;
using vtb.TemplatesService.DomainModel;
using vtb.Utils.Extensions;

namespace vtb.TemplatesService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry();

            services.AddVtbPrerequisites();
            services.AddTenantProvider();
            services.AddAutoMapper(typeof(Startup).Assembly);

            // configuration
            //services.Configure<BusConfiguration>(Configuration.GetSection("Bus"));
            services.Configure<MongoDbConfiguration>(Configuration.GetSection("MongoDb"));
            services.Configure<JwtSettings>(Configuration.GetSection("Jwt"));

            // managers
            services.AddTransient<ITemplateKindManager, TemplateKindManager>();
            services.AddTransient<ITemplateManager, TemplateManager>();

            // repositories
            services.AddTransient<ITemplateKindsRepository, TemplateKindsRepository>();
            services.AddTransient<ITemplatesRepository, TemplatesRepository>();

            // mongo db
            services.AddSingleton((sp) =>
            {
                var mongoOptions = sp.GetService<IOptions<MongoDbConfiguration>>();
                return new MongoClient(mongoOptions.Value.ConnectionString);
            });

            services.AddTransient((sp) =>
            {
                var mongoOptions = sp.GetService<IOptions<MongoDbConfiguration>>();
                var mongoClient = sp.GetService<MongoClient>();
                var database = mongoClient.GetDatabase(mongoOptions.Value.DatabaseName);

                return database;
            });

            // Bus
            //services.Configure<BusConfiguration>(Configuration.GetSection("Bus"));
            //services.AddMassTransit(x =>
            //{
            //    var busConfiguration = Configuration.GetSection("Bus").Get<BusConfiguration>();

            //    if (busConfiguration.InMemory)
            //    {
            //        x.UsingInMemory((context, config) =>
            //        {
            //        });
            //    }
            //    else
            //    {
            //        x.UsingRabbitMq((context, config) =>
            //        {
            //            config.Host(busConfiguration.Host, h =>
            //            {
            //                h.Username(busConfiguration.UserName);
            //                h.Password(busConfiguration.Password);
            //            });
            //        });
            //    }

            //    //x.AddRequestClient<IRemoveTerminalKind>(new Uri("queue:asd"));
            //});
            //services.AddMassTransitHostedService();

            // Auth
            services.AddJwtAuthentication(Configuration["Jwt:Secret"]);
            services.AddAuthorization(configure =>
            {
                foreach (var policy in PermissionPoliciesHelper.Policies)
                    configure.AddPolicy(policy.Key, policy.Value);
            });

            // API
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.DescribeAllParametersInCamelCase();

                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = typeof(Startup).Assembly.GetName().Version.ToString(),
                    Title = "vtb.TemplatesService API",
                    Description = "API to interact with Templates Service over HTTPS protocol.",
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "vtb.TemplatesService API");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var db = app.ApplicationServices.GetService<IMongoDatabase>();
            var templateKinds = db.GetCollection<TemplateKind>(nameof(TemplateKinds));

            if (templateKinds.CountDocuments(Builders<TemplateKind>.Filter.Empty) == 0)
            {
                var seeder = new Seeder(db);

                var tasks = new[] {
                    seeder.Seed<TemplateKind>(typeof(TemplateKinds)),
                    seeder.Seed<Template>(typeof(Templates))
                };

                Task.WaitAll(tasks);
            }
        }
    }
}