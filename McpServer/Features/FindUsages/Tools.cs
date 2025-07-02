using McpServer.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpServer.Features;

using SymbolLocations = ToolResponse<IEnumerable<SymbolLocation>>;

public static partial class Tools {
    [McpServerTool]
    [Description("Find all usages of a symbol at a specific position in a source file. The position (line/column) is zero based")]
    public static async Task<SymbolLocations> FindUsages(
        string filePath,
        int line,
        int column,
        CancellationToken cancellationToken = default) {
        try {
            var workspace = WorkspaceHolder.Workspace;

            var document = workspace.GetDocument(filePath)
                ?? throw new ToolException($"Could not find document for path {filePath}.");

            var semanticModel = await document.GetSemanticModelAsync(cancellationToken)
                ?? throw new ToolException($"Unable to get semantic model for {filePath}.");

            var sourceText = await document.GetTextAsync(cancellationToken);
            var position = sourceText.Lines.GetPosition(new LinePosition(line, column));

            var symbol = await SymbolFinder.FindSymbolAtPositionAsync(semanticModel, position, workspace, cancellationToken)
                ?? throw new ToolException($"No symbol found at position {line}:{column} in file {filePath}.");

            var definitions = await SymbolFinder.FindSourceDefinitionAsync(symbol, workspace.CurrentSolution, cancellationToken) ?? symbol;

            var references = await SymbolFinder.FindReferencesAsync(definitions ?? symbol, workspace.CurrentSolution, cancellationToken);

            var referenceLocations = references
                .SelectMany(r => r.Locations)
                .Select(loc => loc.Location)
                .ToList();

            return referenceLocations
                .Distinct()
                .Select(l => l.GetSymbolLocation(workspace))
                .ToToolResponse();
        } catch (ToolException e) {
            return new ErrorMessage(e.Message);
        }
    }
}
