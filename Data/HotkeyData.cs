using NHkey.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Data;

namespace NHkey.Data
{
    [DataContract(Name="HotkeyData")]
    public class HotkeyData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string FilePath { get; set; }

        [DataMember]
        public string Parameters { get; set; }

        [DataMember]
        public int Key { get; set; }

        [DataMember]
        public int Modifiers { get; set; }

        [DataMember]
        public bool Orphaned { get; set; }

        public HotkeyData()
        {
        }

        public HotkeyData(HotkeyAssociation hotkeyAssociation)
        {
            Name = hotkeyAssociation.Name;
            FilePath = hotkeyAssociation.FilePath;
            Parameters = hotkeyAssociation.Parameters;
            Key = hotkeyAssociation.Hotkey.Key;
            Modifiers = hotkeyAssociation.Hotkey.Modifier;
            Orphaned = hotkeyAssociation.Orphaned;
        }

        public static NHkey.Model.HotkeyAssociation GetHotkey(HotkeyData data)
        {
            var hotkey = new NHotkeyAPI.Hotkey(data.Key, data.Modifiers);

            var hotkeyAssociation = new HotkeyAssociation
                (hotkey, data.Name, data.FilePath, data.Parameters);

            return hotkeyAssociation;
        }

        public static HotkeyData GetData(HotkeyAssociation hotkey)
        {
            return new HotkeyData(hotkey);
        }

    }
}
