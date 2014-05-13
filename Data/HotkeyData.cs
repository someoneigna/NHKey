using NHkey.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

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

        public HotkeyData()
        {
        }

        public HotkeyData(string name, string filepath, string parameters, int key, int modifiers)
        {
            Name = name;
            FilePath = filepath;
            Parameters = parameters;
            Key = key;
            Modifiers = modifiers;
        }

        public static NHkey.Model.HotkeyAssociation GetHotkey(HotkeyData data)
        {
            return NHkey.Model.HotkeyAssociationFactory.MakeHotkeyAssociation(data.Name, data.FilePath, data.Key, data.Modifiers, IntPtr.Zero, data.Parameters);
        }

        public static HotkeyData GetData(HotkeyAssociation hotkey)
        {
            return new HotkeyData(hotkey.Name, hotkey.FilePath, hotkey.Parameters, hotkey.Hotkey.Key, hotkey.Hotkey.Modifier); 
        }

    }
}
