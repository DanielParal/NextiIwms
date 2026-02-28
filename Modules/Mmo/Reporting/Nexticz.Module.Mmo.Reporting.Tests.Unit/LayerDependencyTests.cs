using Shouldly;

namespace Nexticz.Module.Mmo.Reporting.Tests.Unit;

public class LayerDependencyTests
{
    private const string RootNamespace = "Nexticz.Module.Mmo.Reporting";
    private const string Domain = "Domain";
    private const string DomainNamespace = $"{RootNamespace}.{Domain}";
    private const string Application = "Application";
    private const string ApplicationNamespace = $"{RootNamespace}.{Application}";
    private const string Infrastructure = "Infrastructure";
    private const string InfrastructureNamespace = $"{RootNamespace}.{Infrastructure}";
    private const string Presentation = "Presentation";
    private const string PresentationNamespace = $"{RootNamespace}.{Presentation}";
    
    private static readonly string RootPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, $"../../../../{RootNamespace}"));

    [Fact]
    public void Domain_ShouldNotDependOnOtherLayers()
    {
        var domainFolder = Path.Combine(RootPath, Domain);
        string[] forbiddenNamespaces = [ApplicationNamespace, InfrastructureNamespace, PresentationNamespace];

        ValidateNoForbiddenReferences(domainFolder, forbiddenNamespaces);
    }
    
    [Fact]
    public void Application_ShouldNotDependOnPresentationOrInfrastructureLayers()
    {
        var applicationFolder = Path.Combine(RootPath, Application);
        string[] forbiddenNamespaces = [PresentationNamespace, InfrastructureNamespace];

        ValidateNoForbiddenReferences(applicationFolder, forbiddenNamespaces);
    }
    
    [Fact]
    public void Presentation_ShouldNotDependOnInfrastructureLayers()
    {
        var presentationFolder = Path.Combine(RootPath, Presentation);
        string[] forbiddenNamespaces = [InfrastructureNamespace];

        ValidateNoForbiddenReferences(presentationFolder, forbiddenNamespaces);
    }
    
    [Fact]
    public void Infrastructure_ShouldNotDependOnPresentationLayer()
    {
        var infrastructureFolder = Path.Combine(RootPath, Infrastructure);
        string[] forbiddenNamespaces = [PresentationNamespace];

        ValidateNoForbiddenReferences(infrastructureFolder, forbiddenNamespaces);
    }
    
    private static void ValidateNoForbiddenReferences(string folderPath, string[] forbiddenNamespaces)
    {
        var files = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var content = File.ReadAllText(file);
            foreach (var ns in forbiddenNamespaces)
            {
                content.ShouldNotContain(ns);
            }
        }
    }
}