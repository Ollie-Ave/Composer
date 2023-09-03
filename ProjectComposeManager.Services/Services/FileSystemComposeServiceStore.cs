namespace ProjectComposeManager.Services.Services
{
    using Microsoft.Extensions.Options;
    using ProjectComposeManager.Services.Configuration;
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    public class FileSystemComposeServiceStore : IComposeServiceStoreService
    {
        private readonly ModuleDefinitionConfiguration options;
        private readonly IComposeFileBuilderService composeFileBuilderService;
        private readonly IComposeFileParserService composeFileParserService;

        public FileSystemComposeServiceStore(IOptions<ModuleDefinitionConfiguration> options, IComposeFileBuilderService composeFileBuilderService, IComposeFileParserService composeFileParserService)
        {
            this.options = options.Value;
            this.composeFileBuilderService = composeFileBuilderService;
            this.composeFileParserService = composeFileParserService;
        }

        public ServiceModuleModel[] GetAllServiceMetaDataObjects()
        {
            DirectoryInfo servicesDirectory = new($"{options.Location}/Services");

            List<FileInfo> files = servicesDirectory.EnumerateFiles().ToList();

            ServiceModuleModel[] services = new ServiceModuleModel[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                services[i] = this.GetServiceMetaData(files[i].Name.Replace(".json", string.Empty));
            }

            return services;
        }

        public ServiceModuleModel GetServiceMetaData(string name)
        {
            string rawServiceModuleModel = File.ReadAllText($"{options.Location}/Services/{name}.json");

            ServiceModuleModel serviceModuleModel = JsonSerializer.Deserialize<ServiceModuleModel>(rawServiceModuleModel) 
                                                                        ?? throw new Exception("Unable to deserialize service module.");

            return serviceModuleModel;
        }

        public ComposeServiceModel GetComposeService(ServiceModuleModel serviceModuleModel, bool useLocalDockerImage)
        {
            string rawComposeServiceModel= File.ReadAllText(serviceModuleModel.ComposeFilePath);

            ComposeServiceModel composeServiceModel = this.composeFileParserService.ParseService(rawComposeServiceModel);

            if (useLocalDockerImage)
            {
                composeServiceModel = composeServiceModel with
                {
                    Build = serviceModuleModel.ComposeBuild,
                    Image = null,
                };
            }
            else
            {
                composeServiceModel = composeServiceModel with
                {
                    Image = serviceModuleModel.DockerImageUrl,
                    Build = null,
                };
            }

            return composeServiceModel;
        }

        public void Save(ServiceModuleModel modelToSave)
        {
            Directory.CreateDirectory($"{options.Location}/Services");

            File.WriteAllText($"{options.Location}/Services/{modelToSave.Name}.json", JsonSerializer.Serialize(modelToSave));
        }

        public ComposeServiceModel GetComposeServiceByName(string name, bool useLocal)
        {
            ServiceModuleModel serviceMetaData = this.GetServiceMetaData(name);

            return this.GetComposeService(serviceMetaData, useLocal);
        }
    }
}
