using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace McpServer.Core; 
internal static class Helpers {
    internal static async Task<ISymbol?> GetDefinitionSymbol(Document document, int line, int column, CancellationToken cancellationToken) {
        var sourceText = await document.GetTextAsync(cancellationToken);
        var position = sourceText.GetPositionFromLineAndOffset(line, column);
        var symbol = await SymbolFinder.FindSymbolAtPositionAsync(document, position, cancellationToken);

        return symbol switch {
            INamespaceSymbol => null,
            // Always prefer the partial implementation over the definition
            IMethodSymbol { IsPartialDefinition: true, PartialImplementationPart: var impl } => impl,
            // Don't return property getters/settings/initers
            IMethodSymbol { AssociatedSymbol: IPropertySymbol } => null,
            _ => symbol
        };
    }
}
