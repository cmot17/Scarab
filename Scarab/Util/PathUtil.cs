using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;

namespace Scarab.Util
{
    public static class PathUtil
    {
        public class PathInvalidOrUnselectedException : Exception {}
        
        private const string NO_SELECT = "No path was selected!";
        private const string NO_SELECT_MAC = "No application was selected!";
        
        private const string INVALID_PATH = "Invalid Hollow Knight path!\nSelect the folder containing hollow_knight_Data or Hollow Knight_Data.";
        private const string INVALID_APP = "Invalid Hollow Knight app!\nMissing Managed folder or Assembly-CSharp!";
        
        public static async Task<string> SelectPath([DoesNotReturnIf(true)] bool fail = false)
        {
            Debug.WriteLine("Selecting path...");

            Window parent = (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow
                ?? throw new InvalidOperationException();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return await SelectMacApp(parent, fail);

            var dialog = new OpenFolderDialog
            {
                Title = "Select your Hollow Knight folder."
            };

            while (true)
            {
                string? result = await dialog.ShowAsync(parent);

                if (result is null)
                    await MessageBoxManager.GetMessageBoxStandardWindow("Path", NO_SELECT).Show();
                else if (ValidateWithSuffix(result) is not (var managed, var suffix))
                    await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams {
                        ContentTitle = "Path",
                        ContentMessage = INVALID_PATH,
                        MinHeight = 140
                    }).Show();
                else
                    return Path.Combine(managed, suffix);

                if (fail)
                    throw new PathInvalidOrUnselectedException();
            }
        }

        private static async Task<string> SelectMacApp(Window parent, [DoesNotReturnIf(true)] bool fail)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select your Hollow Knight app.",
                AllowMultiple = false
            };

            dialog.Filters.Add(new FileDialogFilter { Extensions = { "app" } });

            while (true)
            {
                string[]? result = await dialog.ShowAsync(parent);

                if (result is null or { Length: 0 })
                    await MessageBoxManager.GetMessageBoxStandardWindow("Path", NO_SELECT_MAC).Show();
                else if (ValidateWithSuffix(result.First()) is not (var managed, var suffix))
                    await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams {
                        ContentTitle = "Path",
                        ContentMessage = INVALID_APP,
                        MinHeight = 140
                    }).Show();
                else
                    return Path.Combine(managed, suffix);

                if (fail)
                    throw new PathInvalidOrUnselectedException();
            }
        }

        private static readonly string[] SUFFIXES =
        {
            // Steam
            "hollow_knight_Data/Managed",
            // GoG
            "Hollow Knight_Data/Managed",
            // Mac
            "Contents/Resources/Data/Managed"
        };

        public static ValidPath? ValidateWithSuffix(string root)
        {
            if (!Directory.Exists(root))
                return null;
            
            string? suffix = SUFFIXES.FirstOrDefault(s => Directory.Exists(Path.Combine(root, s)));

            if (suffix is null || !File.Exists(Path.Combine(root, suffix, "Assembly-CSharp.dll")))
                return null;

            return new ValidPath(root, suffix);
        }
    }
}