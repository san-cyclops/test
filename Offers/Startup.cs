using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.OpenApi.Models;
using Offers.Models.BaseEntities;
using Offers.Repository.Amadeus.Authorization;
using Offers.Repository.Connecter;
using Offers.Services.Alliances;
using Offers.Services.Auth0;
using Offers.Services.Cache;
using Offers.Services.Error;
using Offers.Services.FileSave;
using Offers.Services.Flight;
using Offers.Services.Flights;
using Offers.Services.Locations;
using Offers.Services.Offer;
using Offers.Services.Offers;
using Offers.Services.Seat;
using Offers.Services.Signals;
using Offers.Services.UserRating;
using StackExchange.Redis;
using System;
using System.IO;

namespace Offers
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
             Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //RedisConnection= ConnectionMultiplexer.Connect("localhost");
            ConfigurationOptions option = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { "cache:6379555555555555" }
            };
            var RedisConnection = ConnectionMultiplexer.Connect(option);

            services.AddScoped<IDatabase>(r => RedisConnection.GetDatabase());

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = "https://upgradeengine.us.auth0.com/";
                options.Audience = "https://upgradeengine.com";
            });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Offers", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                     }
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .WithExposedHeaders("UE-Code-Version")
                    );
            });

            services.Add(new ServiceDescriptor(typeof(IOffersService<OffersBaseEntity>), typeof(OffersService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IConnecterRepository<ConnecterBaseEntity>), typeof(ConnecterRepository), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IAuthorizationRepository), typeof(AuthorizationRepository), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IFileSaveService<FileSaveBaseEntity>), typeof(FileSaveService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IOfferService<OfferBaseEntity>), typeof(OfferService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ICacheService<CacheBaseEntity>), typeof(CacheService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ISeatService<SeatBaseEntity>), typeof(SeatService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IAuthService<AuthBaseEntity>), typeof(AuthService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ILocationsService<LocationsBaseEntity>), typeof(LocationsService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IGetAlternattiveOffers<SignalsBaseEntity>), typeof(GetAlternattiveOffers), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IGetBaseOfferService<SignalsBaseEntity>), typeof(GetBaseOfferService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IGetOffersService<SignalsBaseEntity>), typeof(GetOffersService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ISignalBucketService<SignalsBaseEntity>), typeof(SignalBucketService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ISignalsService<SignalsBaseEntity>), typeof(SignalsService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IUserRatingService<UserRatingBaseEntity>), typeof(UserRatingService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IGetAlliances), typeof(GetAlliances), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IUserRatingLogicService<UserRatingBaseEntity>), typeof(UserRatingLogicService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(ISignalsPaidService<SignalsBaseEntity>), typeof(SignalsPaidService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IErrorService<ErrorBaseEntity>), typeof(ErrorService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IFlightsService<OffersBaseEntity>), typeof(FlightsService), ServiceLifetime.Transient));
            services.Add(new ServiceDescriptor(typeof(IFlightService<OfferBaseEntity>), typeof(FlightService), ServiceLifetime.Transient));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactor)
        {
            #region LogerfileConfiguration
            string FolderPath = @"/Logs";
            if (Directory.Exists(FolderPath))
            {
                loggerFactor.AddFile("/Logs/SysLogs-{Date}.txt");
            }
            else
            {
                Directory.CreateDirectory(FolderPath);
                loggerFactor.AddFile("/Logs/SysLogs-{Date}.txt");
            }
            #endregion

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Offers v1"));


            app.UseCors(builder =>
            {
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("UE-Code-Version");
               
            });

            app.Use(async (context, next) =>
            {
                string CurrentEnvironment = Configuration["Application-Deployment-Version:Environment"];
                context.Response.Headers.Add("UE-Code-Version", Configuration["Application-Deployment-Version:"+ CurrentEnvironment +":Version"]);
                await next.Invoke();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
