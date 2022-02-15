// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Config.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The configuration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LustigeFehler;

/// <summary>
/// The configuration class.
/// </summary>
[Serializable]
public class Config
{
    /// <summary>
    /// Gets or sets the messages.
    /// </summary>
    public List<Message> Messages { get; set; } = new();
}
