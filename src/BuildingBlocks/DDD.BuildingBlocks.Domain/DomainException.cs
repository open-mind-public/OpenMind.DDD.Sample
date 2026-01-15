namespace BuildingBlocks.Domain;

/// <summary>
/// Base exception for domain rule violations.
/// Thrown when a domain invariant is violated.
/// </summary>
public class DomainException : Exception
{
    public string Code { get; }

    public DomainException(string message) : base(message)
    {
        Code = "DOMAIN_ERROR";
    }

    public DomainException(string code, string message) : base(message)
    {
        Code = code;
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
        Code = "DOMAIN_ERROR";
    }
}

public class EntityNotFoundException(string entityName, object id)
    : DomainException("ENTITY_NOT_FOUND", $"{entityName} with id '{id}' was not found.");
