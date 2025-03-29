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
        public string FirstRating { get; set; }
        public string SecondRating { get; set; }
        public string ThirdRating { get; set; }
        public string FirstBackupRating { get; set; }
        public string SecondBackupRating { get; set; }
        public string ThirdBackupRating { get; set; }
        public string Video3D { get; set; }
        public string VideoQuality { get; set; }
        public string ColorRange { get; set; }
        public string AudioChannels { get; set; }
        public string VideoCodec { get; set; }
        public string BadgeSize { get; set; }
        public string BadgePos { get; set; }
        public string BackdropBadgePos { get; set; }
        public string AgeRating { get; set; }
        public string PosterSize { get; set; }
        public string PosterBarPos { get; set; }
        public string PosterBarOpacity { get; set; }
        public string PosterBarColor { get; set; }
        public string PosterFontColor { get; set; }
        public string BackdropSize { get; set; }
        public string BackdropBarPos { get; set; }
        public string BackdropBarOpacity { get; set; }
        public string BackdropBarColor { get; set; }
        public string BackdropFontColor { get; set; }
        public string BackdropCertsPos { get; set; }
    }
}
