using NHkey.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace NHkey.Data
{
    public sealed class XMLHotkeyContext : IContext<HotkeyAssociation>
    {
        private List<HotkeyAssociation> data;

        public IQueryable<HotkeyAssociation> Collection { get { return data.AsQueryable(); } }

        private readonly string SaveFilePath;

        bool loaded;

        public XMLHotkeyContext(string path)
        {
            SaveFilePath = path;
            data = new List<HotkeyAssociation>();
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(HotkeyData[]));

            var hotkeyData = data.ConvertAll<HotkeyData>((hotkey) => HotkeyData.GetData(hotkey)).ToArray();

            using (var stream = GetWriteStream(SaveFilePath))
            {
                if (stream != null)
                    serializer.Serialize(stream, hotkeyData);
            }
        }

        public StreamWriter GetWriteStream(string path)
        {
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(path);
            }
            catch(UnauthorizedAccessException access)
            {
                App.Instance.Log.Append(GetType().Name, "Cannot open write stream : " + access.Message);
            }
            catch(DirectoryNotFoundException dirNf)
            {
                App.Instance.Log.Append(GetType().Name, "Cannot open write stream : " + dirNf.Message);
            }

            return writer;
        }

        public StreamReader GetReadStream(string path)
        {
            StreamReader writer = null;
            try
            {
                if (File.Exists(path))
                    writer = new StreamReader(path);
            }
            catch (UnauthorizedAccessException access)
            {
                App.Instance.Log.Append(GetType().Name, "Cannot open write stream : " + access.Message);
            }
            catch (DirectoryNotFoundException dirNf)
            {
                App.Instance.Log.Append(GetType().Name, "Cannot open write stream : " + dirNf.Message);
            }

            return writer;
        }

        public void Load()
        {
            var serializer = new XmlSerializer(typeof(HotkeyData[]));

            using (StreamReader stream = GetReadStream(SaveFilePath))
            {
                HotkeyData[] hotkeys = null;

                if (stream != null)
                {
                    hotkeys = (HotkeyData[])serializer.Deserialize(stream);
                    data = hotkeys.ToList().ConvertAll<HotkeyAssociation>((hotkey) => HotkeyData.GetHotkey(hotkey));
                    loaded = true;
                }
            }
        }

        public void Add(HotkeyAssociation entity)
        {
            if (!Collection.Contains(entity))
            {
                data.Add(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public void Add(List<HotkeyAssociation> entities)
        {
            foreach (var hk in entities)
            {
                if (!data.Contains(hk))
                {
                    Add(hk);
                }
                else
                {
                    Update(hk);
                }
            }
        }

        public void Remove(int hashCode)
        {
            data.Remove(data.Find(x => x.Hotkey.Id == hashCode));
        }

        public void Update(HotkeyAssociation entity)
        {
            data.Remove(entity);
            data.Add(entity);
        }


        public void Remove(HotkeyAssociation entity)
        {
            data.Remove(entity);
        }


        public ICollection<HotkeyAssociation> GetAll()
        {
            if (!loaded) { Load(); }
            return data.ToList();
        }
    }
}
