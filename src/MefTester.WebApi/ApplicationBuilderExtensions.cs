using System;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using MefTester.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MefTester.WebApi
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddPlugins(this IServiceCollection services, string path = null)
        {
            var conventions = new ConventionBuilder();
            conventions
                .ForTypesDerivedFrom<IPlugin>()
                .Export<IPlugin>()
                .Shared();

            path = path ?? AppContext.BaseDirectory;

            var configuration = new ContainerConfiguration()
                .WithAssembliesInPath(path, conventions);

            using (var container = configuration.CreateContainer())
            {
                var plugins = container.GetExports<IPlugin>();

                foreach (var plugin in plugins)
                {
                    plugin.RegisterServices(services);
                }

            }

            return services;
        }


    }

    public static class ContainerConfigurationExtensions
    {
        public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return WithAssembliesInPath(configuration, path, null, searchOption);
        }
        public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, AttributedModelProvider conventions, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            var assemblyFiles = Directory.GetFiles(path, "*.dll", searchOption);
            var assemblies = assemblyFiles.Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
            configuration = configuration.WithAssemblies(assemblies, conventions); return configuration;
        }
    }
}