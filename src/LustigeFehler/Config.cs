// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LustigeFehler
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The configuration class.
    /// </summary>
    [Serializable]
    public class Config
    {
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        // ReSharper disable once CollectionNeverUpdated.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public List<Message> Messages { get; set; }
    }
}