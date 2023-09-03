namespace ProjectComposeManager.Services.Models
{
    public record VolumeModuleModel
    {
        public string Name { get; init; } = string.Empty;

        public string ComposeFilePath { get; init; } = string.Empty;
    }
}