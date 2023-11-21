using System.Collections.Generic;
using System.Xml.Serialization;

using LogRipper.Constants;

namespace LogRipper.Models;

[XmlRoot()]
public class OneTabSearch
{
    [XmlElement()]
    public string Search { get; set; }

    [XmlElement()]
    public ECurrentSearchMode SearchMode { get; set; }

    [XmlElement()]
    public List<OneRule> SearchRules { get; set; }

    [XmlElement()]
    public List<OneLine> Result { get; set; }
}
