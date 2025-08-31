namespace Accounting.Validation;

public class ValidationErrors
{
    public Dictionary<string, string[]> Errors { get; set; } = new();
}