using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Ra3MapViewer
{
    public static class MapParser
    {
        public static IEnumerable<Map> Scan(string folder)
        {
            foreach(var mapDir in Directory.GetDirectories(folder))
            {
                var path = Path.Combine(mapDir, "map.xml");
                if (!File.Exists(path))
                    continue;

                Map map = null;
                try
                {
                    map = ParseXml(path);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"[{e.GetType().Name}] ({path})");
                    continue;
                }

                yield return map;
            }

            yield break;
        }

        public static Map ParseXml(string mapXmlPath)
        {
            var xml = File.ReadAllText(mapXmlPath).Replace("uri:ea.com:eala:asset", "");
            var doc = XDocument.Parse(xml);
            var meta = doc.XPathSelectElement("/AssetDeclaration/GameMap/MapMetaData");

            var map = new Map()
            {
                Id = doc.XPathSelectElement("/AssetDeclaration/GameMap").Attribute("id").Value,
                DisplayName = meta.Attribute("DisplayName").Value,
                MaxPlayers = meta.GetAttrInt32("NumPlayers"),
                Size = new Size(meta.GetAttrDouble("Width"), meta.GetAttrDouble("Height")),
                BorderSize = meta.GetAttrInt32("BorderSize"),
                FolderPath = Path.GetDirectoryName(mapXmlPath),
            };

            map.FileId = Path.GetFileName(map.FolderPath);

            map.PlayerPoints = meta.Elements("StartPosition")
                .Select(e => new
                {
                    Name = e.Attribute("Name").Value,
                    Position = new Point(
                        e.Element("Position").GetAttrDouble("x"),
                        e.Element("Position").GetAttrDouble("y"))
                })
                .Where(a => a.Name.StartsWith("Player_"))
                .OrderBy(a => a.Name)
                .Select(a => a.Position).ToList();

            return map;
        }

        private static int GetAttrInt32(this XElement element, string attr) => int.Parse(element.Attribute(attr)?.Value);

        private static double GetAttrDouble(this XElement element, string attr) => double.Parse(element.Attribute(attr)?.Value);
    }
}
