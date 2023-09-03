namespace ProjectComposeManager.Console.Services
{
    using Microsoft.Extensions.Logging;
    using ProjectComposeManager.Services.Builders;
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System.IO;

    public class AppService
    {
        private readonly IComposeFileParserService composeFileParserService;
        private readonly IComposeFileBuilderService composeFileBuilderService;
        private readonly IBuilderFactory<ComposeModelBuilder> composeModelBuilderFactory;
        private readonly IComposeServiceStoreService composeServiceStoreService;
        private readonly IComposeVolumeStoreService composeVolumeStoreService;
        private readonly ILogger<AppService> logger;

        public AppService(IComposeFileParserService composeFileParserService, IComposeFileBuilderService composeFileBuilderService, IBuilderFactory<ComposeModelBuilder> composeModelBuilderFactory, IComposeServiceStoreService composeServiceStore, IComposeVolumeStoreService composeVolumeStoreService, ILogger<AppService> logger)
        {
            this.composeFileParserService = composeFileParserService;
            this.composeFileBuilderService = composeFileBuilderService;
            this.composeModelBuilderFactory = composeModelBuilderFactory;
            this.composeServiceStoreService = composeServiceStore;
            this.composeVolumeStoreService = composeVolumeStoreService;
            this.logger = logger;
        }

        public void Run()
        {
            const string ImageResizer = "image-resizer";
            const string Yarp = "yarp";
            const string RabbitMQ = "rabbit-mq";
            const string ClusterMessaging = "cluster-messaging";
            const string Imagestore = "image-store";

            ComposeServiceModel[] services = new[]
            {
                this.composeServiceStoreService.GetComposeServiceByName(ImageResizer, true),
                this.composeServiceStoreService.GetComposeServiceByName(Yarp, false),
                this.composeServiceStoreService.GetComposeServiceByName(RabbitMQ, false),
                this.composeServiceStoreService.GetComposeServiceByName(ClusterMessaging, false),
            };

            ComposeVolumeModel[] volumes = new[]
            {
                this.composeVolumeStoreService.GetComposeVolumeByName(Imagestore),
            };

            ComposeModel composeModel = this.composeModelBuilderFactory.CreateBuilder()
                .AddVersionOption("3")
                .AddServiceOptions(services)
                .AddVolumeOptions(volumes)
                .Build();

            this.composeFileBuilderService.BuildYaml(composeModel);

            File.WriteAllText("C://Code/Testing/ComposeManagerTestingServices/test-compose.yml", this.composeFileBuilderService.BuildYaml(composeModel));



            ////string IRF = File.ReadAllText($"C://Code/Testing/ComposeManagerTestingServices/{IRP}");
            ////string YF = File.ReadAllText($"C://Code/Testing/ComposeManagerTestingServices/{YP}");
            ////string RF = File.ReadAllText($"C://Code/Testing/ComposeManagerTestingServices/{RP}");
            ////string CMF = File.ReadAllText($"C://Code/Testing/ComposeManagerTestingServices/{CMP}");
            ////string ISVF = File.ReadAllText($"C://Code/Testing/ComposeManagerTestingServices/{ISV}");

            ////ComposeServiceModel IR_CSM = this.composeFileParserService.ParseService(IRF);
            ////ComposeServiceModel Y_CSM = this.composeFileParserService.ParseService(YF);
            ////ComposeServiceModel R_CSM = this.composeFileParserService.ParseService(RF);
            ////ComposeServiceModel CM_CSM = this.composeFileParserService.ParseService(CMF);

            ////this.composeServiceStoreService.Save(IR_CSM, module);

            ////this.composeServiceStoreService.Save(Y_CSM, Y_CSM_Module);
            ////this.composeServiceStoreService.Save(R_CSM, R_CSM_Module);
            ////this.composeServiceStoreService.Save(CM_CSM, CM_CSM_Module);

            ////ComposeServiceModel[] services = new ComposeServiceModel[]            ////{
            ////    IR_CSM,
            ////    Y_CSM,
            ////    R_CSM,
            ////    CM_CSM,
            ////};





            ////ComposeVolumeModel ISV_CVM = this.composeFileParserService.ParseVolume(ISVF);
            ////this.composeVolumeStoreService.Save(ISV_CVM, volumeModule);

            ////ComposeVolumeModel[] volumes = new ComposeVolumeModel[]
            ////{
            ////    ISV_CVM,
            ////};

            ////ComposeModelBuilder composeModelBuilder = this.composeModelBuilderFactory.CreateBuilder();

            ////composeModelBuilder.AddVersionOption("3");
            ////composeModelBuilder.AddServiceOptions(services);
            ////composeModelBuilder.AddVolumeOptions(volumes);

            ////string builtYaml = this.composeFileBuilderService.BuildYaml(composeModelBuilder.Build());

            ////File.WriteAllText($"C://Code/Testing/ComposeManagerTestingServices/newYamlFile.yml", builtYaml);
        }
    }
}