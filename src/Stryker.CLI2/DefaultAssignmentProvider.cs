using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Stryker.CLI2;

public class DefaultAssignmentProvider : CSharpSyntaxRewriter
{
    public bool Updated { get; set; }
    public override SyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
    {
        var variablesNeedsInitialization = node
            .Declaration
            .Variables
            .Where(x => x.Initializer == null)
            .ToList();

        if (variablesNeedsInitialization.Count == 0)
        {
            return base.VisitLocalDeclarationStatement(node);
        }

        foreach (var variable in variablesNeedsInitialization)
        {
            var replacement = variable.WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.DefaultExpression(node.Declaration.Type.WithoutTrivia())));
            node = node.ReplaceNode(variable, replacement);
            Updated = true;
        }

        return node;
    }
}
