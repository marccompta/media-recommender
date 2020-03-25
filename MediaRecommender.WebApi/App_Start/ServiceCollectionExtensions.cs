using MediaRecommender.Application.Contracts;
using MediaRecommender.Application.Impl;
using MediaRecommender.Application.Impl.Mappers;
using MediaRecommender.Contracts;
using MediaRecommender.Data;
using MediaRecommender.Data.Contracts;
using MediaRecommender.Data.MoviesInternalDb;
using MediaRecommender.Data.MoviesInternalDb.Models;
using MediaRecommender.Data.TheMovieDb;
using MediaRecommender.Data.TheMovieDb.Models;
using MediaRecommender.Repositories;
using MediaRecommender.Resolvers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MediaRecommender.WebApi.App_Start
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApplicationLayer(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddTransient<IBillboardService, BillboardService>();
            services.AddTransient<Application.Impl.Mappers.IMapper, Mapper>();

            return services;
        }

        public static IServiceCollection RegisterDomainLayer(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddTransient<IBillboardRepository, BillboardRepository>();
            services.AddTransient<IIntelligentBillboardResolver, IntelligentBillboardResolver>();
            services.AddTransient<IGenresResolver, GenresResolver>();
            services.AddSingleton<MediaRecommender.Contracts.IMapper, MediaRecommender.Mappers.Mapper>();

            return services;
        }

        public static IServiceCollection RegisterDataLayer(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddTransient<IApiClient, ApiClient>();
            services.AddTransient<IBaseApiClient, TheMovieDbBaseApiClient>();
            services.AddTransient<IExternalMediaRecommendationProvider, TheMovieDbProvider>();
            services.AddTransient<ITheMovieDbAdapter, TheMovieDbAdapter>();
            services.AddTransient<ITheMovieDbGateway, TheMovieDbGateway>();
            services.AddSingleton<Data.TheMovieDb.Mappers.IMapper, Data.TheMovieDb.Mappers.Mapper>();
            services.AddTransient<IInternalDbRepository, InternalDbRepository>();

            services.Configure<MoviesInternalDbOptions>(options =>
            {
                options.ConnectionString = Configuration.GetSection("Data:InternalDatabase").Get<string>();
            });

            services.Configure<TheMovieDbOptions>(options =>
            {
                options.Token = Configuration.GetSection("TMDb:Token").Get<string>();
            });

            return services;
        }
    }
}
