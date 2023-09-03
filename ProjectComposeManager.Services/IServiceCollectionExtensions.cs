namespace Microsoft.Extensions.DependencyInjection
{
    using Microsoft.Extensions.Configuration;
    using ProjectComposeManager.Services.Builders;
    using ProjectComposeManager.Services.Configuration;
    using ProjectComposeManager.Services.Factories;
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using ProjectComposeManager.Services.Services;

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddComposeManagerServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IComposeFileParserService, ComposeFileParserService>();
            services.AddSingleton<IComposeFileBuilderService, ComposeFileBuilderService>();

            services.AddSingleton<IBuilderFactory<ComposeModelBuilder>, BuilderFactory<ComposeModelBuilder>>();
            services.AddSingleton<IBuilderFactory<ComposeServiceModelBuilder>, BuilderFactory<ComposeServiceModelBuilder>>();
            services.AddSingleton<IBuilderFactory<ComposeVolumeModelBuilder>, BuilderFactory<ComposeVolumeModelBuilder>>();
            services.AddSingleton<IBuilderFactory<ServiceModuleBuilder>, BuilderFactory<ServiceModuleBuilder>>();
            services.AddSingleton<IBuilderFactory<VolumeModelBuilder>, BuilderFactory<VolumeModelBuilder>>();

            services.AddSingleton<IComposeServiceStoreService, FileSystemComposeServiceStore>();
            services.AddSingleton<IComposeVolumeStoreService, FileSystemComposeVolumeStore>();

            services.Configure<ModuleDefinitionConfiguration>(configuration.GetSection(nameof(ModuleDefinitionConfiguration)));

            return services;
        }
    }
}
