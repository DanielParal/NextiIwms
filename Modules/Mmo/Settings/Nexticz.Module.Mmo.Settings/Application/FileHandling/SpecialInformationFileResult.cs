using Nexticz.Module.Mmo.SharedKernel.FileHandling;

namespace Nexticz.Module.Mmo.Settings.Application.FileHandling;

internal record SpecialInformationFileResult(byte[] ContentBytes, string ContentType, string FileName) 
    : FileResult(ContentBytes, ContentType, FileName);