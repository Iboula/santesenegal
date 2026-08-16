using SanteSenegal.BmadValidator;

var rootPath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
var result = CorpusValidator.Validate(rootPath);

Console.WriteLine("BMAD corpus validation");
Console.WriteLine();
Console.WriteLine($"Documents: {result.DocumentCount}");
Console.WriteLine($"IDs: {result.UniqueIdCount}");
Console.WriteLine($"Broken dependencies: {result.BrokenDependencyCount}");
Console.WriteLine($"Invalid metadata: {result.InvalidMetadataCount}");
Console.WriteLine($"Invalid catalog entries: {result.InvalidCatalogEntryCount}");
Console.WriteLine();

if (result.IsValid)
{
    Console.WriteLine("VALID");
    return 0;
}

Console.WriteLine("INVALID");
Console.WriteLine();

foreach (var group in result.Errors.GroupBy(error => error.DocumentIdOrPath))
{
    Console.WriteLine(group.Key);
    foreach (var error in group)
    {
        Console.WriteLine($"  - {error.Message}");
    }
}

return 1;
