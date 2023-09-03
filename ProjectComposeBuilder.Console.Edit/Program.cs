namespace ProjectComposeManager.Edit
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ProjectComposeManager.Console.Edit.Services;
    using Serilog;
    using Serilog.Extensions.Logging;
    using System;

    public class Program
    {
        private static void Main()
        {
            try
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                IServiceCollection serviceCollection = new ServiceCollection();

                serviceCollection.AddSingleton<AppService>();
                serviceCollection.AddComposeManagerServices(configuration);

                serviceCollection.AddLogging(loggingBuilder =>
                {
                    SerilogLoggerProvider loggingProvider = new(CreateLogger(configuration));

                    loggingBuilder.AddProvider(loggingProvider);
                });

                IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

                serviceProvider.GetRequiredService<AppService>().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static Serilog.ILogger CreateLogger(IConfigurationRoot configuration)
        {
            string seqServer = configuration.GetRequiredSection("SeqServer").Value ?? throw new Exception("Unable to find SeqServer configuration");

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq(seqServer)
                .CreateLogger();

            return Log.Logger;
        }
    }
}