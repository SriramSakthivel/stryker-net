using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.CLI;
using Stryker.CLI.Logging;
using Stryker.Core.Initialisation;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;

namespace Stryker.CLI2;


public class PreProcessInitialisationProcess : IInitialisationProcess
{
    private readonly Lazy<IInitialisationProcess> _lazyInitialisationProcess;

    public PreProcessInitialisationProcess(Lazy<IInitialisationProcess> lazyInitialisationProcess)
    {
        _lazyInitialisationProcess = lazyInitialisationProcess ?? throw new ArgumentNullException(nameof(lazyInitialisationProcess));
    }

    public MutationTestInput Initialize(StrykerOptions options)
    {
        var original = _lazyInitialisationProcess.Value;

        var input = original.Initialize(options);

        var logger = ApplicationLogging.LoggerFactory.CreateLogger<PreProcessInitialisationProcess>();


        foreach (var inputFile in input.ProjectInfo.ProjectContents.GetAllFiles().OfType<CsharpFileLeaf>())
        {
            var rawSyntaxTree = inputFile.SyntaxTree;

            var provider = new DefaultAssignmentProvider();
            var updatedTree = provider.Visit(rawSyntaxTree.GetRoot()).SyntaxTree;

            if (provider.Updated)
            {
                inputFile.SyntaxTree = updatedTree;

                logger.LogDebug("[PreProcessInitialisationProcess]-----------------------------------Raw Syntax Tree----------------------------------------");
                logger.LogDebug(rawSyntaxTree.GetRoot().ToFullString());
                logger.LogDebug("[PreProcessInitialisationProcess]-------------------------------------------------------------------------------------------");

                logger.LogDebug("[PreProcessInitialisationProcess]--------------------------------Updated Syntax Tree----------------------------------------");
                logger.LogDebug(updatedTree.GetRoot().ToFullString());
                logger.LogDebug("[PreProcessInitialisationProcess]-------------------------------------------------------------------------------------------");
            }
            else
            {
                logger.LogDebug($"[PreProcessInitialisationProcess] : Nothing to modify for variable declaration, ignoring the file {inputFile.FullPath}");
            }
        }

        return input;
    }

    public InitialTestRun InitialTest(StrykerOptions options)
    {
        return _lazyInitialisationProcess.Value.InitialTest(options);
    }
}
