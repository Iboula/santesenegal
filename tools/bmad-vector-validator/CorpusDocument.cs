namespace SanteSenegal.BmadValidator;

public sealed record CorpusDocument(
    string Path,
    string RelativePath,
    IReadOnlyDictionary<string, object> Metadata,
    string Body)
{
    public string Id => GetString("id");

    public string Status => GetString("status");

    public IReadOnlyList<string> Tags => GetStringList("tags");

    public IReadOnlyList<string> Entities => GetStringList("entities");

    public IReadOnlyList<string> Dependencies => GetStringList("dependencies");

    public string GetString(string key)
    {
        return Metadata.TryGetValue(key, out var value) && value is string text ? text : string.Empty;
    }

    public IReadOnlyList<string> GetStringList(string key)
    {
        return Metadata.TryGetValue(key, out var value) && value is IReadOnlyList<string> list
            ? list
            : Array.Empty<string>();
    }
}
