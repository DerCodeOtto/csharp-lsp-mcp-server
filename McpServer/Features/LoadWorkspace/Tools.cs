using McpServer.Core;
using Microsoft.CodeAnalysis.MSBuild;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace McpServer.Features; 
public static partial class Tools {
    [McpServerTool]
    [Description("Loads a .sln or .csproj file into a workspace to be able to use other tools with it.")]
    public static async Task<ToolResponse<string>> LoadWorkspace(string filePath) {
        try {
            if (!File.Exists(filePath)) {
                throw new ToolException($"File {filePath} does not exist.");
            }
            var workspace = MSBuildWorkspace.Create();
            if (filePath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
                await workspace.OpenSolutionAsync(filePath);
            else if (filePath.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
                await workspace.OpenProjectAsync(filePath);
            else
                throw new ToolException($"The provided file must be a .sln or .csproj file but {filePath} is not");
            WorkspaceHolder.Workspace = workspace;
            return "Workspace successfully loaded";
        } catch (ToolException e) {
            return new ErrorMessage(e.Message);
        }
    }
}