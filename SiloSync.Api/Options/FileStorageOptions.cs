using System.ComponentModel.DataAnnotations;

namespace SiloSync.Api.Options;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";

    [Required]
    public string RootPath { get; set; } = string.Empty;

    [Required]
    [Url]
    public string PublicBaseUrl { get; set; } = string.Empty;
}
