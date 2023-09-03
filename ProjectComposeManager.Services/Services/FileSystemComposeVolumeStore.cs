namespace ProjectComposeManager.Services.Services
{
    using Microsoft.Extensions.Options;
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System.Text.Json;
    using System;
    using System.IO;
    using ProjectComposeManager.Services.Configuration;
    using System.Collections.Generic;
    using System.Linq;

    public class FileSystemComposeVolumeStore : IComposeVolumeStoreService
    {
        private readonly ModuleDefinitionConfiguration options;
        private readonly IComposeFileBuilderService composeFileBuilderService;
        private readonly IComposeFileParserService composeFileParserService;

        public FileSystemComposeVolumeStore(IOptions<ModuleDefinitionConfiguration> options, IComposeFileBuilderService composeFileBuilderService, IComposeFileParserService composeFileParserService)
        {
            this.options = options.Value;
            this.composeFileBuilderService = composeFileBuilderService;
            this.composeFileParserService = composeFileParserService;
        }

        public VolumeModuleModel[] GetAllVolumeMetaDataObjects()
        {
            DirectoryInfo servicesDirectory = new($"{options.Location}/Volumes");

            List<FileInfo> files = servicesDirectory.EnumerateFiles().ToList();

            VolumeModuleModel[] volumes = new VolumeModuleModel[files.Count];

            for (int i = 0; i < files.Count; i++)
            {
                volumes[i] = this.GetVolumeMetaData(files[i].Name.Replace(".json", string.Empty));
            }

            return volumes;
        }

        public ComposeVolumeModel GetComposeVolume(VolumeModuleModel serviceModuleModel)
        {
            string rawComposeServiceModel = File.ReadAllText(serviceModuleModel.ComposeFilePath);

            return this.composeFileParserService.ParseVolume(rawComposeServiceModel);
        }

        public ComposeVolumeModel GetComposeVolumeByName(string name)
        {
            VolumeModuleModel volumeMetaData = this.GetVolumeMetaData(name);

            return this.GetComposeVolume(volumeMetaData);
        }

        public VolumeModuleModel GetVolumeMetaData(string name)
        {
            string rawVolumeModuleModel = File.ReadAllText($"{options.Location}/Volumes/{name}.json");

            VolumeModuleModel volumeModuleModel = JsonSerializer.Deserialize<VolumeModuleModel>(rawVolumeModuleModel)
                                                                        ?? throw new Exception("Unable to deserialize volume module.");

            return volumeModuleModel;
        }

        public void Save(VolumeModuleModel modelToSave)
        {
            Directory.CreateDirectory($"{options.Location}/Volumes");

            File.WriteAllText($"{options.Location}/Volumes/{modelToSave.Name}.json", JsonSerializer.Serialize(modelToSave));
        }
    }
}
