// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Message.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The message class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LustigeFehler
{
    using System;

    /// <summary>
    /// The message class.
    /// </summary>
    [Serializable]
    public class Message
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Caption { get; set; }
    }
}