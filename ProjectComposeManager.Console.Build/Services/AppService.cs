namespace ProjectComposeManager.Console.Build.Services
{
    using ProjectComposeManager.Console.Build.Models;
    using ProjectComposeManager.Services.Builders;
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using Spectre.Console;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class AppService
    {
        const string defaultComposeVersion = "3.8";
        const string outputDirectory = "C://ComposeFiles";

        private readonly IComposeServiceStoreService composeServiceStoreService;
        private readonly IComposeVolumeStoreService composeVolumeStoreService;
        private readonly IBuilderFactory<ComposeModelBuilder> composeModelBuilderFactory;
        private readonly IComposeFileBuilderService composeFileBuilderService;

        public AppService(
            IComposeServiceStoreService composeServiceStoreService, 
            IComposeVolumeStoreService composeVolumeStoreService,
            IBuilderFactory<ComposeModelBuilder> composeModelBuilderFactory,
            IComposeFileBuilderService composeFileBuilderService)
        {
            this.composeServiceStoreService = composeServiceStoreService;
            this.composeVolumeStoreService = composeVolumeStoreService;
            this.composeModelBuilderFactory = composeModelBuilderFactory;
            this.composeFileBuilderService = composeFileBuilderService;
        }

        public void Run()
        {
            FigletText titleText = new FigletText("Composer")
                .LeftJustified()
                .Color(Color.Blue);

            AnsiConsole.Write(titleText);

            AnsiConsole.WriteLine();
            string fileName = AnsiConsole.Ask<string>("Enter the [green]name[/] of the outputted file.").Replace(".yml", string.Empty);

            List<ServiceSelectionModel> servicesToBuild = this.GetSelectedServices();

            List<string> volumesToBuild = this.GetSelectedVolumes();

            string builtFile = this.BuildYamlFile(servicesToBuild, volumesToBuild, fileName);

            this.WriteOutFile(fileName, builtFile);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[green]Completed![/] The file is available at: [green]{outputDirectory}/{fileName}.yml[/]");
        }

        private void WriteOutFile(string fileName, string file)
        {
            Directory.CreateDirectory(outputDirectory);

            File.WriteAllText($"{outputDirectory}/{fileName}.yml", file);
        }

        private List<ServiceSelectionModel> ConvertToServiceSelectionModel(List<string> chosenServices, List<string> servicesToBuildLocally)
        {
            List<ServiceSelectionModel> selectedServices = new();

            foreach (string serviceToAdd in chosenServices)
            {
                selectedServices.Add(new()
                {
                    Name = serviceToAdd,
                    UseLocal = servicesToBuildLocally.Contains(serviceToAdd),
                });
            }

            return selectedServices;
        }

        private List<string> GetSelectedVolumes()
        {
            VolumeModuleModel[] volumes = this.composeVolumeStoreService.GetAllVolumeMetaDataObjects();

            MultiSelectionPrompt<string> volumesPrompt = new MultiSelectionPrompt<string>()
                .Title($"Select the volumes you want to build")
                .InstructionsText($"Select the volumes you want to build")
                .AddChoices(volumes.Select(v => v.Name));

            return AnsiConsole.Prompt(volumesPrompt);
        }

        private List<ServiceSelectionModel> GetSelectedServices()
        {
            ServiceModuleModel[] services = this.composeServiceStoreService.GetAllServiceMetaDataObjects();

            MultiSelectionPrompt<string> servicePrompt = new MultiSelectionPrompt<string>()
                .Title($"Select the service you want to build")
                .InstructionsText($"Select the service you want to build")
                .AddChoices(services.Select(s => s.Name));

            List<string> chosenServices = AnsiConsole.Prompt(servicePrompt);

            MultiSelectionPrompt<string> useLocalServiceBuildPrompt = new MultiSelectionPrompt<string>()
                .Title("Select the services you wish to build locally.")
                .InstructionsText($"[grey](Press [blue]<space>[/] to toggle a service, [green]<enter>[/] to accept)[/]")
                .AddChoices(chosenServices);

            List<string> servicesToBuildLocally = AnsiConsole.Prompt(useLocalServiceBuildPrompt);

            return this.ConvertToServiceSelectionModel(chosenServices, servicesToBuildLocally);
        }

        private string BuildYamlFile(List<ServiceSelectionModel> servicesToAdd, List<string> volumesToAdd, string fileName)
        {
            ComposeServiceModel[] composeServiceModels = new ComposeServiceModel[servicesToAdd.Count];

            for (int i = 0; i < servicesToAdd.Count; i++)
            {
                composeServiceModels[i] = this.composeServiceStoreService.GetComposeServiceByName(servicesToAdd[i].Name, servicesToAdd[i].UseLocal);
            }

            ComposeVolumeModel[] composeVolumeModels = new ComposeVolumeModel[volumesToAdd.Count];

            for (int i = 0; i < volumesToAdd.Count; i++)
            {
                composeVolumeModels[i] = this.composeVolumeStoreService.GetComposeVolumeByName(volumesToAdd[i]);
            }


            ComposeModel composeFileToBuild = this.composeModelBuilderFactory.CreateBuilder()
                .AddNameOption(fileName)
                .AddVersionOption(defaultComposeVersion)
                .AddServiceOptions(composeServiceModels)
                .AddVolumeOptions(composeVolumeModels)
                .Build();

            return this.composeFileBuilderService.BuildYaml(composeFileToBuild);
        }
    }
}
