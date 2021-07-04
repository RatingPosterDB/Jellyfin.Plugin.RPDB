using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.RPDB.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string UserApiKey { get; set; }
        public string PosterType { get; set; }
        public string PosterLang { get; set; }
        public string Textless { get; set; }
        public string Backdrops { get; set; }
    }
}
