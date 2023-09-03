namespace ProjectComposeManager.Services.Interfaces
{
    using ProjectComposeManager.Services.Models;
    
    public interface IComposeFileBuilderService
    {
        public string BuildYaml(ComposeModel composeModel);

        public string BuildVolumeYaml(ComposeVolumeModel composeVolumeModel);

        public string BuildServiceYaml(ComposeServiceModel composeServiceModel);
    }
}
