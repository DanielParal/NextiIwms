using Nexticz.Module.Mmo.SharedKernel.FileHandling;

namespace Nexticz.Module.Mmo.Washing.Application.FileHandling;

internal record KitInstructionResult(byte[] ContentBytes, string ContentType, string FileName) 
    : FileResult(ContentBytes, ContentType, FileName);