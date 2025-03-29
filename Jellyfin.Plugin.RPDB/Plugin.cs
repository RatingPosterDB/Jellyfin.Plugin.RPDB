using System;
using System.Collections.Generic;
using Jellyfin.Plugin.RPDB.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.RPDB
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private readonly Guid _id = new Guid("72186ab4-3fce-4054-9207-bcdd18c9fc45");
        public const string BaseUrl = "http://api.ratingposterdb.com/{0}/{1}/{2}/{3}.jpg{4}";

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        /// <inheritdoc />
        public override string Name => "RatingPosterDB";

        /// <inheritdoc />
        public override Guid Id => _id;

        public static Plugin Instance { get; private set; }

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.Configuration.configPage.html", GetType().Namespace)
                }
            };
        }
    }
}
