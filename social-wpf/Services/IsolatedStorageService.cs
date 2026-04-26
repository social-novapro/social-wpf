using social_wpf.Models;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace social_wpf.Services
{
    public class IsolatedStorageService
    {
        private const string SettingsFileName = "appsettings.json";

        public AppSettings LoadSettings()
        {
            using IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();

            if (!storage.FileExists(SettingsFileName))
            {
                return new AppSettings();
            }

            using IsolatedStorageFileStream stream = new IsolatedStorageFileStream(
                SettingsFileName, 
                System.IO.FileMode.Open,
                storage
            );

            AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(stream);

            return settings ?? new AppSettings();
        }

        public void SaveSettings(AppSettings settings)
        {
            using IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();
            using IsolatedStorageFileStream stream = new IsolatedStorageFileStream(
                SettingsFileName,
                System.IO.FileMode.Create,
                storage
            );
            JsonSerializer.Serialize(stream, settings);
        }

        public void ClearSettings()
        {
            using IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForAssembly();

            if (storage.FileExists(SettingsFileName))
            {
                storage.DeleteFile(SettingsFileName);
            }
        }
    }
}
