namespace McpServer.Core;
public sealed class ToolResponse<T> {
    public required bool IsSuccess { get; init; }
    public T? Value { get; init; }
    public ErrorMessage ErrorMessage { get; init; } = ErrorMessage.Empty;

    public static implicit operator ToolResponse<T>(T value) {
        return new ToolResponse<T> {
            IsSuccess = true,
            Value = value
        };
    }
    public static implicit operator ToolResponse<T>(ErrorMessage errorMessage) {
        return new ToolResponse<T> {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}

public static class ToolResponseExtensions {
    public static ToolResponse<T> ToToolResponse<T>(this T value) {
        return new ToolResponse<T> {
            IsSuccess = true,
            Value = value
        };
    }
    public static ToolResponse<T> ToToolResponse<T>(this ErrorMessage errorMessage) {
        return new ToolResponse<T> {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}
