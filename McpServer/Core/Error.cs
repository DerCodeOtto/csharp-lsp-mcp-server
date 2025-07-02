namespace McpServer.Core; 
public sealed record ErrorMessage(string Value) {
    private static readonly ErrorMessage _empty = new(string.Empty);
    public static ErrorMessage Empty => _empty;

    public static implicit operator ErrorMessage(string value) {
        return new ErrorMessage(value);
    }
}
