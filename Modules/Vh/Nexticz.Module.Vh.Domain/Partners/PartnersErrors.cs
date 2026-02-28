using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.Partners;

public abstract class PartnersErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-partnerService-";

    public static Error PartnerWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "partnerWithIdDoesnotExist",
        "Partner s tímto ID neexistuje");
}