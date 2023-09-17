namespace ProjectComposeManager.Services.Services
{
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System;
    using System.Text;

    public class ComposeFileBuilderService : IComposeFileBuilderService
    {
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
            stringBuilder.AppendLineWithIndentation(baseIndentationLevel, $"{composeVolumeModel.Name}:");
            
            int indentationLevel = baseIndentationLevel + 1;
            int secondIndentationLevel = baseIndentationLevel + 2;

            if (!string.IsNullOrWhiteSpace(composeVolumeModel.CustomName))
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, $"name: {composeVolumeModel.CustomName}");
            }

            stringBuilder.AppendLineWithIndentation(indentationLevel, $"labels:");

            foreach((string key, string value) in composeVolumeModel.Labels)
            {
                stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"{key}: {value}");
            }

            if (composeVolumeModel.DriverOptions?.Count > 0)
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, "driver_opts:");
                
                foreach ((string key, string value) in composeVolumeModel.DriverOptions)
                {
                    stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"{key}: {value}");
                }
            }
            else if (!string.IsNullOrWhiteSpace(composeVolumeModel.Driver))
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, $"driver: {composeVolumeModel.Driver}");
            }

            if (!string.IsNullOrWhiteSpace(composeVolumeModel.External.ToString()))
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, $"external: {composeVolumeModel.External}");
            }

            return stringBuilder;
        }

        private StringBuilder BuildService(ComposeServiceModel composeServiceModel, StringBuilder stringBuilder, int baseIndentationLevel)
        {
            int indentationLevel = baseIndentationLevel + 1;
            int secondIndentationLevel = baseIndentationLevel + 2;

            stringBuilder.AppendLineWithIndentation(baseIndentationLevel, $"{composeServiceModel.Name}:");

            if (!string.IsNullOrWhiteSpace(composeServiceModel.Image))
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, $"image: {composeServiceModel.Image}");
            }
            else if (composeServiceModel.Build is not null)
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, $"build:");
                stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"context: {composeServiceModel.Build.Context}");
                stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"dockerfile: {composeServiceModel.Build.DockerFile}");
            }
            else
            {
                throw new InvalidOperationException("Either image or build must be specified");
            }

            if (composeServiceModel.PortBindings.Count > 0)
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, "ports:");
                
                foreach((string exposedPort, string internalPort) in composeServiceModel.PortBindings)
                {
                    stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"- {exposedPort}:{internalPort}");
                }
            }

            if (composeServiceModel.EnvironmentVariables.Count > 0)
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, "environment:");

                foreach((string key, string value) in composeServiceModel.EnvironmentVariables)
                {
                    stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"{key}: {value}");
                }
            }

            if (!string.IsNullOrWhiteSpace(composeServiceModel.HostName))
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, $"hostname: {composeServiceModel.HostName}");
            }

            if (composeServiceModel.Labels.Count > 0)
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, "labels:");

                foreach((string key, string value) in composeServiceModel.Labels)
                {
                    stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"{key}: {value}");
                }
            }

            if (composeServiceModel.Volumes.Length > 0)
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, "volumes:");

                foreach(string volume in composeServiceModel.Volumes)
                {
                    stringBuilder.AppendLineWithIndentation(secondIndentationLevel, $"- {volume}");
                }
            }

            if (!string.IsNullOrWhiteSpace(composeServiceModel.Restart))
            {
                stringBuilder.AppendLineWithIndentation(indentationLevel, $"restart: {composeServiceModel.Restart}");
            }

            return stringBuilder;
        }
    }
}
