namespace Unamora.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Validation failed")
    {
        Errors = new Dictionary<string, string[]>(errors);
    }

    public ValidationException(string property, string error)
        : base("Validation failed")
    {
        Errors = new Dictionary<string, string[]> { [property] = new[] { error } };
    }
}

public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}

public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}
