﻿#region copyright
//  Copyright (C) 2022 Auto Dark Mode
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion
using AutoDarkModeLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdmProperties = AutoDarkModeLib.Properties;


namespace AutoDarkModeApp.Handlers
{
    public static class ThemeCollectionHandler
    {
        public static readonly string ThemeFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Themes";

        //get a list of all files the theme folder contains. If there is no theme-folder, create one.
        public static List<ThemeFile> GetUserThemes()
        {
            try
            {
                List<string> files = Directory.EnumerateFiles(ThemeFolderPath, "*.*", SearchOption.AllDirectories).ToList();
                files = files.Where(f => f.EndsWith(".theme") 
                    && !f.Contains(Helper.PathUnmanagedDarkTheme) 
                    && !f.Contains(Helper.NameUnmanagedLightTheme) 
                    && !f.Contains(Helper.PathManagedTheme))
                .ToList();
                List<ThemeFile> themeFiles = files.Select(f => new ThemeFile(f)).ToList();
                InjectWindowsThemes(themeFiles);
                return themeFiles;
            }
            catch
            {
                Directory.CreateDirectory(ThemeFolderPath);
                return GetUserThemes();
            }
        }

        private static void InjectWindowsThemes(List<ThemeFile> themeFiles)
        {
            if (Environment.OSVersion.Version.Build >= (int)WindowsBuilds.Win11_RC)
            {
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\aero.theme", AdmProperties.Resources.ThemePickerTheme11Light));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\dark.theme", AdmProperties.Resources.ThemePickerTheme11Dark));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\themeA.theme", AdmProperties.Resources.ThemePickerTheme11Glow));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\themeB.theme", AdmProperties.Resources.ThemePickerTheme11CapturedMotion));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\themeC.theme", AdmProperties.Resources.ThemePickerTheme11Sunrise));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\themeD.theme", AdmProperties.Resources.ThemePickerTheme11Flow));

            }
            else
            {
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\aero.theme", AdmProperties.Resources.ThemePickerTheme10Windows));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\Light.theme", AdmProperties.Resources.ThemePickerTheme10WindowsLight));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\theme1.theme", AdmProperties.Resources.ThemePickerTheme10Windows10));
                themeFiles.Add(new ThemeFile(@"C:\Windows\Resources\Themes\theme2.theme", AdmProperties.Resources.ThemePickerTheme10Flowers));
            }
        }
    }

    public class ThemeFile
    {
        public ThemeFile(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(path) ?? "Undefined";
        }
        public ThemeFile(string path, string name) : this(path)
        {
            Name = name;
            IsWindowsTheme = true;
        }

        public string Path { get; }
        public string Name { get; }
        public bool IsWindowsTheme { get; }
        public override string ToString()
        {
            return Name;
        }
        public override bool Equals(object obj)
        {
            return obj is string name ? Name.Equals(name, StringComparison.Ordinal) : base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
