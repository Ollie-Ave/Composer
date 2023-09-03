namespace ProjectComposeManager.Console.Edit.Services
{
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using Spectre.Console;
    using Spectre.Console.Json;
    using System.Linq;
    using System.Text.Json;

    public class AppService
    {
        private readonly IComposeServiceStoreService composeServiceStoreService;
        private readonly IComposeVolumeStoreService composeVolumeStoreService;

        public AppService(
            IComposeServiceStoreService composeServiceStoreService, 
            IComposeVolumeStoreService composeVolumeStoreService)
        {
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
                .Title("Would you like to edit a [green]service[/] or a [blue]volume[/]?")
                .AddChoices(new[] { "Service", "Volume" });

            string type = AnsiConsole.Prompt(typePrompt);

            if (type == "Service")
            {
                this.EditService();
            }
            else
            {
                this.EditVolume();
            }
        }

        private void EditService()
        {
            ServiceModuleModel[] services = this.composeServiceStoreService.GetAllServiceMetaDataObjects();

            SelectionPrompt<string> servicePrompt = new SelectionPrompt<string>()
                .Title($"Select the service you want to build")
                .AddChoices(services.Select(s => s.Name));

            string selectedModuleName = AnsiConsole.Prompt(servicePrompt);

            ServiceModuleModel selectedModule = services.First(s => s.Name == selectedModuleName);

            JsonText json = new(JsonSerializer.Serialize(selectedModule));

            AnsiConsole.Write(
                new Panel(json)
                    .Header("The Current Module Settings:")
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow));

            SelectionPrompt<string> propertyToEditPrompt = new SelectionPrompt<string>()
                .Title($"Select the property you want to edit")
                .AddChoices(new[] { "Name", "Docker Image Url", "Compose File Path", "Build Context", "Docker File" });

            string propertyToEdit = AnsiConsole.Prompt(propertyToEditPrompt);

            AnsiConsole.WriteLine();
            string newValue = AnsiConsole.Ask<string>($"Enter the [green]new value[/] for the [green]{propertyToEdit}[/]:");

            selectedModule = this.MapServicePropertyToEdit(propertyToEdit, newValue, selectedModule);

            this.composeServiceStoreService.Save(selectedModule);
        }

        private void EditVolume()
        {
            VolumeModuleModel[] volumes = this.composeVolumeStoreService.GetAllVolumeMetaDataObjects();

            SelectionPrompt<string> volumePrompt = new SelectionPrompt<string>()
                .Title($"Select the volume you want to build")
                .AddChoices(volumes.Select(s => s.Name));

            string selectedModuleName = AnsiConsole.Prompt(volumePrompt);

            VolumeModuleModel selectedModule = volumes.First(s => s.Name == selectedModuleName);

            JsonText json = new(JsonSerializer.Serialize(selectedModule));

            AnsiConsole.Write(
                new Panel(json)
                    .Header("The Current Module Settings:")
                    .Collapse()
                    .RoundedBorder()
                    .BorderColor(Color.Yellow));

            SelectionPrompt<string> propertyToEditPrompt = new SelectionPrompt<string>()
                .Title($"Select the property you want to edit")
                .AddChoices(new[] { "Name", "Compose File Path" });

            string propertyToEdit = AnsiConsole.Prompt(propertyToEditPrompt);

            AnsiConsole.WriteLine();
            string newValue = AnsiConsole.Ask<string>($"Enter the [green]new value[/] for the [green]{propertyToEdit}[/]:");

            selectedModule = this.MapVolumePropertyToEdit(propertyToEdit, newValue, selectedModule);

            this.composeVolumeStoreService.Save(selectedModule);
        }

        private VolumeModuleModel MapVolumePropertyToEdit(string propertyToEdit, string newValue, VolumeModuleModel selectedModule)
        {
            switch (propertyToEdit)
            {
                case "Name":
                    selectedModule = selectedModule with { Name = newValue };
                    break;
                case "Compose File Path":
                    selectedModule = selectedModule with { ComposeFilePath = newValue };
                    break;
            }

            return selectedModule;
        }

        private ServiceModuleModel MapServicePropertyToEdit(string propertyToEdit, string newValue, ServiceModuleModel selectedModule)
        {
            switch (propertyToEdit)
            {
                case "Name":
                    selectedModule = selectedModule with { Name = newValue };
                    break;
                case "Docker Image Url":
                    selectedModule = selectedModule with { DockerImageUrl = newValue };
                    break;
                case "Compose File Path":
                    selectedModule = selectedModule with { ComposeFilePath = newValue };
                    break;
                case "Build Context":
                    selectedModule = selectedModule with { ComposeBuild = selectedModule.ComposeBuild with { Context = newValue } };
                    break;
                case "Docker File":
                    selectedModule = selectedModule with { ComposeBuild = selectedModule.ComposeBuild with { DockerFile = newValue } };
                    break;
            }

            return selectedModule;
        }
    }
}