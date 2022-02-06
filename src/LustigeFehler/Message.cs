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
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption { get; set; } = string.Empty;
    }
}