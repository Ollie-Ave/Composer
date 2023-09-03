namespace ProjectComposeManager.Services.Builders
{
    using ProjectComposeManager.Services.Interfaces;
    using ProjectComposeManager.Services.Models;
    using System;
    using System.Collections.Generic;

    public class ComposeVolumeModelBuilder
    {
        private ComposeVolumeModel internalVolumeModel;

        public ComposeVolumeModelBuilder()
        {
            this.internalVolumeModel = new();
        }

        public ComposeVolumeModelBuilder AddNameOption(string name)
        {
            this.internalVolumeModel = this.internalVolumeModel with 
            { 
                Name = name 
            };

            return this;
        }

        public ComposeVolumeModelBuilder AddCustomNameOption(string customName)
        {
            this.internalVolumeModel = this.internalVolumeModel with
            {
                CustomName = customName
            };

            return this;
        }

        public ComposeVolumeModelBuilder AddDriverOption(string driver)
        {
            this.internalVolumeModel = this.internalVolumeModel with
            {
                Driver = driver
            };

            return this;
        }

        public ComposeVolumeModelBuilder AddDriverOptions(Dictionary<string, string> driverOptions)
        {
            this.internalVolumeModel = this.internalVolumeModel with
            {
                DriverOptions = driverOptions
            };

            return this;
        }

        public ComposeVolumeModelBuilder AddExternalOption(bool external)
        {
            this.internalVolumeModel = this.internalVolumeModel with
            {
                External = external
            };

            return this;
        }

        public ComposeVolumeModelBuilder AddLabelOptions(Dictionary<string, string> labels)
        {
            this.internalVolumeModel = this.internalVolumeModel with
            {
                Labels = labels
            };

            return this;
        }

        public ComposeVolumeModel Build()
        {
            this.ValidateModel(this.internalVolumeModel);

            return this.internalVolumeModel;
        }

        public void ValidateModel(ComposeVolumeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new InvalidOperationException("Volume name is required.");
            }

            if (!string.IsNullOrWhiteSpace(model.Driver) && model.DriverOptions is not null)
            {
                throw new InvalidOperationException("Unable to use both Driver and Driver options.");
            }
            if (string.IsNullOrWhiteSpace(model.Driver) && model.DriverOptions is null)
            {
                throw new InvalidOperationException("Either Driver or Driver Options must be set.");
            }
        }
    }
}