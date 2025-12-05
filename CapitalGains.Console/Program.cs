using CapitalGains.Application.Interfaces;
using CapitalGains.Application.Services;
using CapitalGains.Common;
using CapitalGains.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

public class Program
{
    public static IConfiguration Configuration { get; private set; }
    public static IHost AppHost { get; private set; }

    public static IHost CreateHost(string[] args) =>
                    Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration((hostContext, appConfig) =>
                        {
                            appConfig
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables();
                        })
                        .ConfigureServices((hostContext, services) =>
                        {
                            //Add the app services
                            services.AddSingleton<IOperationsService, OperationsService>();
                        })
                        .UseSerilog((context, services, configuration) =>
                        {
                            //Configure the log service (Serilog)
                            var logConfig = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                    .Build();

                            configuration.ReadFrom.Configuration(logConfig);
                        })
                        .Build();       

    public static void Main(string[] args)
    {
        AppHost = CreateHost(args);
        if (args.Length > 0)
        {
            // Initialize the configuration, optionally with an environment name
            Configuration = AppHost.Services.GetRequiredService<IConfiguration>();
            IOperationsService operationsServices = AppHost.Services.GetService<IOperationsService>();

            try
            {
                List<string> jsonArray = GetData(args[0], operationsServices);
                foreach (string json in jsonArray)
                {
                    Console.WriteLine(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        else
        {
            ILogger<Program> logger = AppHost.Services.GetRequiredService<ILogger<Program>>();
            string errMessage = "The filename with the operations is missing.";
            logger.Log(LogLevel.Error, errMessage);
            Console.WriteLine(errMessage);
        }
        Console.WriteLine();
        Console.WriteLine("Press ENTER to finish...");
        Console.Read();
    }

    internal static List<string> GetData(string fileName, IOperationsService operationsServices)
    {
        //Get settings
        decimal tax = 20;
        decimal minimumTaxed = 20000;
        ILogger<Program> logger = AppHost.Services.GetRequiredService<ILogger<Program>>();

        if (!File.Exists(fileName))
        {
            string errMessage = "The specified file does not exist.";
            logger.Log(LogLevel.Error, errMessage);
            throw new FileNotFoundException(errMessage);
        }

        try
        {
            IConfigurationSection section = Configuration.GetSection("Settings");
            tax = decimal.Parse(section["Tax"]);
            minimumTaxed = decimal.Parse(section["MinimumTaxed"]);
        }
        catch { }

        List<string> retValue = new List<string>();
        string textOperations = Tools.ReadFile(fileName);
        if (textOperations == string.Empty)
        {
            string errMessage = $"The file {fileName} is not in a correct format.";
            logger.Log(LogLevel.Error, errMessage);
            throw new InvalidOperationException("The specified file is empty.");
        }
        string[] jsonArray = textOperations.Split("]");

        for (int i = 0; i < jsonArray.Length - 1; i++)
        {
            //Build json for the service
            string jsonData = jsonArray[i] + "]";
            List<OperationDTO> dataOperations = new List<OperationDTO>();
            try
            {
                dataOperations = Converters.ConvertJsonArrayToDTO(jsonData);
            }
            catch (Exception ex)
            {
                string message = $"The file {fileName} is not in a correct format.";
                logger.Log(LogLevel.Error, ex, message);
                throw new InvalidCastException(message);
            }

            var processed = operationsServices.ProcessMessage(dataOperations, tax, minimumTaxed);
            string jsonTax = Converters.ConvertDTOtoJsonArray(processed);
            retValue.Add(jsonTax);
        }
        return retValue;
    }
}