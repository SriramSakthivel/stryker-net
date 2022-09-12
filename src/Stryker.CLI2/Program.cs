using Spectre.Console;
using Stryker.Core;
using Stryker.Core.Exceptions;
using Stryker.Core.Initialisation;

namespace Stryker.CLI2
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var app = new StrykerCli2(null, new Lazy<IProjectOrchestrator>(CreateIProjectOrchestrator));
                return app.Run(args);
            }
            catch (InputException exception)
            {
                AnsiConsole.MarkupLine("[Yellow]Stryker.NET failed to mutate your project. For more information see the logs below:[/]");
                AnsiConsole.WriteLine(exception.ToString());
                return ExitCodes.OtherError;
            }
        }

        private static IProjectOrchestrator CreateIProjectOrchestrator()
        {
            IInitialisationProcess initProcess = new PreProcessInitialisationProcess(new Lazy<IInitialisationProcess>(() => new InitialisationProcess()));
            return new ProjectOrchestrator(null, new ProjectMutator(initProcess));
        }
    }
}
