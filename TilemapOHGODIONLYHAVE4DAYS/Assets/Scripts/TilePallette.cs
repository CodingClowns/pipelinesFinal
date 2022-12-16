using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class TilePallette
{
    public string[] paths = new string[64];
    public int count;
    public int Count { get => count; set => count = value; }

    public TilePallette()
    {
        Count = 0;
    }

    public void Serialize(string path)
    {
        var stream = new StreamWriter(new FileStream(path, FileMode.Create));
        new XmlSerializer(typeof(TilePallette)).Serialize(stream, this);
        stream.Close();
    }

    public static TilePallette Deserialize(string path)
    {
        var stream = new StreamReader(new FileStream(path, FileMode.Open));
        var serializer = new XmlSerializer(typeof(TilePallette));
        var reader = XmlReader.Create(stream);

        if (!serializer.CanDeserialize(reader))
        {
            return null;
        }

        var pallette = (TilePallette)serializer.Deserialize(reader);
        stream.Close();
        return pallette;
    }
}
