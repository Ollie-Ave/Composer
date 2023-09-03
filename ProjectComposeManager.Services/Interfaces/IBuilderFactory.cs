namespace ProjectComposeManager.Services.Interfaces
{
    public interface IBuilderFactory<T>
        where T : class
    {
        public T CreateBuilder();
    }
}
