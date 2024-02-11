using System.Xml.Serialization;

namespace AutomaticScanPlex.Dtos;

[XmlRoot(ElementName = "Location")]
public class Location
{
    [XmlAttribute(AttributeName = "id")]
    public string Id { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "path")]
    public string Path { get; set; } = string.Empty;
}

[XmlRoot(ElementName = "Directory")]
public class Directory
{
    [XmlElement(ElementName = "Location")]
    public Location? Location { get; set; }
    
    [XmlAttribute(AttributeName = "allowSync")]
    public string AllowSync { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "art")]
    public string Art { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "composite")]
    public string Composite { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "filters")]
    public string Filters { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "refreshing")]
    public string Refreshing { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "thumb")]
    public string Thumb { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "key")]
    public string Key { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "type")]
    public string Type { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "title")]
    public string Title { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "agent")]
    public string Agent { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "scanner")]
    public string Scanner { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "language")]
    public string Language { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "uuid")]
    public string Uuid { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "updatedAt")]
    public string UpdatedAt { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "createdAt")]
    public string CreatedAt { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "scannedAt")]
    public string ScannedAt { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "content")]
    public string Content { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "directory")]
    public string DirectoryPath { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "contentChangedAt")]
    public string ContentChangedAt { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "hidden")]
    public string Hidden { get; set; } = string.Empty;
}

[XmlRoot(ElementName = "MediaContainer")]
public class MediaContainerDto
{
    [XmlElement(ElementName = "Directory")]
    public List<Directory>? Directories { get; set; }
    [XmlAttribute(AttributeName = "size")]
    public string Size { get; set; } = string.Empty;
    [XmlAttribute(AttributeName = "allowSync")]
    public string AllowSync { get; set; } = string.Empty;
    [XmlAttribute(AttributeName = "title1")]
    public string Title1 { get; set; } = string.Empty;
}