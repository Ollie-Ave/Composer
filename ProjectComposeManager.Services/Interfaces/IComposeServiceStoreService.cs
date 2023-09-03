using ProjectComposeManager.Services.Models;

namespace ProjectComposeManager.Services.Interfaces
{
    public interface IComposeServiceStoreService
    {
        public ServiceModuleModel[] GetAllServiceMetaDataObjects();

        public ServiceModuleModel GetServiceMetaData(string name);

        public ComposeServiceModel GetComposeServiceByName(string name, bool useLocal);

        public ComposeServiceModel GetComposeService(ServiceModuleModel serviceModuleModel, bool useLocal);

        public void Save(ServiceModuleModel modelToSave);
    }
}