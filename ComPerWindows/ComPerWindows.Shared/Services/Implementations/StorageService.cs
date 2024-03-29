﻿using ComPerWindows.Services.Interfaces;

namespace ComPerWindows.Services.Implementations
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Windows.Storage;
    using Newtonsoft.Json.Linq;

    public class StorageService : IStorageService
    {
        private readonly StorageFolder _localFolder;

        public StorageService()
        {
            _localFolder = ApplicationData.Current.LocalFolder;
        }

        public async Task CreateOrUpdateData(string key, object o)
        {
            try
            {
                if (o != null)
                {
                    var sessionFile = await _localFolder.CreateFileAsync(key, CreationCollisionOption.ReplaceExisting);
                    var outputString = JToken.FromObject(o).ToString();
                    await FileIO.WriteTextAsync(sessionFile, outputString);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Encountered exception: {0}", e);
            }
        }

        public async Task<T> GetDataOrDefault<T>(string key, T defaultValue)
        {
            try
            {
                T results = defaultValue;

                var sessionFile = await _localFolder.CreateFileAsync(key, CreationCollisionOption.OpenIfExists);
                var data = await FileIO.ReadTextAsync(sessionFile);
                
                if (!String.IsNullOrWhiteSpace(data))
                {
                    results = JToken.Parse(data).ToObject<T>();
                }

                return results;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Encountered exception: {0}", e);
            }

            return defaultValue;
        }

        public async Task DeleteData(string key)
        {
            try
            {
                var file = await _localFolder.GetFileAsync(key);
                await file.DeleteAsync();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Encountered exception: {0}", e);
            }
        }
    }
}
