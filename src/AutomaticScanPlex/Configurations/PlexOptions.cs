using System.ComponentModel.DataAnnotations;

namespace AutomaticScanPlex.Configurations;

public class PlexOptions
{
    public const string Plex = "Plex";
    
    [Required(AllowEmptyStrings = false)]
    public string? Token { get; set; }
    
    [Required]
    [Url]
    public string? SectionsUrlBase { get; set; }
}
