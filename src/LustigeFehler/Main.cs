// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Main.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LustigeFehler
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class Main : Form
    {
        /// <summary>
        /// The random generator.
        /// </summary>
        private readonly Random random = new();

        /// <summary>
        /// The configuration.
        /// </summary>
        private Config config = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Main"/> class.
        /// </summary>
        public Main()
        {
            this.InitializeComponent();
            this.Configure();
        }

        /// <summary>
        /// Sets the visible core.
        /// </summary>
        /// <param name="value">The value.</param>
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        /// <summary>
        /// Configures the data.
        /// </summary>
        private void Configure()
        {
            this.LoadConfig();
            this.SpamUser();
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        private void LoadConfig()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                this.config = Import.LoadConfigFromXmlFile(Path.Combine(Directory.GetParent(location)?.FullName ?? string.Empty, "Config.xml")) ?? new();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Spams the user.
        /// </summary>
        private void SpamUser()
        {
            while (true)
            {
                try
                {
                    var message = this.GetRandomMessage();
                    var buttons = this.GetRandomButtons();
                    var icons = this.GetRandomIcon();

                    if (buttons is not null && icons is not null)
                    {
                        MessageBox.Show(message.Name, message.Caption, buttons.Value, icons.Value);
                    }
                    
                    Thread.Sleep(this.random.Next(2300));
                }
                catch
                {
                    // ignored
                }
            }
        }

        /// <summary>
        /// Gets a random message.
        /// </summary>
        /// <returns>A new random <see cref="Message"/>.</returns>
        private Message GetRandomMessage()
        {
            return this.config.Messages.ElementAt(this.random.Next(this.config.Messages.Count));
        }

        /// <summary>
        /// Gets the random buttons.
        /// </summary>
        /// <returns>The <see cref="MessageBoxButtons"/>.</returns>
        private MessageBoxButtons? GetRandomButtons()
        {
            var values = Enum.GetValues(typeof(MessageBoxButtons));
            var val = values.GetValue(this.random.Next(values.Length));

            if (val is null)
            {
                return null;
            }

            return (MessageBoxButtons)val;
        }

        /// <summary>
        /// Gets the random icon.
        /// </summary>
        /// <returns>The <see cref="MessageBoxIcon"/>.</returns>
        private MessageBoxIcon? GetRandomIcon()
        {
            var values = Enum.GetValues(typeof(MessageBoxIcon));
            var val = values.GetValue(this.random.Next(values.Length));

            if (val is null)
            {
                return null;
            }

            return (MessageBoxIcon)val;
        }
    }
}