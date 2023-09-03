namespace ProjectComposeManager.Console.Build.Models
{
    internal record ServiceSelectionModel
    {
        public string Name { get; init; } = string.Empty;

        public bool UseLocal { get; init; }
    }
}
