namespace ProjectComposeManager.Services.Builders
{
    using ProjectComposeManager.Services.Constants;
    using ProjectComposeManager.Services.Models;
    using System;
    using System.Collections.Generic;

    public class ComposeServiceModelBuilder
    {
        private ComposeServiceModel internalServiceModel;

        public ComposeServiceModelBuilder()
        {
            this.internalServiceModel = new();
        }

        public ComposeServiceModelBuilder AddNameOption(string name)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                Name = name,
            };

            return this;
        }

        public ComposeServiceModelBuilder AddBuildOption(string context, string dockerFile)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                Build = new ComposeBuildModel()
                {
                    Context = context,
                    DockerFile = dockerFile,
                }
            };

            return this;
        }

        public ComposeServiceModelBuilder AddImageOption(string image)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                Image = image,
            };

            return this;
        }

        public ComposeServiceModelBuilder AddPortBindingOptions(Dictionary<string, string> portBindings)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                PortBindings = portBindings,
            };

            return this;
        }

        public ComposeServiceModelBuilder AddRestartOption(string restartOption)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                Restart = restartOption,
            };

            return this;
        }

        public ComposeServiceModelBuilder AddHostNameOption(string hostName)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                HostName = hostName,
            };

            return this;
        }

        public ComposeServiceModelBuilder AddEnvironmentOptions(Dictionary<string, string> environmentVariables)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                EnvironmentVariables = environmentVariables,
            };

            return this;
        }

        public ComposeServiceModelBuilder AddVolumeOptions(string[] composeVoumeNames)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                Volumes = composeVoumeNames
            };

            return this;
        }

        public ComposeServiceModelBuilder AddLabelsOptions(Dictionary<string, string> labels)
        {
            this.internalServiceModel = this.internalServiceModel with
            {
                Labels = labels,
            };

            return this;
        }

        public ComposeServiceModel Build()
        {
            this.ValidateModel(this.internalServiceModel);

            return this.internalServiceModel;
        }

        public void ValidateModel(ComposeServiceModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new InvalidOperationException("Unable to build a compose service without a name");
            }

            if (model.Build is null && model.Image is null)
            {
                throw new InvalidOperationException("Unable to build a compose service without either an image or a build option");
            }
            else if (model.Build is not null && model.Image is not null)
            {
                throw new InvalidOperationException("Unable to build a compose service with both an image and a build option");
            }

            if ((model.Build is not null) && (string.IsNullOrWhiteSpace(model.Build.Context) || string.IsNullOrWhiteSpace(model.Build.DockerFile)))
            {
                throw new InvalidOperationException("Cannot have a build option without a context and a dockerfile");
            }
            if (model.Restart != RestartOptionConstants.No &&
                model.Restart != RestartOptionConstants.Always &&
                model.Restart != RestartOptionConstants.OnFailure &&
                model.Restart != RestartOptionConstants.UnlessStopped)
            {
                throw new InvalidOperationException("Unable to build a compose service with an invalid restart option");
            }

            if (string.IsNullOrWhiteSpace(model.HostName))
            {
                throw new InvalidOperationException("Unable to build a compose service without a host name");
            }
        }
    }
}