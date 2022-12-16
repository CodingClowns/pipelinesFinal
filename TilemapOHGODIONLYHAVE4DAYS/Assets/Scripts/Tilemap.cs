using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

/// <summary>
/// Storage class for tilemap data.
/// </summary>
public class Tilemap
{
    public int[][] tileData;

    [XmlIgnore]
    public IntWrapper[][] tile;

    public Tilemap()
    {
        tile = new IntWrapper[16][];
        tileData = new int[16][];

        for (int x = 0; x < 16; x ++)
        {
            tile[x] = new IntWrapper[16];
            tileData[x] = new int[16];
            for (int y = 0; y < 16; y++)
            {
                tile[x][y] = new IntWrapper(0);
                tileData[x][y] = 0;
            }
        }
    }

    public void Serialize(string path)
    {
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                tileData[x][y] = tile[x][y].value;
            }
        }
        var stream = new StreamWriter(new FileStream(path, FileMode.Create));
        new XmlSerializer(typeof(Tilemap)).Serialize(stream, this);
        stream.Close();
    }

    public static Tilemap Deserialize(string path)
    {
        var stream = new StreamReader(new FileStream(path, FileMode.Open));
        var serializer = new XmlSerializer(typeof(Tilemap));
        var reader = XmlReader.Create(stream);

        if (!serializer.CanDeserialize(reader))
        {
            return null;
        }

        var tilemap = (Tilemap)serializer.Deserialize(reader);
        stream.Close();
        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                tilemap.tile[x][y].value = tilemap.tileData[x][y];
            }
        }
        return tilemap;
    }
}
