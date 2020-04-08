using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DigitalTwin.ProvisioningApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var appSettings = AppSettings.Load();

                var actionName = ParseArgs(args);
                actionName = ActionName.ProvisionTwin;
                if (actionName == null)
                    return;

                switch (actionName)
                {
                    case ActionName.CreateEndpoints:
                        await Actions.CreateEndpoints(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.CreateRoleAssignments:
                        await Actions.CreateRoleAssignments(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.GetAvailableAndFreshSpaces:
                        await Actions.GetAvailableAndFreshSpaces(await SetupHttpClient(Loggers.SilentLogger, appSettings));
                        break;
                    case ActionName.GetOntologies:
                        await Api.GetOntologies(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.GetSpaces:
                        await Actions.GetSpaces(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;
                    case ActionName.ProvisionTwin:
                        await Actions.ProvisionTwin(await SetupHttpClient(Loggers.ConsoleLogger, appSettings), Loggers.ConsoleLogger);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e}");
            }
        }

        private static ActionName? ParseArgs(string[] args)
        {
            if (args.Length >= 1 && Enum.TryParse(args[0], out ActionName actionName))
            {
                return actionName;
            }
            else
            {
                // Generate the list of available action names from the enum
                // and output them in the usage string
                var actionNames = Enum.GetNames(typeof(ActionName))
                    .Aggregate((string acc, string s) => acc + " | " + s);
                Console.WriteLine($"Usage: dotnet run [{actionNames}]");

                return null;
            }
        }

        private static async Task<HttpClient> SetupHttpClient(ILogger logger, AppSettings appSettings)
        {
            var httpClient = new HttpClient(new LoggingHttpHandler(logger))
            {
                BaseAddress = new Uri(appSettings.BaseUrl),
            };

            var accessToken = await Authentication.GetToken(appSettings);
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            return httpClient;
        }
    }
}
