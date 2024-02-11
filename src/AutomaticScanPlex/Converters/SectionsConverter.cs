using AutomaticScanPlex.Models;
using AutomaticScanPlex.Dtos;

namespace AutomaticScanPlex.Converters;

public class SectionsConverter
{
    public List<Section> Convert(MediaContainerDto? mediaContainerDto)
    {
        if (mediaContainerDto is null || mediaContainerDto.Directories is null)
        {
            return Enumerable.Empty<Section>().ToList();
        }
        
        var sections = new List<Section>();

        foreach (var sectionDto in mediaContainerDto.Directories)
        {
            var section = Convert(sectionDto);
            
            if (section is not null)
            {
                sections.Add(section);
            }
        }

        return sections;
    }

    public Section? Convert(Dtos.Directory directory) 
    { 
        if (directory is null)
        {
            return default;
        }

        return new Section
        {
            Id = directory.Key,
            Name = directory.Title,
            Path = directory.Location?.Path
        };
    }
}
