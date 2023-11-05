using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

using LogRipper.Constants;
using LogRipper.Exceptions;
using LogRipper.Helpers;
using LogRipper.Windows;

using Microsoft.Win32;

namespace LogRipper.Models
{
    [XmlRoot()]
    public class OneState
    {
        public OneState() { }

        [XmlElement()]
        public List<OneRule> ListRules { get; set; }

        [XmlElement()]
        public List<OneFile> ListFiles { get; set; }

        [XmlElement()]
        public List<OneLine> ListLines { get; set; }

        [XmlElement()]
        public List<OneCategory> ListCategory { get; set; }

        [XmlElement()]
        public int CurrentNumLine { get; set; }

        [XmlElement()]
        public DateTime? StartDate { get; set; }

        [XmlElement()]
        public DateTime? EndDate { get; set; }

        [XmlElement()]
        public bool FilterByDate { get; set; }

        [XmlElement()]
        public bool HideAllOthersLines { get; set; }

        internal static void SaveCurrentState()
        {
            SaveFileDialog dialog = new()
            {
                Filter = "State file|*.state|Zip file|*.zip|All files|*.*",
            };
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;
                MainWindow win = Application.Current.GetCurrentWindow<MainWindow>();
                OneState state = new()
                {
                    ListRules = win.MyDataContext.ListRules.ListRules.ToList(),
                    ListFiles = FileManager.GetAllFiles(),
                    ListLines = win.MyDataContext.ListLines.ToList(),
                    ListCategory = win.MyDataContext.ListCategory,
                    CurrentNumLine = win.MyDataContext.NumStartVisibleLine,
                    FilterByDate = win.MyDataContext.FilterByDate,
                    StartDate = win.MyDataContext.StartDateTime,
                    EndDate = win.MyDataContext.EndDateTime,
                    HideAllOthersLines = win.MyDataContext.HideAllOthersLines,
                };
                XmlSerializer serializer = new(typeof(OneState));
                if (File.Exists(filename))
                    File.Delete(filename);
                if (Path.GetExtension(filename).Trim().ToLower() == ".state")
                {
                    FileStream fs = new(filename, FileMode.CreateNew, FileAccess.Write);
                    serializer.Serialize(fs, state);
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }
                else
                {
                    MemoryStream ms = new();
                    serializer.Serialize(ms, state);
                    if (!Compression.CompressData(ms, filename, 1024, true))
                        throw new LogRipperException("Error during compression");
                    ms.Dispose();
                }
            }
        }

        internal static void LoadNewState()
        {
            try
            {
                OpenFileDialog dialog = new()
                {
                    Filter = "State file|*.state|Zip file|*.zip|All files|*.*",
                };
                if (dialog.ShowDialog() == true)
                {
                    string filename = dialog.FileName;
                    MainWindow win = Application.Current.GetCurrentWindow<MainWindow>();
                    OneState newState;
                    XmlReaderSettings settings = new()
                    {
                        CheckCharacters = false,
                    };
                    XmlSerializer serializer = new(typeof(OneState));
                    XmlReader stream;
                    Stream dataOut;
                    if (Path.GetExtension(dialog.FileName).Trim().ToLower() == ".state")
                    {
                        dataOut = new FileStream(filename, FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        Compression.DecompressData(filename, out MemoryStream data, 1024);
                        dataOut = data;
                    }
                    stream = XmlReader.Create(dataOut, settings);
                    newState = (OneState)serializer.Deserialize(stream);
                    FileManager.SetNewListFiles(newState.ListFiles);
                    win.MyDataContext.SelectedLine = null;
                    if (newState.ListLines != null)
                    {
                        win.MyDataContext.ListLines = new ObservableCollection<OneLine>(newState.ListLines);
                    }
                    if (newState.ListRules != null)
                    {
                        win.MyDataContext.ListRules.SetRules(new ObservableCollection<OneRule>(newState.ListRules));
                        win.MyDataContext.ListCategory = newState.ListCategory;
                        win.MyDataContext.UpdateCategory();
                    }
                    win.MyDataContext.FilterByDate = newState.FilterByDate;
                    win.MyDataContext.StartDateTime = newState.StartDate;
                    win.MyDataContext.EndDateTime = newState.EndDate;
                    win.MyDataContext.HideAllOthersLines = newState.HideAllOthersLines;
                    win.MyDataContext.RefreshVisibleLines();
                    win.ListBoxLines.SelectedIndex = newState.CurrentNumLine;
                    win.ScrollToSelected();
                    stream.Close();
                    stream.Dispose();
                    dataOut.Close();
                    dataOut.Dispose();
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.ShowModal(Locale.ERROR_READING_FILE + Environment.NewLine + ex.Message, Locale.TITLE_ERROR, MessageBoxButton.OK);
            }
        }
    }
}
