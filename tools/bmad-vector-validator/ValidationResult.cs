namespace SanteSenegal.BmadValidator;

public sealed record ValidationResult(
    int DocumentCount,
    int UniqueIdCount,
    IReadOnlyList<ValidationError> Errors)
{
    public bool IsValid => Errors.Count == 0;

    public int BrokenDependencyCount => Errors.Count(error => error.Kind == ValidationErrorKind.Dependency);

    public int InvalidMetadataCount => Errors.Count(error => error.Kind == ValidationErrorKind.Metadata);

    public int InvalidCatalogEntryCount => Errors.Count(error => error.Kind == ValidationErrorKind.Catalog);
}
