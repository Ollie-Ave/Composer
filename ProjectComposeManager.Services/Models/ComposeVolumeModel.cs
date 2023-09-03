namespace ProjectComposeManager.Services.Models
{
    using System.Collections.Generic;

    public record ComposeVolumeModel
    {
        public string Name { get; init; } = string.Empty;

        public string CustomName { get; init; } = string.Empty;

        public string? Driver { get; init; }

        public Dictionary<string, string>? DriverOptions { get; init; }

        public bool External { get; init; }

        public Dictionary<string, string> Labels { get; init; } = new();
    }
}