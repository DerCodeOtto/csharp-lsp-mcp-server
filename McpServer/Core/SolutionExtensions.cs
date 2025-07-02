using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace McpServer.Core; 
internal static class SolutionExtensions {
    internal static SourceGeneratedFileInfo? GetSourceGeneratedFileInfo(this Solution solution, Location location) {
        Debug.Assert(location.IsInSource);
        var document = solution.GetDocument(location.SourceTree);
        if (document is not SourceGeneratedDocument) {
            return null;
        }

        return new SourceGeneratedFileInfo {
            ProjectGuid = document.Project.Id.Id,
            DocumentGuid = document.Id.Id
        };
    }
}
