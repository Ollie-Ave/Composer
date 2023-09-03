namespace ProjectComposeBuilder.Console.AddNew.Services
{
    using ProjectComposeManager.Services.Builders;
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using Spectre.Console;

    internal class AppService
    {
        private readonly IBuilderFactory<ServiceModuleBuilder> serviceModuleBuilderFactory;
        private readonly IBuilderFactory<VolumeModelBuilder> volumeModuleModelBuilderFactory;
        private readonly IComposeServiceStoreService composeServiceStoreService;
        private readonly IComposeVolumeStoreService composeVolumeStoreService;

        public AppService(
            IBuilderFactory<ServiceModuleBuilder> serviceModuleBuilderFactory,
            IBuilderFactory<VolumeModelBuilder> volumeModuleModelBuilderFactory,
            IComposeServiceStoreService composeServiceStoreService,
            IComposeVolumeStoreService composeVolumeStoreService)
        {
            this.serviceModuleBuilderFactory = serviceModuleBuilderFactory;
            this.volumeModuleModelBuilderFactory = volumeModuleModelBuilderFactory;
            this.composeServiceStoreService = composeServiceStoreService;
            this.composeVolumeStoreService = composeVolumeStoreService;
        }

        public void Run()
        {
            FigletText titleText = new FigletText("Composer")
                .LeftJustified()
                .Color(Color.Blue);

            AnsiConsole.Write(titleText);
            AnsiConsole.WriteLine();

            SelectionPrompt<string> typePrompt = new SelectionPrompt<string>()
                .Title("Would you like to add a [green]service[/] or a [blue]volume[/]?")
                .AddChoices(new[] { "Service", "Volume" });

            string type = AnsiConsole.Prompt(typePrompt);

            if (type == "Service")
            {
                this.AddService();
            }
            else
            {
                this.AddVolume();
            }
        }

        private void AddService()
        {
            string serviceName = AnsiConsole.Ask<string>("Enter the [green]name[/] of the service.");
            string composeFilePath = AnsiConsole.Ask<string>("Enter the [green]file path[/] of the [green]base compose file[/] for the service.");
            string dockerImageUrl = AnsiConsole.Ask<string>("Enter the [green]docker image url[/] of the service.");

            string context = AnsiConsole.Ask<string>("Enter the [green]build context location[/] for the service.");
            string dockerFile = AnsiConsole.Ask<string>("Enter the [green]docker file[/] of the service.");

            ComposeBuildModel buildSettings = new ComposeBuildModel()
            {
                Context = context,
                DockerFile = dockerFile,
            };

            ServiceModuleModel serviceModuleModel = this.serviceModuleBuilderFactory.CreateBuilder()
                .AddNameOption(serviceName)
                .AddComposeFilePathOption(composeFilePath)
                .AddDockerImageUrlOption(dockerImageUrl)
                .AddComposeBuildOption(buildSettings)
                .Build();

            this.composeServiceStoreService.Save(serviceModuleModel);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"Added service [green]{serviceName}[/] successfully!");
        }

        private void AddVolume()
        {
            string volumeName = AnsiConsole.Ask<string>("Enter the [green]name[/] of the volume.");
            string composeFilePath = AnsiConsole.Ask<string>("Enter the [green]file path[/] of the [green]base compose file[/] for the volume.");

            VolumeModuleModel volumeModuleModel = this.volumeModuleModelBuilderFactory.CreateBuilder()
                .AddNameOption(volumeName)
                .AddComposeFilePathOption(composeFilePath)
                .Build();

            this.composeVolumeStoreService.Save(volumeModuleModel);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"Added volume [green]{volumeName}[/] successfully!");
        }
    }
}
