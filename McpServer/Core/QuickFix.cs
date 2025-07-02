using Newtonsoft.Json;

namespace McpServer.Core;

public class QuickFix {
    public required string FileName { get; init; }
    [JsonConverter(typeof(ZeroBasedIndexConverter))]
    public int Line { get; set; }
    [JsonConverter(typeof(ZeroBasedIndexConverter))]
    public int Column { get; set; }
    [JsonConverter(typeof(ZeroBasedIndexConverter))]
    public int EndLine { get; set; }
    [JsonConverter(typeof(ZeroBasedIndexConverter))]
    public int EndColumn { get; set; }
    public string? Text { get; set; }
    public ICollection<string> Projects { get; set; } = [];

    public override bool Equals(object? obj) {
        if (obj is not QuickFix quickFix) {
            return false;
        }

        return FileName == quickFix.FileName
            && Line == quickFix.Line
            && Column == quickFix.Column
            && EndLine == quickFix.EndLine
            && EndColumn == quickFix.EndColumn
            && Text == quickFix.Text;
    }

    public override int GetHashCode() {
        unchecked {
            var hash = 17;
            hash = hash * 23 + (FileName?.GetHashCode()).GetValueOrDefault();
            hash = hash * 23 + Line.GetHashCode();
            hash = hash * 23 + Column.GetHashCode();
            hash = hash * 23 + EndLine.GetHashCode();
            hash = hash * 23 + EndColumn.GetHashCode();
            hash = hash * 23 + (Text?.GetHashCode()).GetValueOrDefault();
            return hash;
        }
    }

    public override string ToString()
        => $"{Text} ({Line}:{Column}) - ({EndLine}:{EndColumn})";

    public bool Contains(int line, int column) {
        if (Line > line || EndLine < line) {
            return false;
        }

        if (Line == line && Column > column) {
            return false;
        }

        if (EndLine == line && EndColumn < column) {
            return false;
        }

        return true;
    }
}