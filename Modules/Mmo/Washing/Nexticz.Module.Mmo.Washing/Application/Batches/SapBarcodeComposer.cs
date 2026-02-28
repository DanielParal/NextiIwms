using System.Text.RegularExpressions;

namespace Nexticz.Module.Mmo.Washing.Application.Batches;

internal static class SapBarcodeComposer
{
    public static string Compose(object sourceObj, string barcodeTemplate)
    {
        barcodeTemplate = ReplacePropertyValues(barcodeTemplate, sourceObj);
        barcodeTemplate = ReplaceConditionsWithValues(barcodeTemplate, sourceObj);
        
        return barcodeTemplate;
    }

    private static string ReplaceConditionsWithValues(string barcodeTemplate, object sourceObj)
    {
        const string pattern = @"\[\[(.*?)\]\]";
        var matches = Regex.Matches(barcodeTemplate, pattern);
        var conditions = matches
            .Select(m => m.Groups[1].Value)
            .ToList();

        foreach (var condition in conditions)
        {
            var conditionParts = condition.Split(';', StringSplitOptions.RemoveEmptyEntries);
            
            if (conditionParts.Length != 3)
                continue;
            
            var conditionValue = conditionParts[0];
            var trueValue = GetConditionTrueFalseValue(conditionParts[1]);
            var falseValue = GetConditionTrueFalseValue(conditionParts[2]);
            
            var value = IsConditionTrue(conditionValue, sourceObj) ? trueValue : falseValue;
            barcodeTemplate = barcodeTemplate.Replace($"[[{condition}]]", value);
        }
        
        return barcodeTemplate;
    }

    private static string GetConditionTrueFalseValue(string value)
    {
        return value == "null" ? string.Empty : value;
    }

    private static bool IsConditionTrue(string condition, object sourceObj)
    {
        var equalIndex = condition.IndexOf('=');
        
        if (equalIndex == -1)
            return false;
        
        var propertyName = condition[..equalIndex];
        var propertyValue = condition[(equalIndex + 1)..];
        
        var propertyValueFromSource = GetPropertyValue(sourceObj, propertyName);
        return propertyValueFromSource == propertyValue;
    }

    private static string ReplacePropertyValues(string barcodeTemplate, object sourceObj)
    {
        const string pattern = @"\{\{(.*?)\}\}";
        var matches = Regex.Matches(barcodeTemplate, pattern);
        var propertyNames = matches
            .Select(m => m.Groups[1].Value)
            .ToList();

        foreach (var propertyName in propertyNames)
        {
            var propertyValue = GetPropertyValue(sourceObj, propertyName);
            if (propertyValue is null)
                continue;
            
            barcodeTemplate = barcodeTemplate.Replace($"{{{{{propertyName}}}}}", propertyValue);
        }
        
        return barcodeTemplate;
    }
    
    private static string? GetPropertyValue(this object obj, string propertyName)
    {
        var property = obj.GetType().GetProperty(propertyName);
        var value = property?.GetValue(obj);
        return value?.ToString();
    }
}