namespace ProjectComposeManager.Services.Models
{
    using System;

    public record ComposeModel
    {
        public string Version { get; init; } = "3.9";

        public string Name { get; init; } = string.Empty;

        public ComposeServiceModel[] Services { get; init; } = Array.Empty<ComposeServiceModel>();

        public ComposeVolumeModel[] Volumes { get; init; } = Array.Empty<ComposeVolumeModel>();
    }
}