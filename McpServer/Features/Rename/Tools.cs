using McpServer.Core;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis;
using ModelContextProtocol.Server;
using System.ComponentModel;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Rename;

namespace McpServer.Features;
public static partial class Tools {
    [McpServerTool]
    [Description("Renames all occurences of the symbol at a specific position in a source file to the provided new name. The position (line/column) is zero based")]
    public static async Task<ToolResponse<RenameResult>> Rename (
        string filePath,
        int line,
        int column,
        string newName,
        CancellationToken cancellationToken = default) {
        try {
            var workspace = WorkspaceHolder.Workspace;
            var document = workspace.GetDocument(filePath)
                ?? throw new ToolException($"Could not find document for path {filePath}.");
            var sourceText = await document.GetTextAsync(cancellationToken);
            var position = sourceText.Lines.GetPosition(new LinePosition(line, column));
            var symbol = await SymbolFinder.FindSymbolAtPositionAsync(document, position, cancellationToken)
                ?? throw new ToolException($"No symbol found at position {line}:{column} in file {filePath}.");
            Solution solution = workspace.CurrentSolution;

            var symbolRenameOptions = new SymbolRenameOptions(
                RenameInComments: true,
                RenameInStrings: true,
                RenameOverloads: true);

            var changedSolution = await Renamer.RenameSymbolAsync(
                solution,
                symbol,
                symbolRenameOptions,
                newName,
                cancellationToken);
            var solutionChanges = changedSolution.GetChanges(workspace.CurrentSolution);
            var changedFiles = solutionChanges.GetProjectChanges()
                .SelectMany(x => x.GetChangedDocuments())
                .Select(changedSolution.GetDocument)
                .Select(doc => doc?.FilePath ?? string.Empty) ?? [];
            return workspace.TryApplyChanges(changedSolution)
                ? new RenameResult(changedFiles.ToList()).ToToolResponse()
                : new ErrorMessage("Failed to apply changes to the workspace.");

        } catch (ToolException e) {
            return new ErrorMessage(e.Message);
        }
    }
}

public sealed record RenameResult(IEnumerable<string> ChangedFiles);