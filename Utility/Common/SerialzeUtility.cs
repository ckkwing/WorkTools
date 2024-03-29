﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public class SerialzeUtility
    {
        /// <summary>
        /// 把对象序列化为字节数组
        /// </summary>
        public static byte[] SerializeObjectToBytes(object obj)
        {
            if (obj == null)
                return null;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            byte[] bytes = ms.ToArray();
            return bytes;
        }

        /// <summary>
        /// 把字节数组反序列化成对象
        /// </summary>
        public static object DeserializeObjectFromBytes(byte[] bytes)
        {
            object obj = null;
            if (bytes == null)
                return obj;
            MemoryStream ms = new MemoryStream(bytes)
            {
                Position = 0
            };
            BinaryFormatter formatter = new BinaryFormatter();
            obj = formatter.Deserialize(ms);
            ms.Close();
            return obj;
        }

        /// <summary>
        /// 把对象序列化为文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public static void SerializeObjectToFile(string fileName, object obj)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
            }
        }

        /// <summary>
        /// 把文件反序列化成对象
        /// </summary>
        public static object DeserializeObjectFromFile(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                object obj = formatter.Deserialize(fs);
                return obj;
            }
        }

        /// <summary>
        /// 把对象序列化到文件(AES加密)
        /// </summary>
        /// <param name="keyString">密钥(16位)</param>
        public static void SerializeObjectToFile(string fileName, object obj, string key, string iv)
        {
            using (AesCryptoServiceProvider crypto = new AesCryptoServiceProvider())
            {
                crypto.Key = Encoding.ASCII.GetBytes(key);
                crypto.IV = Encoding.ASCII.GetBytes(iv);
                using (ICryptoTransform transform = crypto.CreateEncryptor())
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        using (CryptoStream cs = new CryptoStream(fs, transform, CryptoStreamMode.Write))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(cs, obj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 把文件反序列化成对象(AES加密)
        /// </summary>
        /// <param name="keyString">密钥(16位)</param>
        public static object DeserializeObjectFromFile(string fileName, string key, string iv)
        {
            using (AesCryptoServiceProvider crypto = new AesCryptoServiceProvider())
            {
                crypto.Key = Encoding.ASCII.GetBytes(key);
                crypto.IV = Encoding.ASCII.GetBytes(iv);
                using (ICryptoTransform transform = crypto.CreateDecryptor())
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (CryptoStream cs = new CryptoStream(fs, transform, CryptoStreamMode.Read))
                        {
                            BinaryFormatter formatter = new BinaryFormatter();
                            object obj = formatter.Deserialize(cs);
                            return obj;
                        }
                    }
                }
            }
        }
    }
}
