namespace ProjectComposeManager.Services.Models
{
    public record ComposeBuildModel
    {
        public string Context { get; init; } = string.Empty;

        public string DockerFile { get; init; } = string.Empty;
    }
}