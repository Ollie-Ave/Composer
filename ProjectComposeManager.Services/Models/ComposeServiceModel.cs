namespace ProjectComposeManager.Services.Models
{
    using ProjectComposeManager.Services.Constants;
    using System;
    using System.Collections.Generic;

    public record ComposeServiceModel
    {
        public string Name { get; init; } = string.Empty;

        public ComposeBuildModel? Build { get; init; }

        public string? Image { get; init; }

        public Dictionary<string, string> PortBindings { get; init; } = new();

        public string Restart { get; init; } = RestartOptionConstants.UnlessStopped;

        public Dictionary<string, string> EnvironmentVariables { get; init; } = new();

        public string HostName { get; init; } = string.Empty;

        public Dictionary<string, string> Labels { get; init; } = new();

        public string[] Volumes { get; init; } = Array.Empty<string>();
    }
}