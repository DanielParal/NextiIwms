using ErrorOr;
using Nexticz.Lib.Shared.Errors;

namespace Nexticz.Module.Vh.Domain.LoadingDevices;

public abstract class LoadingDeviceErrors : IErrorComponentSlugProvider
{
    public static string ComponentSlug { get; } = "vh-api-loadingDevice-";

    public static Error LoadingDeviceWithIdDoesnotExist => Error.Validation(
        ComponentSlug + "loadingDeviceWithIdDoesnotExist",
        "Načítací zařízení s tímto ID neexistuje");
    
    public static Error LoadingDeviceCanNotBeRegisteredError => Error.Validation(
        ComponentSlug + "loadingDeviceCanNotBeRegisteredError",
        "Načítací zařízení nemůže být zaregistrováno");
}