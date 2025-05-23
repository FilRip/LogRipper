﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LogRipper.Helpers
{
    internal static class ReloadOlderConfig
    {
        internal static void SearchAndReload()
        {
            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company);
            if (!Directory.Exists(appData))
                Directory.CreateDirectory(appData);
            Version mostPrevious = new("1.0.0.0");
            string lastRep = null;
            foreach (string subFolder in Directory.GetDirectories(appData, $"{Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product}*.*"))
            {
                foreach (string versionFolder in Directory.GetDirectories(subFolder))
                {
                    Version oldVersion = new(Path.GetFileName(versionFolder));
                    if (oldVersion.CompareTo(Assembly.GetEntryAssembly().GetName().Version) < 0 && oldVersion.CompareTo(mostPrevious) > 0)
                    {
                        lastRep = versionFolder;
                        mostPrevious = oldVersion;
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(lastRep))
            {
                PropertyInfo[] listProperties = [.. Properties.Settings.Default.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Name.ToLower().Contains("size"))];
                string[] listLines = File.ReadAllLines(Path.Combine(lastRep, "user.config"));
                for (int i = 0; i < listLines.Length; i++)
                {
                    string line = listLines[i].Trim();
                    if (line.StartsWith("<setting "))
                    {
                        string name = line.Substring(line.IndexOf('"') + 1, line.IndexOf('"', line.IndexOf('"') + 1) - line.IndexOf('"') - 1);
#pragma warning disable S127 // "for" loop stop conditions should be invariant
                        i++;
#pragma warning restore S127 // "for" loop stop conditions should be invariant
                        string value = listLines[i].Trim().Replace("<value>", "").Replace("</value>", "");
                        PropertyInfo pi = Array.Find(listProperties, p => p.Name == name);
                        if (pi != null)
                        {
                            try
                            {
                                object newValue;
                                if (pi.PropertyType == typeof(System.Drawing.Color))
                                {
                                    string[] splitter = value.Split(',');
                                    newValue = System.Drawing.Color.FromArgb(int.Parse(splitter[0]), int.Parse(splitter[1]), int.Parse(splitter[2]));
                                }
                                else
                                    newValue = Convert.ChangeType(value, pi.PropertyType);
                                pi.SetValue(Properties.Settings.Default, newValue);
                            }
                            catch (Exception) { /* Ignore errors */ }
                        }
                    }
                }
            }
        }
    }
}
