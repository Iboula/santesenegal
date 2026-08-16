namespace SanteSenegal.BmadValidator;

public static class FrontMatterParser
{
    public static bool TryParse(string filePath, string docsRoot, out CorpusDocument? document, out IReadOnlyList<ValidationError> errors)
    {
        document = null;
        var localErrors = new List<ValidationError>();
        var text = File.ReadAllText(filePath);
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var relativePath = Path.GetRelativePath(docsRoot, filePath).Replace('\\', '/');

        if (lines.Length == 0 || lines[0] != "---")
        {
            errors = localErrors;
            return false;
        }

        var closingIndex = Array.FindIndex(lines, 1, line => line == "---");
        if (closingIndex < 0)
        {
            localErrors.Add(new ValidationError(relativePath, "front matter closing delimiter is missing", ValidationErrorKind.Metadata));
            errors = localErrors;
            return true;
        }

        var metadata = ParseYamlSubset(lines[1..closingIndex], relativePath, localErrors);
        var body = string.Join('\n', lines[(closingIndex + 1)..]);
        document = new CorpusDocument(filePath, relativePath, metadata, body);
        errors = localErrors;
        return true;
    }

    private static Dictionary<string, object> ParseYamlSubset(string[] lines, string relativePath, List<ValidationError> errors)
    {
        var metadata = new Dictionary<string, object>(StringComparer.Ordinal);
        string? currentListKey = null;

        for (var index = 0; index < lines.Length; index++)
        {
            var lineNumber = index + 2;
            var line = lines[index];

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith('\t'))
            {
                errors.Add(new ValidationError(relativePath, $"YAML line {lineNumber} uses tab indentation", ValidationErrorKind.Metadata));
                continue;
            }

            if (line.StartsWith("  - ", StringComparison.Ordinal))
            {
                if (currentListKey is null || metadata[currentListKey] is not List<string> values)
                {
                    errors.Add(new ValidationError(relativePath, $"YAML line {lineNumber} has a list item without a list key", ValidationErrorKind.Metadata));
                    continue;
                }

                values.Add(line[4..].Trim());
                continue;
            }

            currentListKey = null;
            var colonIndex = line.IndexOf(':', StringComparison.Ordinal);
            if (colonIndex <= 0)
            {
                errors.Add(new ValidationError(relativePath, $"YAML line {lineNumber} is not a key-value entry", ValidationErrorKind.Metadata));
                continue;
            }

            var key = line[..colonIndex].Trim();
            var value = line[(colonIndex + 1)..].Trim();

            if (string.IsNullOrWhiteSpace(key) || key.Contains(' '))
            {
                errors.Add(new ValidationError(relativePath, $"YAML line {lineNumber} has an invalid key", ValidationErrorKind.Metadata));
                continue;
            }

            if (metadata.ContainsKey(key))
            {
                errors.Add(new ValidationError(relativePath, $"YAML key '{key}' is duplicated", ValidationErrorKind.Metadata));
                continue;
            }

            if (string.IsNullOrEmpty(value))
            {
                var list = new List<string>();
                metadata[key] = list;
                currentListKey = key;
                continue;
            }

            if (value.StartsWith('[') || value.StartsWith('{'))
            {
                errors.Add(new ValidationError(relativePath, $"YAML line {lineNumber} uses unsupported inline collection syntax", ValidationErrorKind.Metadata));
                continue;
            }

            metadata[key] = value.Trim('"');
        }

        return metadata;
    }
}
