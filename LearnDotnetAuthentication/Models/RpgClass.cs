using System.Text.Json.Serialization;

namespace LearnDotnetAuthentication.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RpgClass
{
    Knight = 1,
    Mage = 2,
    Cleric = 3
}