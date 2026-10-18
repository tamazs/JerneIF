namespace Api;

/// <summary>Thrown when an authenticated caller is not allowed to touch the requested resource.</summary>
public class ForbiddenException(string message) : Exception(message);
