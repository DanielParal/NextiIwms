namespace Nexticz.OnPremise.Deployment.Cli.Utilities;

public static class ErrorMessages
{
    public static readonly string DisplayMenuNotConfirmedCode = "DisplayMenu.NotConfirmedOption";
    public static readonly string DisplayMenuNotConfirmedDescription = "Not confirmed option.";
    
    public static readonly string VersionBuilderCreateVersionFileCode = "VersionBuilder.CreateVersionFile";
    public static string VersionBuilderCreateVersionFileDescription(string exception) => $"Failed to create files. Exception: {exception}";
    
    
    public static readonly string ProcessRunnerFailureCode = "ProcessRunner.FailedToRun";
    public static readonly string ProcessRunnerFailureUnexpectedCode = "ProcessRunner.UnexpectedFailure";
    public static string ProcessRunnerFailureDescription(string command, string arguments, string exception) => 
        $"Failed to run process runner. Exception: {exception}, Command: {command}, Arguments: {arguments}";
    
    public static readonly string EnvironmentExtractorNotFoundCode = "EnvironmentExtractor.NotFound";
    public static string EnvironmentExtractorNotFoundDescription(string environmentNamePattern) => 
        $"We didn't find any environment file with the pattern: {environmentNamePattern}";
    
    
    public static readonly string ImageNumberExtractorNotFoundCode = "ImageNumberExtractor.NotFound";
    public static string ImageNumberExtractorNotFoundDescription(string searchPattern) => 
        $"We didn't find any version file with the pattern: {searchPattern}";
    
    public static readonly string FileNameParserNotFoundCode = "FileNameParser.NotFound";
    public static string FileNameParserNotFoundDescription(string input) => 
        $"We couldn't extract last part from input: {input}";
    
    public static readonly string AzureKeyVaultDownloaderFailureCode = "AzureKeyVaultDownloader.GetSecrets";
    public static string AzureKeyVaultDownloaderFailureDescription(string exception) => 
        $"We couldn't get secrets from Azure key vault. Exception: {exception}";
}