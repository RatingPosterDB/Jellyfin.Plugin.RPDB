using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Data.Enums;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.RPDB.Providers
{
    public class SeriesProvider : IRemoteImageProvider, IHasOrder
    {
        private readonly IServerConfigurationManager _config;
        private readonly IHttpClientFactory _httpClientFactory;
        public const string BaseTmdbId = "series-{0}";

        public SeriesProvider(IServerConfigurationManager config, IHttpClientFactory httpClientFactory, IFileSystem fileSystem)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;

            Current = this;
        }

        internal static SeriesProvider Current { get; private set; }

        /// <inheritdoc />
        public string Name => "RatingPosterDB";

        /// <inheritdoc />
        public int Order => 1;

        /// <inheritdoc />
        public bool Supports(BaseItem item)
            => item is Series;

        /// <inheritdoc />
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item)
        {
            return new List<ImageType>
            {
                ImageType.Primary,
                ImageType.Backdrop
            };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken)
        {
            var list = new List<RemoteImageInfo>();

            var series = (Series)item;

            var idType = "imdb";
            var seriesId = series.GetProviderId(MetadataProvider.Imdb);

            if (string.IsNullOrEmpty(seriesId))
            {
                idType = "tmdb";
                seriesId = string.Format(BaseTmdbId, series.GetProviderId(MetadataProvider.Tmdb));
            }

            if (string.IsNullOrEmpty(seriesId))
            {
                idType = "tvdb";
                seriesId = series.GetProviderId(MetadataProvider.Tvdb);
            }

            if (!string.IsNullOrEmpty(seriesId))
            {
                try
                {
                    await AddImages(item, list, idType, seriesId, cancellationToken).ConfigureAwait(false);
                }
                catch (FileNotFoundException)
                {
                    // No biggie. Don't blow up
                }
                catch (IOException)
                {
                    // No biggie. Don't blow up
                }
            }

            return list;
        }

        private async Task AddImages(BaseItem item, List<RemoteImageInfo> list, string idType, string seriesId, CancellationToken cancellationToken)
        {
            await Task.Run(() => PopulateImages(item, list, "poster", idType, seriesId, ImageType.Primary));
            await Task.Run(() => PopulateImages(item, list, "backdrop", idType, seriesId, ImageType.Backdrop));
        }

        private void PopulateImages(BaseItem item, List<RemoteImageInfo> list, string reqType, string idType, string seriesId, ImageType type)
        {

            var clientKey = Plugin.Instance.Configuration.UserApiKey;

            if (string.IsNullOrWhiteSpace(clientKey))
            {
                return;
            }

            var posterType = "poster-default";
            var fallback = "";
            var posterLang = Plugin.Instance.Configuration.PosterLang;

            var width = 0;
            var height = 0;

            if (reqType.Equals("backdrop"))
            {
                width = 1920;
                height = 1080;
                var backdrops = Plugin.Instance.Configuration.Backdrops;
                if (backdrops.Equals("1"))
                {
                    if (clientKey.StartsWith("t0-") || clientKey.StartsWith("t1-"))
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
                posterType = "backdrop-default";
            }
            else if (reqType.Equals("poster"))
            {
                width = 580;
                height = 859;
                fallback = "&fallback=true";
                if (!clientKey.StartsWith("t0-")) {
                    posterType = Plugin.Instance.Configuration.PosterType;
                }
                var textless = Plugin.Instance.Configuration.Textless;
                if (!posterLang.Equals("en") && !textless.Equals("1") && !clientKey.StartsWith("t0-"))
                {
                    fallback += "&lang=";
                    fallback += posterLang;
                }
                if (textless.Equals("1") && !clientKey.StartsWith("t0-"))
                {
                    if (posterType.StartsWith("poster-")) {
                        posterType = posterType.Replace("poster-", "textless-");
                    }
                    else if (posterType.StartsWith("rating-")) {
                        posterType = posterType.Replace("rating-", "textless-");
                    }
                }
            }
            if ((posterType.Equals("rating-order") || posterType.Equals("textless-order") || posterType.Equals("backdrop-default")) && !clientKey.StartsWith("t0-") && !clientKey.StartsWith("t1-"))
            {
                fallback += "&order=";
                var firstRating = Plugin.Instance.Configuration.FirstRating; 
                fallback += firstRating;
                var secondRating = Plugin.Instance.Configuration.SecondRating; 
                if (!secondRating.Equals("none"))
                {
                    fallback += "%2C";
                    fallback += secondRating;
                }
                var thirdRating = Plugin.Instance.Configuration.ThirdRating; 
                if (!thirdRating.Equals("none"))
                {
                    fallback += "%2C";
                    fallback += thirdRating;
                }
                var firstBackupRating = Plugin.Instance.Configuration.FirstBackupRating; 
                if (!firstBackupRating.Equals("none"))
                {
                    fallback += "%2C";
                    fallback += firstBackupRating;
                }
                var secondBackupRating = Plugin.Instance.Configuration.SecondBackupRating;             
                if (!secondBackupRating.Equals("none"))
                {
                    fallback += "%2C";
                    fallback += secondBackupRating;
                }
                var thirdBackupRating = Plugin.Instance.Configuration.ThirdBackupRating;             
                if (!thirdBackupRating.Equals("none"))
                {
                    fallback += "%2C";
                    fallback += thirdBackupRating;
                }
            }

            var ageRating = Plugin.Instance.Configuration.AgeRating;
            var videoQuality = Plugin.Instance.Configuration.VideoQuality;
            var video3D = Plugin.Instance.Configuration.Video3D;
            var colorRange = Plugin.Instance.Configuration.ColorRange;
            var audioChannels = Plugin.Instance.Configuration.AudioChannels;
            var videoCodec = Plugin.Instance.Configuration.VideoCodec;

            var addedVideoQuality = false;
            var addedVideo3d = false;
            var addedColorRange = false;
            var addedVideoCodec = false;
            var addedAudioChannels = false;

            var series = (Series)item;

            var badgeString = "";

            if (ageRating.Equals("1")) {
                badgeString += "commonsensegold";
            }

            if (videoQuality.Equals("1") || video3D.Equals("1") || colorRange.Equals("1") || audioChannels.Equals("1") || videoCodec.Equals("1")) {

                var episode = series
                    .GetRecursiveChildren().OfType<Episode>()
                    .FirstOrDefault();

                if (episode != null) {

                    var hasMediaSources = episode as IHasMediaSources;

                    if (hasMediaSources != null)
                    {
                        var mediaStreams = hasMediaSources.GetMediaStreams();

                        foreach (var stream in mediaStreams)
                        {
                            if (stream.Type.Equals(MediaStreamType.Video))
                            {
                                if (videoQuality.Equals("1") && !addedVideoQuality)
                                {
                                    var resBadge = "";
                                    if (stream.Width.HasValue)
                                    {
                                        if (stream.Width.Equals(852) || (stream.Width > 852 && stream.Width < 1280) || stream.Width < 852) {
                                            resBadge = "480p";
                                        }
                                        else if (stream.Width.Equals(1280) || (stream.Width > 1280 && stream.Width < 1920) || (stream.Width < 1280 && stream.Width > 852)) {
                                            resBadge = "720p";
                                        }
                                        else if (stream.Width.Equals(1920) || (stream.Width > 1920 && stream.Width < 2048) || (stream.Width < 1920 && stream.Width > 1280)) {
                                            resBadge = "1080p";
                                        }
                                        else if (stream.Width.Equals(2048) || (stream.Width > 2048 && stream.Width < 3840) || (stream.Width < 2048 && stream.Width > 1920)) {
                                            resBadge = "2k";
                                        }
                                        else if (stream.Width.Equals(3840) || (stream.Width > 3840 && stream.Width < 5120) || (stream.Width < 3840 && stream.Width > 2048)) {
                                            resBadge = "4k";
                                        }
                                        else if (stream.Width.Equals(5120) || (stream.Width > 5120 && stream.Width < 7680) || (stream.Width < 5120 && stream.Width > 3840)) {
                                            resBadge = "5k";
                                        }
                                        else if (stream.Width.Equals(7680) || stream.Width > 7680 || (stream.Width < 7680 && stream.Width > 5120)) {
                                            resBadge = "8k";
                                        }
                                        if (!resBadge.Equals("")) {
                                            if (!badgeString.Equals("")) {
                                                badgeString += "%2C";
                                            }
                                            badgeString += resBadge;
                                            addedVideoQuality = true;
                                        }
                                    }
                                }

                                if (video3D.Equals("1") && !addedVideo3d) {
                                    var video3dBadge = "";
                                    var video = episode as Video;
                                    if (video != null && video.Video3DFormat.HasValue) {
                                        video3dBadge = "3d";
                                    }
                                    if (!video3dBadge.Equals("")) {
                                        if (!badgeString.Equals("")) {
                                            badgeString += "%2C";
                                        }
                                        badgeString += video3dBadge;
                                        addedVideo3d = true;
                                    }
                                }

                                if (colorRange.Equals("1") && !addedColorRange) {
                                    var colorRangeBadge = "";
                                    if (!stream.VideoRangeType.Equals(VideoRangeType.Unknown))
                                    {
                                        if (stream.VideoRangeType.Equals(VideoRangeType.DOVI) || stream.VideoRangeType.Equals(VideoRangeType.DOVIWithHDR10) || stream.VideoRangeType.Equals(VideoRangeType.DOVIWithHLG) || stream.VideoRangeType.Equals(VideoRangeType.DOVIWithSDR)) {
                                            colorRangeBadge = "dolbyvisioncolor";
                                        }
                                        else if (stream.VideoRangeType.Equals(VideoRangeType.HDR10)) {
                                            colorRangeBadge = "hdrcolor";
                                        }
                                        else if (stream.VideoRangeType.Equals(VideoRangeType.HDR10Plus)) {
                                            colorRangeBadge = "hdr10plus_colorful";
                                        }
                                    }
                                    if (!colorRangeBadge.Equals("")) {
                                        if (!badgeString.Equals("")) {
                                            badgeString += "%2C";
                                        }
                                        badgeString += colorRangeBadge;
                                        addedColorRange = true;
                                    }
                                }

                                if (videoCodec.Equals("1") && !addedVideoCodec) {
                                    if (!string.IsNullOrEmpty(stream.Codec))
                                    {
                                        var videoCodecBadge = "";
                                        var codec = stream.Codec;
                                        if (string.Equals(codec, "hevc", StringComparison.OrdinalIgnoreCase) || string.Equals(codec, "h265", StringComparison.OrdinalIgnoreCase)) {
                                            videoCodecBadge = "h265";
                                        } else if (string.Equals(codec, "avc", StringComparison.OrdinalIgnoreCase) || string.Equals(codec, "h264", StringComparison.OrdinalIgnoreCase)) {
                                            videoCodecBadge = "h264";
                                        }
                                        if (!videoCodecBadge.Equals("")) {
                                            if (!badgeString.Equals("")) {
                                                badgeString += "%2C";
                                            }
                                            badgeString += videoCodecBadge;
                                            addedVideoCodec = true;
                                        }
                                    }
                                }
                            } else if (stream.Type.Equals(MediaStreamType.Audio)) {

                                if (audioChannels.Equals("1") && !addedAudioChannels) {
                                    if (stream.Channels.HasValue)
                                    {
                                        var audBadge = "";
                                        if (stream.Channels.Equals(2)) {
                                            audBadge = "audio20";
                                        }
                                        else if (stream.Channels.Equals(6)) {
                                            audBadge = "audio51";
                                        }
                                        else if (stream.Channels.Equals(8)) {
                                            audBadge = "audio71";
                                        }
                                        if (!audBadge.Equals("")) {
                                            if (!badgeString.Equals("")) {
                                                badgeString += "%2C";
                                            }
                                            badgeString += audBadge;
                                            addedAudioChannels = true;
                                        }
                                    }
                                }

                            }

                        }

                    }
                }
            }

            if (!badgeString.Equals("")) {
                fallback += "&badges=";
                fallback += badgeString;
                var badgeSize = Plugin.Instance.Configuration.BadgeSize;
                if (badgeSize.Equals("small")) {
                    fallback += "&badgeSize=small";
                }
                if (reqType.Equals("backdrop"))
                {
                    var backdropBadgePos = Plugin.Instance.Configuration.BackdropBadgePos;
                    if (!backdropBadgePos.Equals("left")) {
                        fallback += "&badgePos=";
                        fallback += backdropBadgePos;
                    }
                }
                else if (reqType.Equals("poster"))
                {
                    var badgePos = Plugin.Instance.Configuration.BadgePos;
                    if (!badgePos.Equals("left")) {
                        fallback += "&badgePos=";
                        fallback += badgePos;
                    }
                }
            }

            if (!clientKey.StartsWith("t0-") && !clientKey.StartsWith("t1-")) {
                if (reqType.Equals("backdrop"))
                {
                    var backdropSize = Plugin.Instance.Configuration.BackdropSize;
                    var backdropBarPos = Plugin.Instance.Configuration.BackdropBarPos;
                    var backdropBarOpacity = Plugin.Instance.Configuration.BackdropBarOpacity;
                    var backdropBarColor = Plugin.Instance.Configuration.BackdropBarColor;
                    var backdropFontColor = Plugin.Instance.Configuration.BackdropFontColor;
                    var backdropCertsPos = Plugin.Instance.Configuration.BackdropCertsPos;

                    if (!backdropSize.Equals("medium")) {
                        if (backdropSize.Equals("small")) {
                            width = 1280;
                            height = 720;
                        }
                        else if (backdropSize.Equals("large")) {
                            width = 3840;
                            height = 2160;
                        }
                        fallback += "&imageSize=";
                        fallback += backdropSize;
                    }

                    if (!backdropBarPos.Equals("right-top")) {
                        fallback += "&ratingBarPos=";
                        fallback += backdropBarPos;
                    }

                    if (!backdropBarOpacity.Equals("default")) {
                        fallback += "&ratingBarOpacity=";
                        fallback += backdropBarOpacity;
                    }

                    if (!backdropBarColor.Equals("default")) {
                        fallback += "&ratingBarColor=";
                        fallback += backdropBarColor;
                    }

                    if (!backdropFontColor.Equals("default")) {
                        fallback += "&fontColor=";
                        fallback += backdropFontColor;
                    }

                    if (!backdropCertsPos.Equals("left")) {
                        fallback += "&certsPos=";
                        fallback += backdropCertsPos;
                    }
                }
                else if (reqType.Equals("poster"))
                {
                    var posterSize = Plugin.Instance.Configuration.PosterSize;
                    var posterBarPos = Plugin.Instance.Configuration.PosterBarPos;
                    var posterBarOpacity = Plugin.Instance.Configuration.PosterBarOpacity;
                    var posterBarColor = Plugin.Instance.Configuration.PosterBarColor;
                    var posterFontColor = Plugin.Instance.Configuration.PosterFontColor;

                    if (!posterSize.Equals("medium")) {
                        if (posterSize.Equals("large")) {
                            width = 1280;
                            height = 1896;
                        }
                        else if (posterSize.Equals("verylarge")) {
                            width = 2000;
                            height = 2962;
                        }
                        fallback += "&imageSize=";
                        fallback += posterSize;
                    }

                    if (!posterBarPos.Equals("bottom")) {
                        fallback += "&ratingBarPos=";
                        fallback += posterBarPos;
                    }

                    if (!posterBarOpacity.Equals("default")) {
                        fallback += "&ratingBarOpacity=";
                        fallback += posterBarOpacity;
                    }

                    if (!posterBarColor.Equals("default")) {
                        fallback += "&ratingBarColor=";
                        fallback += posterBarColor;
                    }

                    if (!posterFontColor.Equals("default")) {
                        fallback += "&fontColor=";
                        fallback += posterFontColor;
                    }
                }
            }

            if (fallback.Length > 0) {
                fallback = '?' + fallback.Substring(1);
            }

            var url = string.Format(Plugin.BaseUrl, clientKey, idType, posterType, seriesId, fallback);

            list.Add(new RemoteImageInfo
            {
                Type = type,
                Width = width,
                Height = height,
                ProviderName = Name,
                Url = url,
                Language = null
            });

        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);

            var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response;
        }

    }
}
