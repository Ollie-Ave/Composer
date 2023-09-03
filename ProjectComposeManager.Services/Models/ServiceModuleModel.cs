namespace ProjectComposeManager.Services.Models
{
    public record ServiceModuleModel
    {
        public string Name { get; init; } = string.Empty;

        public string DockerImageUrl { get; init; } = string.Empty;

        public ComposeBuildModel ComposeBuild { get; init; } = new();

        public string ComposeFilePath { get; init; } = string.Empty;
    }
}