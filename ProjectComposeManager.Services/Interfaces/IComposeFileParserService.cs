namespace ProjectComposeManager.Services.Interfaces
{
    using ProjectComposeManager.Services.Models;
    using System.Collections.Generic;

    public interface IComposeFileParserService
    {
        public ComposeModel ParseFullFile(string rawYamlFile);

        public ComposeServiceModel ParseService(string rawYamlService);

        public ComposeVolumeModel ParseVolume(string rawYamlService);
    }
}
