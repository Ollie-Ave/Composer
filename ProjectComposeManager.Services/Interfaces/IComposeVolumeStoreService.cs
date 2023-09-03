namespace ProjectComposeManager.Services.Interfaces
{
    using ProjectComposeManager.Services.Models;

    public interface IComposeVolumeStoreService
    {
        public VolumeModuleModel[] GetAllVolumeMetaDataObjects();

        public VolumeModuleModel GetVolumeMetaData(string name);

        public ComposeVolumeModel GetComposeVolume(VolumeModuleModel serviceModuleModel);

        public ComposeVolumeModel GetComposeVolumeByName(string name);

        public void Save(VolumeModuleModel modelToSave);
    }
}