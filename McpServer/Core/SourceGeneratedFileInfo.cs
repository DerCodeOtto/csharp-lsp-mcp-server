namespace McpServer.Core;

public record SourceGeneratedFileInfo {
    public Guid ProjectGuid { get; init; }
    public Guid DocumentGuid { get; init; }
}
