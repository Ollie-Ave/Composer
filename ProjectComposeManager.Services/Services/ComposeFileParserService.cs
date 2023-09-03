namespace ProjectComposeManager.Services.Services
{
    using ProjectComposeManager.Services.Builders;
    using ProjectComposeManager.Services.Extensions;
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    public class ComposeFileParserService : IComposeFileParserService
    {
        const string tabChar = "  ";

        private readonly IBuilderFactory<ComposeModelBuilder> composeModelBuilderFactory;
        private readonly IBuilderFactory<ComposeServiceModelBuilder> composeServiceModelFactory;
        private readonly IBuilderFactory<ComposeVolumeModelBuilder> composeVolumeModelBuilderFactory;

        public ComposeFileParserService(IBuilderFactory<ComposeModelBuilder> composeModelBuilderFactory,
                                        IBuilderFactory<ComposeServiceModelBuilder> composeServiceModelFactory, 
                                        IBuilderFactory<ComposeVolumeModelBuilder> composeVolumeModelBuilderFactory)
        {
            this.composeModelBuilderFactory = composeModelBuilderFactory;
            this.composeServiceModelFactory = composeServiceModelFactory;
            this.composeVolumeModelBuilderFactory = composeVolumeModelBuilderFactory;
        }

        public ComposeModel ParseFullFile(string rawYamlFile)
        {
            List<string> lines = rawYamlFile.Split("\n").ToList();
            ComposeModelBuilder composeFileBuilder = this.composeModelBuilderFactory.CreateBuilder();

            Dictionary<string, int> topLevelDeclarations = lines
                .Where(line => !line.StartsWith(tabChar, StringComparison.InvariantCultureIgnoreCase) && line.IndexOf(':') != -1)
                .Select(line => (line.Split(':')[0], lines.IndexOf(line)))
                .ToDictionary(k => k.Item1, v => v.Item2);

            int lineNumber;

            if (topLevelDeclarations.TryGetValue("version", out lineNumber))
            {
                string version = lines[lineNumber].Split(":")[1].RemoveDoubleAndSingleQuotes();

                composeFileBuilder.AddVersionOption(version);
            }

            if (topLevelDeclarations.TryGetValue("name", out lineNumber))
            {
                string name = lines[lineNumber].Split(":")[1].RemoveDoubleAndSingleQuotes();

                composeFileBuilder.AddNameOption(name);
            }

            if (topLevelDeclarations.TryGetValue("services", out lineNumber))
            {
                List<string> servicesSectionLines = new()
                {
                    lines[lineNumber],
                };

                servicesSectionLines.AddRange(lines
                                                        .Skip(lineNumber + 1)
                                                        .TakeWhile(l => l.StartsWith(tabChar, StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrWhiteSpace(l))
                                                        .ToList());


                int currentIndentationLevel = 1;

                ComposeServiceModel[] composeServiceModels = this.ParseServices(servicesSectionLines, currentIndentationLevel).ToArray();

                composeFileBuilder.AddServiceOptions(composeServiceModels);
            }

            if (topLevelDeclarations.TryGetValue("volumes", out lineNumber))
            {
                List<string> volumeSectionLines = new()
                {
                    lines[lineNumber],
                };

                volumeSectionLines.AddRange(lines
                                                        .Skip(lineNumber + 1)
                                                        .TakeWhile(l => l.StartsWith(tabChar, StringComparison.InvariantCultureIgnoreCase))
                                                        .ToList());

                int currentIndentationLevel = 1;

                ComposeVolumeModel[] composeVolumeModels = this.ParseVolumes(volumeSectionLines, currentIndentationLevel).ToArray();

                composeFileBuilder.AddVolumeOptions(composeVolumeModels);
            }

            return composeFileBuilder.Build();
        }

        public ComposeServiceModel ParseService(string rawYamlService)
        {
            return this.ParseServices(rawYamlService.Split("\n").ToList(), 0).First();
        }

        public ComposeVolumeModel ParseVolume(string rawYamlService)
        {
            return this.ParseVolumes(rawYamlService.Split("\n").ToList(), 0).First();
        }

        private List<ComposeVolumeModel> ParseVolumes(List<string> lines, int currentIndentationLevel)
        {
            List<ComposeVolumeModel> composeVolumeModels = new();

            List<int> volumeDeclarationLineNumbers = lines
                .Where(line => line.HasIndentationOf(currentIndentationLevel))
                .Select(line => lines.IndexOf(line))
                .ToList();


            foreach (int lineNumber in volumeDeclarationLineNumbers)
            {
                ComposeVolumeModelBuilder volumeModelBuilder = this.composeVolumeModelBuilderFactory.CreateBuilder();

                List<string> volumeSectionLines = lines
                    .Skip(lineNumber + 1)
                    .TakeWhile(l => l.HasIndentationOfAtLeast(currentIndentationLevel + 1))
                    .ToList();

                volumeModelBuilder.AddNameOption(lines[lineNumber]
                                                        .Remove(lines[lineNumber].IndexOf(":"))
                                                        .RemoveDoubleAndSingleQuotes());

                if (this.TryParseSingleLineOption(volumeSectionLines, "name", out string? customNameOption))
                {
                    volumeModelBuilder.AddCustomNameOption(customNameOption);
                }

                if (this.TryParseSingleLineOption(volumeSectionLines, "driver", out string? driverOption))
                {
                    volumeModelBuilder.AddDriverOption(driverOption);
                }
                else if (this.TryParseKeyValueSection(volumeSectionLines, "driver_opts", currentIndentationLevel, out Dictionary<string, string>? driverOptions))
                {
                    volumeModelBuilder.AddDriverOptions(driverOptions);
                }
                else
                {
                    throw new InvalidOperationException("Volume declaration is invalid. It must contain either a driver or driver opts.");
                }

                if (this.TryParseSingleLineOption(volumeSectionLines, "external", out string? externalOption))
                {
                    if (bool.TryParse(externalOption, out bool externalOptionAsBoolean))
                    {
                        volumeModelBuilder.AddExternalOption(externalOptionAsBoolean);
                    }
                    else
                    {
                        throw new InvalidOperationException("External option must be a boolean value.");
                    }
                }

                if (this.TryParseKeyValueSection(lines, "labels", currentIndentationLevel, out Dictionary<string, string>? labelOptions))
                {
                    volumeModelBuilder.AddLabelOptions(labelOptions);
                }

                composeVolumeModels.Add(volumeModelBuilder.Build());
            }

            return composeVolumeModels;
        }

        private List<ComposeServiceModel> ParseServices(List<string> lines, int currentIndentationLevel)
        {
            List<ComposeServiceModel> serviceModels = new();

            List<int> serviceDeclarationLineNumbers = lines
                .Where(line => line.HasIndentationOf(currentIndentationLevel))
                .Select(line => lines.IndexOf(line))
                .ToList();

            foreach (int lineNumber in serviceDeclarationLineNumbers)
            {
                ComposeServiceModelBuilder serviceModelBuilder = this.composeServiceModelFactory.CreateBuilder();

                List<string> serviceSectionLines = lines
                    .Skip(lineNumber + 1)
                    .TakeWhile(l => l.HasIndentationOfAtLeast(currentIndentationLevel + 1))
                    .ToList();

                serviceModelBuilder.AddNameOption(lines[lineNumber]
                                                        .Remove(lines[lineNumber].IndexOf(":"))
                                                        .RemoveDoubleAndSingleQuotes());

                serviceModelBuilder = this.ParseImageOrBuildOptions(serviceModelBuilder, serviceSectionLines, currentIndentationLevel);

                serviceModelBuilder = this.ParsePortOptions(serviceModelBuilder, serviceSectionLines, currentIndentationLevel);

                serviceModelBuilder = this.ParseVolumeOptions(serviceModelBuilder, serviceSectionLines, currentIndentationLevel);

                if (this.TryParseSingleLineOption(serviceSectionLines, "restart", out string? restartOption))
                {
                    serviceModelBuilder.AddRestartOption(restartOption);
                }

                if (this.TryParseSingleLineOption(serviceSectionLines, "hostname", out string? hostnameOption))
                {
                    serviceModelBuilder.AddHostNameOption(hostnameOption);
                }

                if (this.TryParseKeyValueSection(serviceSectionLines, "environment", currentIndentationLevel, out Dictionary<string, string>? environmentOptions))
                {
                    serviceModelBuilder.AddEnvironmentOptions(environmentOptions);
                }

                if (this.TryParseKeyValueSection(serviceSectionLines, "labels", currentIndentationLevel, out Dictionary<string, string>? labelOptions))
                {
                    serviceModelBuilder.AddLabelsOptions(labelOptions);
                }

                serviceModels.Add(serviceModelBuilder.Build());
            }

            return serviceModels;
        }

        private bool TryParseSingleLineOption(List<string> sectionLines, string optionName, [NotNullWhen(true)] out string? optionValue)
        {
            bool success = false;
            optionName = $"{optionName}:";
            optionValue = null;

            if (sectionLines.Any(l => l.Contains(optionName)))
            {
                int indexOfOptionName = sectionLines.IndexOf(sectionLines.First(l => l.Contains(optionName)));

                optionValue = sectionLines[indexOfOptionName]
                    .Split(":")[1]
                    .RemoveDoubleAndSingleQuotes();

                success = true;
            }

            return success;
        }

        private bool TryParseKeyValueSection(List<string> sectionLines, string optionName, int currentIndentationLevel, [NotNullWhen(true)] out Dictionary<string, string>? keyValuePairs)
        {
            bool success = false;
            optionName = $"{optionName}:";
            keyValuePairs = null;

            if (sectionLines.Any(l => l.Contains(optionName)))
            {
                int indexOfOptionName = sectionLines.IndexOf(sectionLines.First(l => l.Contains(optionName)));

                string[] innerSectionLines = sectionLines
                    .Skip(indexOfOptionName + 1)
                    .TakeWhile(l => l.HasIndentationOf(currentIndentationLevel + 2))
                    .ToArray();

                keyValuePairs = innerSectionLines
                    .Select(sectionLines => sectionLines.Split(":"))
                    .ToDictionary(x => x[0].RemoveDoubleAndSingleQuotes(), x => x[1].RemoveDoubleAndSingleQuotes());

                success = true;
            }

            return success;

        }

        private ComposeServiceModelBuilder ParseVolumeOptions(ComposeServiceModelBuilder serviceModelBuilder, List<string> serviceSectionLines, int currentIndentationLevel)
        {
            if (serviceSectionLines.Any(l => l.Contains("volumes:")))
            {
                int indexOfVolumeOptions = serviceSectionLines.IndexOf(serviceSectionLines.First(l => l.Contains("volumes:")));

                string[] volumeSectionLines = serviceSectionLines
                    .Skip(indexOfVolumeOptions + 1)
                    .TakeWhile(l => l.HasIndentationOf(currentIndentationLevel + 2))
                    .ToArray();

                string[] volumeNames = volumeSectionLines
                    .Select(x => x[(x.IndexOf("-") + 1)..].RemoveDoubleAndSingleQuotes())
                    .ToArray();

                serviceModelBuilder.AddVolumeOptions(volumeNames);
            }

            return serviceModelBuilder;
        }


        private ComposeServiceModelBuilder ParsePortOptions(ComposeServiceModelBuilder serviceModelBuilder, List<string> serviceSectionLines, int currentIndentationLevel)
        {
            if (serviceSectionLines.Any(l => l.Contains("ports:")))
            {
                int indexOfPortOptions = serviceSectionLines.IndexOf(serviceSectionLines.First(l => l.Contains("ports:")));

                string[] portSectionLines = serviceSectionLines
                    .Skip(indexOfPortOptions + 1)
                    .TakeWhile(l => l.HasIndentationOf(currentIndentationLevel + 2))
                    .ToArray();

                Dictionary<string, string> ports = portSectionLines
                    .Select(x => x.Split("-")[1].Split(":"))
                    .ToDictionary(x => x[0].RemoveDoubleAndSingleQuotes(), x => x[1].RemoveDoubleAndSingleQuotes());

                serviceModelBuilder.AddPortBindingOptions(ports);
            }

            return serviceModelBuilder;
        }

        private ComposeServiceModelBuilder ParseImageOrBuildOptions(ComposeServiceModelBuilder serviceModelBuilder, List<string> serviceSectionLines, int currentIndentationLevel)
        {
            if (this.TryParseSingleLineOption(serviceSectionLines, "image", out string? imageOption))
            {
                serviceModelBuilder.AddImageOption(imageOption.RemoveDoubleAndSingleQuotes());
            }
            else if (serviceSectionLines.Any(l => l.Contains("build:")))
            {
                int indexOfBuildSection = serviceSectionLines.IndexOf(serviceSectionLines.First(l => l.Contains("build:")));

                List<string> buildSectionLines = serviceSectionLines
                    .Skip(indexOfBuildSection + 1)
                    .TakeWhile(l => l.HasIndentationOf(currentIndentationLevel + 2))
                    .ToList();

                string context = string.Empty;
                string dockerFile = string.Empty;

                if (buildSectionLines.Any(l => l.Contains("context:")))
                {
                    int indexOfContextOption = buildSectionLines.IndexOf(buildSectionLines.First(l => l.Contains("context:")));

                    context = buildSectionLines[indexOfContextOption]
                       .Split(":")[1]
                       .RemoveDoubleAndSingleQuotes();
                }

                if (buildSectionLines.Any(l => l.Contains("dockerfile:")))
                {
                    int indexOfDockerFileOption = buildSectionLines.IndexOf(buildSectionLines.First(l => l.Contains("dockerfile:")));

                    dockerFile = buildSectionLines[indexOfDockerFileOption]
                        .Split(":")[1]
                        .RemoveDoubleAndSingleQuotes();
                }

                serviceModelBuilder.AddBuildOption(context, dockerFile);
            }

            return serviceModelBuilder;
        }
    }
}
