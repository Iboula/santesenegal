namespace SanteSenegal.BmadValidator;

public sealed record ValidationError(string DocumentIdOrPath, string Message, ValidationErrorKind Kind);

public enum ValidationErrorKind
{
    Metadata,
    Dependency,
    Catalog
}
