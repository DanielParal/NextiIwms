using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace Nexticz.Module.Auth.Infrastructure.Identity;

internal static class IdentityPasswordValidationMapper
{
    public static List<Error> CreateErrors(IEnumerable<IdentityError> identityErrors)
    {
        var errors = identityErrors.Select(identityError => identityError.Code switch
            {
                nameof(IdentityPasswordValidationError.InvalidEmail) => IdentityPasswordValidationErrors.InvalidEmail,
                nameof(IdentityPasswordValidationError.DuplicateEmail) => IdentityPasswordValidationErrors.DuplicateEmail,
                nameof(IdentityPasswordValidationError.PasswordTooShort) => IdentityPasswordValidationErrors.PasswordTooShort,
                nameof(IdentityPasswordValidationError.PasswordRequiresNonAlphanumeric) => IdentityPasswordValidationErrors.PasswordRequiresNonAlphanumeric,
                nameof(IdentityPasswordValidationError.PasswordRequiresDigit) => IdentityPasswordValidationErrors.PasswordRequiresDigit,
                nameof(IdentityPasswordValidationError.PasswordRequiresUpper) => IdentityPasswordValidationErrors.PasswordRequiresUpper,
                nameof(IdentityPasswordValidationError.PasswordRequiresLower) => IdentityPasswordValidationErrors.PasswordRequiresLower,
                nameof(IdentityPasswordValidationError.PasswordRequiresUniqueChars) => IdentityPasswordValidationErrors.PasswordRequiresUniqueChars,
                _ => IdentityPasswordValidationErrors.PasswordUnspecifiedError
            })
            .ToList();
        
        return errors;
    }
}

public enum IdentityPasswordValidationError
{
    InvalidEmail,
    DuplicateEmail,
    PasswordTooShort,
    PasswordRequiresNonAlphanumeric,
    PasswordRequiresDigit,
    PasswordRequiresUpper,
    PasswordRequiresLower,
    PasswordRequiresUniqueChars
}