namespace ProjectComposeManager.Services.Factories
{
    using ProjectComposeManager.Services.Interfaces;

    public class BuilderFactory<T> : IBuilderFactory<T>
        where T : class, new()
    {
        public T CreateBuilder()
        {
            return new T();
        }
    }
}
