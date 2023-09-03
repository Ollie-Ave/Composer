namespace ProjectComposeManager.Services.Services
{
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System;
    using System.Linq;
    using System.Text;

    public class ComposeFileBuilderService : IComposeFileBuilderService
    {
        const string tabChar = "  ";

        public string BuildYaml(ComposeModel composeModel)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine($"version: '{composeModel.Version}'");

            if (!string.IsNullOrWhiteSpace(composeModel.Name))
            {
                stringBuilder.AppendLine($"name: '{composeModel.Name}'");
            }

            stringBuilder.AppendLine("services:");

            foreach(ComposeServiceModel composeServiceModel in composeModel.Services)
            {
                stringBuilder = this.BuildService(composeServiceModel, stringBuilder, 1);
            }

            stringBuilder.AppendLine("volumes:");

            foreach(ComposeVolumeModel composeVolumeModel in composeModel.Volumes)
            {
                stringBuilder = this.BuildVolume(composeVolumeModel, stringBuilder, 1);
            }

            return stringBuilder.ToString();
        }

        public string BuildServiceYaml(ComposeServiceModel composeServiceModel)
        {
            StringBuilder stringBuilder = new();

            this.BuildService(composeServiceModel, stringBuilder, 0);

            return stringBuilder.ToString();
        }

        public string BuildVolumeYaml(ComposeVolumeModel composeVolumeModel)
        {
            StringBuilder stringBuilder = new();

            this.BuildVolume(composeVolumeModel, stringBuilder, 0);

            return stringBuilder.ToString();
        }

        private StringBuilder BuildVolume(ComposeVolumeModel composeVolumeModel, StringBuilder stringBuilder, int baseIndentationLevel)
        {
            string baseIndentation = string.Concat(Enumerable.Repeat(tabChar, baseIndentationLevel));

            stringBuilder.AppendLine($"{baseIndentation}{composeVolumeModel.Name}:");

            if (!string.IsNullOrWhiteSpace(composeVolumeModel.CustomName))
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}name: {composeVolumeModel.CustomName}");
            }

            stringBuilder.AppendLine($"{baseIndentation}{tabChar}labels:");

            foreach((string key, string value) in composeVolumeModel.Labels)
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}{key}: {value}");
            }

            if (composeVolumeModel.DriverOptions?.Count > 0)
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}driver_opts:");
                
                foreach ((string key, string value) in composeVolumeModel.DriverOptions)
                {
                    stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}{key}: {value}");
                }
            }
            else if (!string.IsNullOrWhiteSpace(composeVolumeModel.Driver))
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}driver: {composeVolumeModel.Driver}");
            }

            if (!string.IsNullOrWhiteSpace(composeVolumeModel.External.ToString()))
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}external: {composeVolumeModel.External.ToString().ToLowerInvariant()}");
            }

            return stringBuilder;
        }

        private StringBuilder BuildService(ComposeServiceModel composeServiceModel, StringBuilder stringBuilder, int baseIndentationLevel)
        {
            string baseIndentation = string.Concat(Enumerable.Repeat(tabChar, baseIndentationLevel));

            stringBuilder.AppendLine($"{baseIndentation}{composeServiceModel.Name}:");

            if (!string.IsNullOrWhiteSpace(composeServiceModel.Image))
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}image: {composeServiceModel.Image}");
            }
            else if (composeServiceModel.Build is not null)
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}build:");
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}context: {composeServiceModel.Build.Context}");
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}dockerfile: {composeServiceModel.Build.DockerFile}");
            }
            else
            {
                throw new InvalidOperationException("Either image or build must be specified");
            }

            if (composeServiceModel.PortBindings.Count > 0)
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}ports:");
                
                foreach((string exposedPort, string internalPort) in composeServiceModel.PortBindings)
                {
                    stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}- {exposedPort}:{internalPort}");
                }
            }

            if (composeServiceModel.EnvironmentVariables.Count > 0)
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}environment:");

                foreach((string key, string value) in composeServiceModel.EnvironmentVariables)
                {
                    stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}{key}: {value}");
                }
            }

            if (!string.IsNullOrWhiteSpace(composeServiceModel.HostName))
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}hostname: {composeServiceModel.HostName}");
            }

            if (composeServiceModel.Labels.Count > 0)
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}labels:");

                foreach((string key, string value) in composeServiceModel.Labels)
                {
                    stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}{key}: {value}");
                }
            }

            if (composeServiceModel.Volumes.Length > 0)
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}volumes:");

                foreach(string volume in composeServiceModel.Volumes)
                {
                    stringBuilder.AppendLine($"{baseIndentation}{tabChar}{tabChar}- {volume}");
                }
            }

            if (!string.IsNullOrWhiteSpace(composeServiceModel.Restart))
            {
                stringBuilder.AppendLine($"{baseIndentation}{tabChar}restart: {composeServiceModel.Restart}");
            }

            return stringBuilder;
        }
    }
}
