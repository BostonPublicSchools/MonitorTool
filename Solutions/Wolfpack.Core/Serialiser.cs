using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using JsonSerializer = ServiceStack.Text.JsonSerializer;

namespace Wolfpack.Core
{
    public class Serialiser
    {
        public static string ToJson<T>(T entity, bool format = true)
        {
            return format 
                // pretty or...
                ? JsonConvert.SerializeObject(entity, Formatting.Indented) 
                // fast!
                : JsonSerializer.SerializeToString(entity);
        }

        public static void ToJsonInFile<T>(string path, T entity, bool format = true)
        {
            var folder = Path.GetDirectoryName(path);

            if (folder != null)
                Directory.CreateDirectory(folder);

            using (var sw = new StreamWriter(path))
                sw.WriteLine(ToJson(entity, format));
        }

        public static T FromJson<T>(string data)
        {
            return JsonSerializer.DeserializeFromString<T>(data);
        }

        public static object FromJson(string data, Type type)
        {
            return JsonSerializer.DeserializeFromString(data, type);
        }

        public static T FromJsonInFile<T>(string path)
        {
            string data;
            using (var stream = File.OpenRead(path))
            using (var sr = new StreamReader(stream))
            {
                data = sr.ReadToEnd();                
            }

            return JsonSerializer.DeserializeFromString<T>(data);
        }

        /// <summary>
        /// Serialize the object using the DataContractSerializer
        /// </summary>
        /// <param name="encoding">The encoding to use when serializing.</param>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public static string ToXml<T>(Encoding encoding, T entity)
        {
            var serializer = new DataContractSerializer(typeof(T));

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, entity);
                return encoding.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Serialize the object using the DataContractSerializer to the file specified. If the file
        /// exists it will be overwritten and it requires an exclusive write lock on the file.
        /// </summary>
        /// <param name="path">The filename to write the serialization string to</param>
        /// <param name="encoding">The encoding to use when serializing.</param>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public static void ToXmlInFile<T>(string path, Encoding encoding, T entity)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var filename = SmartLocation.GetLocation(path);

            using (var writer = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.WriteObject(writer, entity);
            }
        }

        /// <summary>
        /// Serialize the object using the DataContractSerializer to the file specified with UTF8
        /// encoding. If the file exists it will be overwritten and it requires an exclusive write
        /// lock on the file.
        /// </summary>
        /// <param name="path">The filename to write the serialization string to</param>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public static void ToXmlInFile<T>(string path, T entity)
        {
            ToXmlInFile(path, Encoding.UTF8, entity);
        }

        /// <summary>
        /// Serialize the object using the DataContractSerializer, defaults to using UTF8 encoding
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns></returns>
        public static string ToXml<T>(T entity)
        {
            return ToXml(Encoding.UTF8, entity);
        }

        /// <summary>
        /// Deserialize the object using the DataContractSerializer
        /// </summary>
        /// <param name="encoding">The encoding to use when deserializing.</param>
        /// <param name="entity">The entity to deserialize.</param>
        /// <returns></returns>
        public static T FromXml<T>(Encoding encoding, string entity)
        {
            var serializer = new DataContractSerializer(typeof(T));
            
            using (var ms = new MemoryStream(encoding.GetBytes(entity)))
            {
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Deserialize the object using the DataContractSerializer, defaults to using UTF8 encoding
        /// </summary>
        /// <param name="entity">The entity to deserialize.</param>
        /// <returns></returns>
        public static T FromXml<T>(string entity)
        {
            return FromXml<T>(Encoding.UTF8, entity);
        }

        /// <summary>
        /// Deserialize the object using the DataContractSerializer from the file specified. If the file
        /// does not exist then an exception will be thrown
        /// </summary>
        /// <param name="path">The fully qualified path to the file</param>
        /// <returns></returns>
        public static T FromXmlInFile<T>(string path)
        {
            return FromXmlInFile<T>(path, Encoding.UTF8);
        }

        /// <summary>
        /// Deserialize the object using the DataContractSerializer from the file specified. If the file
        /// does not exist then an exception will be thrown
        /// </summary>
        /// <param name="path">The fully qualified path to the file</param>
        /// <param name="encoding">The encoding to use when deserializing.</param>
        /// <returns></returns>
        public static T FromXmlInFile<T>(string path, Encoding encoding)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var filename = SmartLocation.GetLocation(path);

            using (var reader = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return (T)serializer.ReadObject(reader);
            }
        }
    }
}