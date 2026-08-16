using System.Text.RegularExpressions;

namespace SanteSenegal.BmadValidator;

public static partial class CorpusValidator
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.Ordinal)
    {
        "draft",
        "ready-for-review",
        "accepted",
        "deprecated"
    };

    private static readonly string[] RequiredMetadata =
    [
        "id",
        "title",
        "status",
        "tags",
        "entities"
    ];

    public static ValidationResult Validate(string repositoryRoot)
    {
        var docsRoot = Path.Combine(repositoryRoot, "docs");
        var errors = new List<ValidationError>();
        var documents = new List<CorpusDocument>();

        if (!Directory.Exists(docsRoot))
        {
            errors.Add(new ValidationError("docs", "docs directory does not exist", ValidationErrorKind.Metadata));
            return new ValidationResult(0, 0, errors);
        }

        foreach (var file in Directory.EnumerateFiles(docsRoot, "*.md", SearchOption.AllDirectories))
        {
            if (!FrontMatterParser.TryParse(file, docsRoot, out var document, out var parseErrors))
            {
                continue;
            }

            errors.AddRange(parseErrors);
            if (document is not null)
            {
                documents.Add(document);
                ValidateMetadata(document, errors);
            }
        }

        ValidateUniqueIds(documents, errors);

        var ids = documents
            .Where(document => !string.IsNullOrWhiteSpace(document.Id))
            .Select(document => document.Id)
            .ToHashSet(StringComparer.Ordinal);

        ValidateDependencies(documents, ids, errors);
        ValidateCatalog(repositoryRoot, documents, errors);
        ValidateRelationships(repositoryRoot, ids, errors);

        return new ValidationResult(documents.Count, ids.Count, errors);
    }

    private static void ValidateMetadata(CorpusDocument document, List<ValidationError> errors)
    {
        foreach (var key in RequiredMetadata)
        {
            if (!document.Metadata.ContainsKey(key))
            {
                errors.Add(new ValidationError(DocumentName(document), $"required metadata '{key}' is missing", ValidationErrorKind.Metadata));
            }
        }

        if (string.IsNullOrWhiteSpace(document.Id))
        {
            errors.Add(new ValidationError(document.RelativePath, "id is empty", ValidationErrorKind.Metadata));
        }

        if (!string.IsNullOrWhiteSpace(document.Status) && !AllowedStatuses.Contains(document.Status))
        {
            errors.Add(new ValidationError(DocumentName(document), $"status '{document.Status}' is not allowed", ValidationErrorKind.Metadata));
        }

        ValidateList(document, "tags", document.Tags, errors);
        ValidateList(document, "entities", document.Entities, errors);

        foreach (var tag in document.Tags)
        {
            if (tag != tag.ToLowerInvariant())
            {
                errors.Add(new ValidationError(DocumentName(document), $"tag '{tag}' must be lowercase", ValidationErrorKind.Metadata));
            }
        }
    }

    private static void ValidateList(CorpusDocument document, string key, IReadOnlyList<string> values, List<ValidationError> errors)
    {
        if (document.Metadata.TryGetValue(key, out var rawValue) && rawValue is not IReadOnlyList<string>)
        {
            errors.Add(new ValidationError(DocumentName(document), $"metadata '{key}' must be a YAML list", ValidationErrorKind.Metadata));
            return;
        }

        if (!document.Metadata.ContainsKey(key))
        {
            return;
        }

        if (values.Count == 0 || values.Any(string.IsNullOrWhiteSpace))
        {
            errors.Add(new ValidationError(DocumentName(document), $"metadata '{key}' must contain non-empty values", ValidationErrorKind.Metadata));
        }
    }

    private static void ValidateUniqueIds(IReadOnlyList<CorpusDocument> documents, List<ValidationError> errors)
    {
        foreach (var group in documents.Where(document => !string.IsNullOrWhiteSpace(document.Id)).GroupBy(document => document.Id))
        {
            if (group.Count() <= 1)
            {
                continue;
            }

            foreach (var document in group)
            {
                errors.Add(new ValidationError(DocumentName(document), "duplicate id", ValidationErrorKind.Metadata));
            }
        }
    }

    private static void ValidateDependencies(IReadOnlyList<CorpusDocument> documents, HashSet<string> ids, List<ValidationError> errors)
    {
        foreach (var document in documents)
        {
            foreach (var dependency in document.Dependencies)
            {
                if (string.IsNullOrWhiteSpace(dependency))
                {
                    errors.Add(new ValidationError(DocumentName(document), "dependency is empty", ValidationErrorKind.Dependency));
                    continue;
                }

                if (!ids.Contains(dependency))
                {
                    errors.Add(new ValidationError(DocumentName(document), $"dependency {dependency} does not exist", ValidationErrorKind.Dependency));
                }
            }
        }
    }

    private static void ValidateCatalog(string repositoryRoot, IReadOnlyList<CorpusDocument> documents, List<ValidationError> errors)
    {
        var catalogPath = Path.Combine(repositoryRoot, "docs", "vector", "catalog.md");
        if (!File.Exists(catalogPath))
        {
            return;
        }

        var documentByPath = documents.ToDictionary(document => document.RelativePath, StringComparer.OrdinalIgnoreCase);
        foreach (var line in File.ReadLines(catalogPath))
        {
            var match = CatalogRowRegex().Match(line);
            if (!match.Success)
            {
                continue;
            }

            var id = match.Groups["id"].Value.Trim();
            var path = match.Groups["path"].Value.Trim().Replace('\\', '/');
            var fullPath = Path.Combine(repositoryRoot, path.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(fullPath))
            {
                errors.Add(new ValidationError("BMAD-VEC-CATALOG-001", $"catalog path {path} does not exist", ValidationErrorKind.Catalog));
                continue;
            }

            if (!documentByPath.TryGetValue(path["docs/".Length..], out var document))
            {
                errors.Add(new ValidationError("BMAD-VEC-CATALOG-001", $"catalog path {path} is not vectorizable", ValidationErrorKind.Catalog));
                continue;
            }

            if (!string.Equals(document.Id, id, StringComparison.Ordinal))
            {
                errors.Add(new ValidationError("BMAD-VEC-CATALOG-001", $"catalog entry {id} points to document id {document.Id}", ValidationErrorKind.Catalog));
            }
        }
    }

    private static void ValidateRelationships(string repositoryRoot, HashSet<string> ids, List<ValidationError> errors)
    {
        var relationshipsPath = Path.Combine(repositoryRoot, "docs", "vector", "relationships.md");
        if (!File.Exists(relationshipsPath))
        {
            return;
        }

        var text = File.ReadAllText(relationshipsPath);
        foreach (Match match in ExplicitIdRegex().Matches(text))
        {
            var id = match.Value;
            if (!ids.Contains(id))
            {
                errors.Add(new ValidationError("BMAD-VEC-RELATIONSHIPS-001", $"relationship id {id} does not exist", ValidationErrorKind.Dependency));
            }
        }
    }

    private static string DocumentName(CorpusDocument document)
    {
        return string.IsNullOrWhiteSpace(document.Id) ? document.RelativePath : document.Id;
    }

    [GeneratedRegex(@"^\|\s*(?<id>[A-Z0-9]+(?:-[A-Z0-9]+)+)\s*\|.*?`\s*(?<path>docs/[^`]+\.md)\s*`", RegexOptions.Compiled)]
    private static partial Regex CatalogRowRegex();

    [GeneratedRegex(@"\b[A-Z][A-Z0-9]+(?:-[A-Z0-9]+)+\b", RegexOptions.Compiled)]
    private static partial Regex ExplicitIdRegex();
}
