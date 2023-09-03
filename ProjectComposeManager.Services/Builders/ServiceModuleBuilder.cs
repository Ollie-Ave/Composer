namespace ProjectComposeManager.Services.Builders
{
    using ProjectComposeManager.Services.Models;
    using System;
    using System.Runtime.CompilerServices;

    public class ServiceModuleBuilder
    {
        private ServiceModuleModel internalServiceModuleModel;

        public ServiceModuleBuilder()
        {
            this.internalServiceModuleModel = new();
        }

        public ServiceModuleBuilder AddNameOption(string name)
        {
            this.internalServiceModuleModel = this.internalServiceModuleModel with
            {
                Name = name,
            };

            return this;
        }

        public ServiceModuleBuilder AddDockerImageUrlOption(string imageUrl)
        {
            this.internalServiceModuleModel = this.internalServiceModuleModel with
            {
                DockerImageUrl = imageUrl,
            };

            return this;
        }

        public ServiceModuleBuilder AddComposeFilePathOption(string composeFilePath)
        {
            this.internalServiceModuleModel = this.internalServiceModuleModel with
            {
                ComposeFilePath = composeFilePath,
            };

            return this;
        }

        public ServiceModuleBuilder AddComposeBuildOption(ComposeBuildModel composeBuild)
        {
            this.internalServiceModuleModel = this.internalServiceModuleModel with
            {
                ComposeBuild = composeBuild,
            };

            return this;
        }

        public ServiceModuleModel Build()
        {
            return this.internalServiceModuleModel;
        }
    }
}
