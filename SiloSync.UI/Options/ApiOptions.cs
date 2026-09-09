using System.ComponentModel.DataAnnotations;

namespace SiloSync.UI.Options;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>Base URL of SiloSync.Api, e.g. http://localhost:5132</summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;
}
