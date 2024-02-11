using System.Xml.Serialization;

namespace AutomaticScanPlex.Dtos;

[XmlRoot(ElementName = "Location")]
public class Location
{
    [XmlAttribute(AttributeName = "id")]
    public long Id { get; set; }

    [XmlAttribute(AttributeName = "path")]
    public string? Path { get; set; }
}

[XmlRoot(ElementName = "Directory")]
public class Directory
{
    [XmlElement(ElementName = "Location")]
    public Location? Location { get; set; }
    
    [XmlAttribute(AttributeName = "allowSync")]
    public string? AllowSync { get; set; }

    [XmlAttribute(AttributeName = "art")]
    public string? Art { get; set; }

    [XmlAttribute(AttributeName = "composite")]
    public string? Composite { get; set; }

    [XmlAttribute(AttributeName = "filters")]
    public string? Filters { get; set; }

    [XmlAttribute(AttributeName = "refreshing")]
    public string? Refreshing { get; set; }

    [XmlAttribute(AttributeName = "thumb")]
    public string? Thumb { get; set; }

    [XmlAttribute(AttributeName = "key")]
    public long Key { get; set; }

    [XmlAttribute(AttributeName = "type")]
    public string? Type { get; set; }

    [XmlAttribute(AttributeName = "title")]
    public string? Title { get; set; }

    [XmlAttribute(AttributeName = "agent")]
    public string? Agent { get; set; }

    [XmlAttribute(AttributeName = "scanner")]
    public string? Scanner { get; set; }

    [XmlAttribute(AttributeName = "language")]
    public string? Language { get; set; }

    [XmlAttribute(AttributeName = "uuid")]
    public string? Uuid { get; set; }

    [XmlAttribute(AttributeName = "updatedAt")]
    public string? UpdatedAt { get; set; }

    [XmlAttribute(AttributeName = "createdAt")]
    public string? CreatedAt { get; set; }

    [XmlAttribute(AttributeName = "scannedAt")]
    public string? ScannedAt { get; set; }

    [XmlAttribute(AttributeName = "content")]
    public string? Content { get; set; }

    [XmlAttribute(AttributeName = "directory")]
    public string? DirectoryPath { get; set; }

    [XmlAttribute(AttributeName = "contentChangedAt")]
    public string? ContentChangedAt { get; set; }

    [XmlAttribute(AttributeName = "hidden")]
    public string? Hidden { get; set; }
}

[XmlRoot(ElementName = "MediaContainer")]
public class MediaContainerDto
{
    [XmlElement(ElementName = "Directory")]
    public List<Directory>? Directories { get; set; }
    [XmlAttribute(AttributeName = "size")]
    public string? Size { get; set; }
    [XmlAttribute(AttributeName = "allowSync")]
    public string? AllowSync { get; set; }
    [XmlAttribute(AttributeName = "title1")]
    public string? Title { get; set; }
}