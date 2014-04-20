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
    public class JSONHotkeyContext : IContext<Hotkey>
    {
        // Hotkey.GetHashCode -> Hotkey
        private List<Hotkey> data;

        public IQueryable<Hotkey> Collection { get { return data.AsQueryable(); } }
        private readonly string SaveFilePath;

        public JSONHotkeyContext(string path)
        {
            SaveFilePath = path;
            data = new List<Hotkey>();
        }

        public void Save()
        {
            FileStream saveFile = null;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer( typeof(Hotkey[]) );

            try
            {
                saveFile = File.Create(SaveFilePath);
                if (saveFile.CanWrite)
                {
                    serializer.WriteObject(saveFile, data.ToArray());
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
            DataContractJsonSerializer serializer = new DataContractJsonSerializer( typeof(Hotkey[]) );
            
            if (!File.Exists(SaveFilePath)) return;

            try
            {
                saveFile = File.OpenRead(SaveFilePath);
                if (saveFile.CanRead)
                {
                    Hotkey[] hotkeys = null;
                    try
                    {
                        saveFile.Position = 0;
                        hotkeys = (Hotkey[])serializer.ReadObject(saveFile);
                        for (int i = 0; i < hotkeys.Length; i++)
                        {
                            if (hotkeys[i] != null && hotkeys[i].FilePath != null)
                            {
                                data.Add(hotkeys[i]); // AddSavedHotkey(hotkeys[i]);
                            }
                        }
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

        public void Add(Hotkey entity)
        {
            if (!Collection.Contains(entity))
                data.Add(entity);
            else
                Update(entity);
        }

        public void Remove(int hashCode)
        {
            data.Remove(data.Find(x => x.Id == hashCode));
        }

        public void Update(Hotkey entity)
        {
            data.Remove(entity);
            data.Add(entity);
        }


        public void Remove(Hotkey entity)
        {
            data.Remove(entity);
        }


        public ICollection<Hotkey> GetAll()
        {
            return data.ToList();
        }
    }
}
