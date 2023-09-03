namespace ProjectComposeManager.Services.Builders
{
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System;
    using System.IO;

    public class ComposeModelBuilder
    {
        private ComposeModel internalComposeModel;

        public ComposeModelBuilder()
        {
            this.internalComposeModel = new();
        }

        public ComposeModelBuilder AddNameOption(string name)
        {
            this.internalComposeModel = this.internalComposeModel with
            {
                Name = name,
            };

            return this;
        }

        public ComposeModelBuilder AddVersionOption(string version)
        {
            this.internalComposeModel = this.internalComposeModel with
            {
                Version = version,
            };

            return this;
        }

        public ComposeModelBuilder AddServiceOptions(ComposeServiceModel[] services)
        {
            this.internalComposeModel = this.internalComposeModel with
            {
                Services = services,
            };

            return this;
        }

        public ComposeModelBuilder AddVolumeOptions(ComposeVolumeModel[] volumes)
        {
            this.internalComposeModel = this.internalComposeModel with
            {
                Volumes = volumes,
            };

            return this;
        }

        public void ValidateModel(ComposeModel model)
        {
            if (model.Services.Length == 0)
            {
                throw new InvalidDataException("No services found in the compose file.");
            }
        }

        public ComposeModel Build()
        {
            this.ValidateModel(this.internalComposeModel);

            return this.internalComposeModel;
        }
    }
}
