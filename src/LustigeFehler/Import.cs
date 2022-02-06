// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Import.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The import class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LustigeFehler
{
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// The import class.
    /// </summary>
    public static class Import
    {
        /// <summary>
        /// Loads the configuration from a XML file.
        /// </summary>
        /// <param name="fileName">The file name.</param>
        /// <returns>The read <see cref="Config"/>.</returns>
        public static Config? LoadConfigFromXmlFile(string fileName)
        {
            var xDocument = XDocument.Load(fileName);
            return CreateObjectsFromString<Config?>(xDocument);
        }

        /// <summary>
        /// Creates the object from a <see cref="string"/>.
        /// </summary>
        /// <typeparam name="T">The type parameter.</typeparam>
        /// <param name="xDocument">The X document.</param>
        /// <returns>A new object as <see cref="T"/>.</returns>
        private static T? CreateObjectsFromString<T>(XDocument xDocument)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T?)xmlSerializer.Deserialize(new StringReader(xDocument.ToString()));
        }
    }
}