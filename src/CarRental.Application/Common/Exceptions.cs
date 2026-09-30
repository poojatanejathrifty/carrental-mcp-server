namespace CarRental.Application.Common;

public class NotFoundException(string entity, object key)
    : Exception($"{entity} with id '{key}' was not found.");

public class ConflictException(string message) : Exception(message);

public class BusinessRuleException(string message) : Exception(message);
