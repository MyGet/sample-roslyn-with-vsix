using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;

namespace MyGet.DateTimeNowAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MyGetDateTimeNowAnalyzerCodeFixProvider)), Shared]
    public class MyGetDateTimeNowAnalyzerCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(MyGetDateTimeNowAnalyzerAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find "Now"
            var identifierNode = root.FindNode(diagnosticSpan);
            
            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create("Replace with DateTime.UtcNow", c => ReplaceWithDateTimeUtcNow(context.Document, identifierNode, c)),
                diagnostic);
        }

        private async Task<Document> ReplaceWithDateTimeUtcNow(Document document, SyntaxNode identifierNode, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(identifierNode, SyntaxFactory.IdentifierName("UtcNow"));
            return document.WithSyntaxRoot(newRoot);
        }
    }
}