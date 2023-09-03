namespace ProjectComposeManager.Services.Builders
{
    using ProjectComposeManager.Services.Models;

    public class VolumeModelBuilder
    {
        private VolumeModuleModel internalVolumeModuleModel;

        public VolumeModelBuilder()
        {
            this.internalVolumeModuleModel = new VolumeModuleModel();
        }

        public VolumeModelBuilder AddNameOption(string name)
        {
            this.internalVolumeModuleModel = this.internalVolumeModuleModel with
            {
                Name = name,
            };

            return this;
        }

        public VolumeModelBuilder AddComposeFilePathOption(string composeFilePath)
        {
            this.internalVolumeModuleModel = this.internalVolumeModuleModel with
            {
                ComposeFilePath = composeFilePath,
            };

            return this;
        }

        public VolumeModuleModel Build()
        {
            return this.internalVolumeModuleModel;
        }
    }
}
