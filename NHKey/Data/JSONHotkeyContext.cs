using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using NHkey.Model;

namespace NHkey.Data
{
    public sealed class JSONHotkeyContext : IContext<HotkeyAssociation>
    {
        private List<HotkeyAssociation> data;

        public IQueryable<HotkeyAssociation> Collection { get { return data.AsQueryable(); } }

        private readonly string SaveFilePath;

        bool loaded;

        public JSONHotkeyContext(string path)
        {
            SaveFilePath = path;
            data = new List<HotkeyAssociation>();
        }

        public void Save()
        {
            FileStream saveFile = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HotkeyData[]));
            HotkeyData[] hotkeyData = data.ConvertAll<HotkeyData>((hk) => HotkeyData.GetData(hk)).ToArray();

            try
            {
                saveFile = File.Create(SaveFilePath);
                if (saveFile.CanWrite)
                {
                    serializer.WriteObject(saveFile, hotkeyData);
                }
            }
            catch (IOException except)
            {
                throw except;
            }
            finally
            {
                if (saveFile != null)
                    saveFile.Close();
            }
        }

        public void Load()
        {
            FileStream saveFile = null;

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HotkeyData[]));

            if (!File.Exists(SaveFilePath)) return;

            loaded = true;
            try
            {
                saveFile = File.OpenRead(SaveFilePath);

                if (saveFile.CanRead)
                {
                    HotkeyData[] hotkeys = null;
                    try
                    {
                        hotkeys = (HotkeyData[])serializer.ReadObject(saveFile);
                        data = hotkeys.ToList().ConvertAll<HotkeyAssociation>((hotkeyData) => HotkeyData.GetHotkey(hotkeyData));
                    }
                    catch (SerializationException ex)
                    {
                        throw ex;
                        //System.Windows.MessageBox.Show(ex.Message);
                    }

                }
            }
            catch (IOException except)
            {
                throw except;
                //System.Windows.MessageBox.Show(except.Message);
            }
            finally
            {
                if (saveFile != null)
                    saveFile.Close();
            }
        }

        public void Add(HotkeyAssociation entity)
        {
            if (!data.Contains(entity))
                data.Add(entity);
            else
                Update(entity);
        }

        public void Add(List<HotkeyAssociation> entities)
        {
            foreach (var hk in entities)
            {
                if (!data.Contains(hk))
                    Add(hk);
                else
                    Update(hk);
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
            if (!loaded)
            {
                Load();
            }
            return data.ToList();
        }
    }
}
