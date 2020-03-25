using Autofac;
using Microsoft.Extensions.Configuration;

namespace MediaRecommender.WebApi.App_Start
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder RegisterApplicationLayer(this ContainerBuilder builder, IConfiguration Configuration)
        {
            return builder;
        }

        public static ContainerBuilder RegisterDomainLayer(this ContainerBuilder builder, IConfiguration Configuration)
        {
            return builder;
        }

        public static ContainerBuilder RegisterDataLayer(this ContainerBuilder builder, IConfiguration Configuration)
        {
            return builder;
        }
    }
}
