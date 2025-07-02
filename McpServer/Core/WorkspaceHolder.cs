using Microsoft.CodeAnalysis;

namespace McpServer.Core;
public static class WorkspaceHolder {
    private static Workspace? _workspace;

    public static Workspace Workspace {
        get {
            if (_workspace == null) {
                throw new ToolException("No Workspace loaded");
            }
            return _workspace;
        }
        set {
            _workspace = value;
        }
    }

    public static Document? GetDocument(this Workspace workspace, string filePath) {
        return workspace.CurrentSolution.Projects
            .SelectMany(p => p.Documents)
            .FirstOrDefault(d => d.FilePath?.Replace('\\', '/') == filePath.Replace('\\', '/'));
    }
    public static IEnumerable<Document> GetDocuments(this Workspace workspace, string filePath) =>
        workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath)
        .Select(id => workspace.CurrentSolution.GetDocument(id))
        .OfType<Document>();
}