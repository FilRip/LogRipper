using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Serialization;

namespace LogRipper.Models
{
    [XmlRoot()]
    public class SavedRules
    {
        [XmlElement()]
        public string Title { get; set; }

        [XmlElement()]
        public List<OneRule> ListRules { get; set; }

        public void Save(string filename)
        {
            XmlSerializer serializer = new(typeof(SavedRules));
            if (File.Exists(filename))
                File.Delete(filename);
            FileStream fs = new(filename, FileMode.CreateNew, FileAccess.Write);
            serializer.Serialize(fs, this);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }

        public static void SaveFile(string filename, string title, ObservableCollection<OneRule> newListRules)
        {
            SavedRules sr = new()
            {
                Title = title,
                ListRules = new List<OneRule>()
            };
            sr.ListRules.AddRange(newListRules);
            sr.Save(filename);
        }

        public static SavedRules LoadFile(string filename)
        {
            if (!File.Exists(filename))
                return null;
            FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read);
            XmlSerializer serializer = new(typeof(SavedRules));
            try
            {
                object stream = serializer.Deserialize(fs);
                return (SavedRules)stream;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                fs?.Close();
                fs?.Dispose();
            }
        }
    }
}
