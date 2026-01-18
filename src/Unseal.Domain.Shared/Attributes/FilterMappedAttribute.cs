using System;

namespace Unseal.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class FilterMappedAttribute : Attribute
{
    public string MapTo { get; }

    public FilterMappedAttribute(string mapTo)
    {
        MapTo = mapTo;
    }
}