using Nexticz.Module.Mmo.Washing.Application.Batches;
using Shouldly;

namespace Nexticz.Module.Mmo.Washing.Tests.Unit.Application;

public class SapBarcodeComposerTests
{
    
    [Fact]
    public void Compose_SinglePlaceholder_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample" };
        var template = "*{{Name}}401KOMPLETY*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*Sample401KOMPLETY*");
    }

    [Fact]
    public void Compose_MultiplePlaceholders_ReplacesAllValues()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample", Number = 123 };
        var template = "*{{Name}}-{{Number}}*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*Sample-123*");
    }

    [Fact]
    public void Compose_NonExistentProperty_LeavesPlaceholder()
    {
        // Arrange
        var obj = new TestClass();
        var template = "*{{NonExistent}}401KOMPLETY*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*{{NonExistent}}401KOMPLETY*");
    }

    [Fact]
    public void Compose_NullPropertyValue_LeavesPlaceholder()
    {
        // Arrange
        var obj = new TestClass { NullableString = null };
        var template = "*{{NullableString}}401KOMPLETY*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*{{NullableString}}401KOMPLETY*");
    }

    [Fact]
    public void Compose_EmptyTemplate_ReturnsEmptyString()
    {
        // Arrange
        var obj = new TestClass();
        var template = "";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("");
    }

    [Fact]
    public void Compose_NoPlaceholders_ReturnsUnchangedTemplate()
    {
        // Arrange
        var obj = new TestClass();
        var template = "*401KOMPLETY*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*401KOMPLETY*");
    }

    [Fact]
    public void Compose_NullSourceObject_ThrowsArgumentNullException()
    {
        // Arrange
        object? obj = null;
        var template = "*{{Name}}401KOMPLETY*";

        // Act & Assert
        Should.Throw<NullReferenceException>(() => SapBarcodeComposer.Compose(obj!, template));
    }

    [Fact]
    public void Compose_NullTemplate_ThrowsArgumentNullException()
    {
        // Arrange
        var obj = new TestClass();
        string? template = null;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => SapBarcodeComposer.Compose(obj, template!));
    }

    [Fact]
    public void Compose_MalformedPlaceholder_LeavesPlaceholder()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample" };
        var template = "*{{Name401KOMPLETY*"; // Missing }}

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*{{Name401KOMPLETY*");
    }

    [Fact]
    public void Compose_EmptyPlaceholder_SkipsReplacement()
    {
        // Arrange
        var obj = new TestClass();
        var template = "*{{}}401KOMPLETY*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*{{}}401KOMPLETY*");
    }
    
    [Fact]
    public void Compose_SinglePlaceholderWithTrueConditionWithInt_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample", Number = 30 };
        var template = "*{{Name}}[[Number=30;401True;100False]]*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*Sample401True*");
    }
    
    [Fact]
    public void Compose_SinglePlaceholderWithFalseConditionWithInt_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample", Number = 20 };
        var template = "*{{Name}}[[Number=30;401True;100False]]*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*Sample100False*");
    }
    
    [Fact]
    public void Compose_SinglePlaceholderWithTrueConditionWithString_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample" };
        var template = "*{{Name}}[[Name=Sample;401True;100False]]*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*Sample401True*");
    }
    
    [Fact]
    public void Compose_SinglePlaceholderWithFalseConditionWithString_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample" };
        var template = "*{{Name}}[[Name=NonExisting;401True;100False]]*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*Sample100False*");
    }
    
    [Fact]
    public void Compose_SinglePlaceholderWithMultipleConditions_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample", Number = 20 };
        var template = "*{{Name}}[[Name=Sample;TextTrue;TextFalse]]SomeText[[Number=30;IntTrue;IntFalse]]*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*SampleTextTrueSomeTextIntFalse*");
    }
    
    [Fact]
    public void Compose_SinglePlaceholderWithConditionWithIsNullOrWhitespaceFalseCondition_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "Sample" };
        var template = "*{{Name}}[[Name=;TextTrue;TextFalse]]SomeText*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*SampleTextFalseSomeText*");
    }
    
    [Fact]
    public void Compose_SinglePlaceholderWithConditionWithIsNullOrWhitespaceTrueCondition_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new TestClass { Name = "" };
        var template = "*{{Name}}[[Name=;TextTrue;TextFalse]]SomeText*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*TextTrueSomeText*");
    }
    
    [Fact]
    public void Compose_RealTemplateForBoshWithTrueCondition_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new ObjectForBarcodeComposer("6000010465", "EM");
        var template = "*P{{KitNumber}}[[ManufactureCode=EM;Q1$DPE3-PSA30535;null]]*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*P6000010465Q1$DPE3-PSA30535*");
    }
    
    [Fact]
    public void Compose_RealTemplateForBoshWithFalseCondition_ReplacesWithPropertyValue()
    {
        // Arrange
        var obj = new ObjectForBarcodeComposer("6000010465", "DO");
        var template = "*P{{KitNumber}}[[ManufactureCode=EM;Q1$DPE3-PSA30535;null]]*";

        // Act
        var result = SapBarcodeComposer.Compose(obj, template);

        // Assert
        result.ShouldBe("*P6000010465*");
    }
    
    private class TestClass
    {
        public string Name { get; set; } = "Test";
        public int Number { get; set; } = 42;
        public string? NullableString { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    private record ObjectForBarcodeComposer(string KitNumber, string ManufactureCode);
}