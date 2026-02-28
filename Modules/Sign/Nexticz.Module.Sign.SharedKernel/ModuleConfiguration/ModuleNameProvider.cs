namespace Nexticz.Module.Sign.SharedKernel.ModuleConfiguration;

public static class ModuleNameProvider
{
    public const string Name = nameof(ModuleName.Sign);
    public static readonly string MassTransitModulePrefixName = $"module-{Name.ToLowerInvariant()}";
}