using McpServer.Core;
using Microsoft.CodeAnalysis;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpServer.Features;

public static partial class Tools {
    [McpServerTool]
    [Description("Finds the definition of the symbol at a specific position in a source file. The position (line/column) is zero based")]
    public static async Task<ToolResponse<SymbolLocation>> FindDefinition(
        string filePath,
        int line,
        int column,
        CancellationToken cancellationToken = default) {
        try {
            var workspace = WorkspaceHolder.Workspace;
            var document = workspace.GetDocument(filePath)
                ?? throw new ToolException($"Could not find document for path {filePath}.");
            var symbol = await Helpers.GetDefinitionSymbol(document, line, column, cancellationToken)
                ?? throw new ToolException($"No symbol found at position {line}:{column} in file {filePath}.");
            var location = symbol.Locations.First();

            return location switch {
                { IsInSource: true } => GetSymbolLocation(symbol, location, document),
                { IsInMetadata: true } => new ErrorMessage("Symbol is defined in metadata, not in source code."),
                _ => new ErrorMessage("Could not find symbol location in source code.")
            };
        } catch (ToolException e) {
            return new ErrorMessage(e.Message);
        }

        static SymbolLocation GetSymbolLocation(ISymbol symbol, Location location, Document document) {
            var lineSpan = symbol.Locations.First().GetMappedLineSpan();
            return new SymbolLocation {
                FileName = lineSpan.Path,
                Line = lineSpan.StartLinePosition.Line,
                Column = lineSpan.StartLinePosition.Character,
                GeneratedFileInfo = SolutionExtensions.GetSourceGeneratedFileInfo(document.Project.Solution, location)
            };
        }
    }
}
