namespace SanteSenegal.BmadValidator.Tests;

public sealed class CorpusValidatorTests
{
    [Fact]
    public void ValidDocumentPasses()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument("docs/stories/US-001.md", ValidDocument("US-001"));

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.True(result.IsValid);
        Assert.Equal(1, result.DocumentCount);
        Assert.Equal(1, result.UniqueIdCount);
    }

    [Fact]
    public void MissingIdFails()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument(
            "docs/stories/no-id.md",
            """
            ---
            title: Missing ID
            status: draft
            tags:
              - docs
            entities:
              - Story
            ---

            # Missing ID
            """);

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Message.Contains("required metadata 'id' is missing", StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateIdFails()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument("docs/stories/a.md", ValidDocument("US-001"));
        workspace.WriteDocument("docs/stories/b.md", ValidDocument("US-001"));

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Message == "duplicate id");
    }

    [Fact]
    public void UppercaseTagFails()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument(
            "docs/stories/US-001.md",
            ValidDocument("US-001").Replace("  - docs", "  - Docs", StringComparison.Ordinal));

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Message.Contains("must be lowercase", StringComparison.Ordinal));
    }

    [Fact]
    public void InvalidStatusFails()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument(
            "docs/stories/US-001.md",
            ValidDocument("US-001").Replace("status: draft", "status: done", StringComparison.Ordinal));

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Message.Contains("status 'done' is not allowed", StringComparison.Ordinal));
    }

    [Fact]
    public void MissingDependencyFails()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument(
            "docs/stories/US-002.md",
            ValidDocument("US-002").Replace(
                "tags:",
                """
                dependencies:
                  - US-999
                tags:
                """,
                StringComparison.Ordinal));

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Message == "dependency US-999 does not exist");
    }

    [Fact]
    public void InvalidYamlFails()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument(
            "docs/stories/US-001.md",
            """
            ---
            id US-001
            title: Invalid YAML
            status: draft
            tags:
              - docs
            entities:
              - Story
            ---

            # Invalid YAML
            """);

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Message.Contains("is not a key-value entry", StringComparison.Ordinal));
    }

    [Fact]
    public void MissingCatalogPathFails()
    {
        using var workspace = TestWorkspace.Create();
        workspace.WriteDocument("docs/stories/US-001.md", ValidDocument("US-001"));
        workspace.WriteDocument(
            "docs/vector/catalog.md",
            """
            ---
            id: BMAD-VEC-CATALOG-001
            title: Catalog
            status: draft
            tags:
              - catalog
            entities:
              - US-999
            ---

            # Catalog

            | ID | Type | Titre | Statut | Document | Dependances principales |
            | --- | --- | --- | --- | --- | --- |
            | US-999 | Story | Missing | draft | `docs/stories/missing.md` | - |
            """);

        var result = CorpusValidator.Validate(workspace.Root);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.Message.Contains("catalog path docs/stories/missing.md does not exist", StringComparison.Ordinal));
    }

    private static string ValidDocument(string id)
    {
        return $$"""
               ---
               id: {{id}}
               title: Valid document
               status: draft
               tags:
                 - docs
               entities:
                 - Story
               ---

               # Valid document
               """;
    }

    private sealed class TestWorkspace : IDisposable
    {
        private TestWorkspace(string root)
        {
            Root = root;
        }

        public string Root { get; }

        public static TestWorkspace Create()
        {
            var root = Path.Combine(Path.GetTempPath(), "bmad-validator-tests", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "docs"));
            return new TestWorkspace(root);
        }

        public void WriteDocument(string relativePath, string content)
        {
            var path = Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }

        public void Dispose()
        {
            if (Directory.Exists(Root))
            {
                Directory.Delete(Root, recursive: true);
            }
        }
    }
}
