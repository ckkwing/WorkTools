﻿using Google.Apis.Json;
using Google.Apis.Util.Store;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Algorithms;

namespace WorkTools.Modules.Windows
{
    /// <summary>
    /// Encrypted file data store that implements <see cref="IDataStore"/>. This store creates a different file for each 
    /// combination of type and key. This file data store stores a JSON format of the specified object.
    /// </summary>
    internal class EncryptedFileDataStore : IDataStore
    {
        private const string XdgDataHomeSubdirectory = "google-filedatastore";
        private static readonly Task CompletedTask = Task.FromResult(0);

        readonly string folderPath;
        /// <summary>Gets the full folder path.</summary>
        public string FolderPath { get { return folderPath; } }

        /// <summary>
        /// Constructs a new file data store. If <c>fullPath</c> is <c>false</c> the path will be used as relative to 
        /// <c>Environment.SpecialFolder.ApplicationData"</c> on Windows, or <c>$HOME</c> on Linux and MacOS,
        /// otherwise the input folder will be treated as absolute.
        /// The folder is created if it doesn't exist yet.
        /// </summary>
        /// <param name="folder">Folder path.</param>
        /// <param name="fullPath">
        /// Defines whether the folder parameter is absolute or relative to
        /// <c>Environment.SpecialFolder.ApplicationData</c> on Windows, or<c>$HOME</c> on Linux and MacOS.
        /// </param>
        public EncryptedFileDataStore(string folder, bool fullPath = false)
        {
            folderPath = fullPath
                ? folder
                : Path.Combine(GetHomeDirectory(), folder);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        private string GetHomeDirectory()
        {
            string appData = Environment.GetEnvironmentVariable("APPDATA");
            if (!string.IsNullOrEmpty(appData))
            {
                // This is almost certainly windows.
                // This path must be the same between the desktop FileDataStore and this netstandard FileDataStore.
                return appData;
            }
            string home = Environment.GetEnvironmentVariable("HOME");
            if (!string.IsNullOrEmpty(home))
            {
                // This is almost certainly Linux or MacOS.
                // Follow the XDG Base Directory Specification: https://specifications.freedesktop.org/basedir-spec/latest/index.html
                // Store data in subdirectory of $XDG_DATA_HOME if it exists, defaulting to $HOME/.local/share if not set.
                string xdgDataHome = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
                if (string.IsNullOrEmpty(xdgDataHome))
                {
                    xdgDataHome = Path.Combine(home, ".local", "share");
                }
                return Path.Combine(xdgDataHome, XdgDataHomeSubdirectory);
            }
            throw new PlatformNotSupportedException("Relative FileDataStore paths not supported on this platform.");
        }

        /// <summary>Creates a unique stored key based on the key and the class type.</summary>
        /// <param name="key">The object key.</param>
        /// <param name="t">The type to store or retrieve.</param>
        public static string GenerateStoredKey(string key, Type t)
        {
            return string.Format("{0}-{1}", t.FullName, key);
        }

        /// <summary>
        /// Clears all values in the data store. This method deletes all files in <see cref="FolderPath"/>.
        /// </summary>
        public Task ClearAsync()
        {
            if (Directory.Exists(folderPath))
            {
                Directory.Delete(folderPath, true);
                Directory.CreateDirectory(folderPath);
            }

            return CompletedTask;
        }

        /// <summary>
        /// Deletes the given key. It deletes the <see cref="GenerateStoredKey"/> named file in 
        /// <see cref="FolderPath"/>.
        /// </summary>
        /// <param name="key">The key to delete from the data store.</param>
        public Task DeleteAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return CompletedTask;
        }

        /// <summary>
        /// Returns the stored value for the given key or <c>null</c> if the matching file (<see cref="GenerateStoredKey"/>
        /// in <see cref="FolderPath"/> doesn't exist.
        /// </summary>
        /// <typeparam name="T">The type to retrieve.</typeparam>
        /// <param name="key">The key to retrieve from the data store.</param>
        /// <returns>The stored object.</returns>
        public Task<T> GetAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            if (File.Exists(filePath))
            {
                try
                {
                    var base64Content = File.ReadAllText(filePath);
                    var serialized = Base64Helper.Decode(base64Content);
                    tcs.SetResult(NewtonsoftJsonSerializer.Instance.Deserialize<T>(serialized));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }
            else
            {
                tcs.SetResult(default(T));
            }
            return tcs.Task;
        }

        /// <summary>
        /// Stores the given value for the given key. It creates a new file (named <see cref="GenerateStoredKey"/>) in 
        /// <see cref="FolderPath"/>.
        /// </summary>
        /// <typeparam name="T">The type to store in the data store.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to store in the data store.</param>
        public Task StoreAsync<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key MUST have a value");
            }

            var serialized = NewtonsoftJsonSerializer.Instance.Serialize(value);
            var base64Content = Base64Helper.Encode(serialized);
            var filePath = Path.Combine(folderPath, GenerateStoredKey(key, typeof(T)));
            File.WriteAllText(filePath, base64Content);
            return CompletedTask;
        }
    }
}
