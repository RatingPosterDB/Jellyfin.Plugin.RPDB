using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Json;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.RPDB.Providers
{
    public class MovieProvider : IRemoteImageProvider, IHasOrder
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MovieProvider(IServerConfigurationManager config, IHttpClientFactory httpClientFactory, IFileSystem fileSystem)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public string Name => "RPDB";

        /// <inheritdoc />
        public int Order => 1;

        /// <inheritdoc />
        public bool Supports(BaseItem item)
            => item is Movie;

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
            var baseItem = item;
            var list = new List<RemoteImageInfo>();

            var idType = "imdb";
            var movieId = baseItem.GetProviderId(MetadataProvider.Imdb);

            if (string.IsNullOrEmpty(movieId))
            {
                idType = "tmdb";
                movieId = baseItem.GetProviderId(MetadataProvider.Tmdb);
            }

            if (!string.IsNullOrEmpty(movieId))
            {
                try
                {
                    await AddImages(list, idType, movieId, cancellationToken).ConfigureAwait(false);
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

        private async Task AddImages(List<RemoteImageInfo> list, string idType, string movieId, CancellationToken cancellationToken)
        {
            await Task.Run(() => PopulateImages(list, "poster", idType, movieId, ImageType.Primary, 580, 859));
            await Task.Run(() => PopulateImages(list, "backdrop", idType, movieId, ImageType.Backdrop, 1920, 1080));
        }

        private void PopulateImages(List<RemoteImageInfo> list, string reqType, string idType, string movieId, ImageType type, int width, int height)
        {

            var clientKey = Plugin.Instance.Configuration.UserApiKey;

            if (string.IsNullOrWhiteSpace(clientKey))
            {
                return;
            }

            var posterType = "poster-default";
            var fallback = "";

            if (reqType.Equals("backdrop"))
            {
                var backdrops = Plugin.Instance.Configuration.Backdrops;
                if (backdrops.Equals("1"))
                {
                    if (clientKey.StartsWith("t1-") || clientKey.StartsWith("t2-"))
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
                posterType = Plugin.Instance.Configuration.PosterType;
                fallback = "?fallback=true";
                var textless = Plugin.Instance.Configuration.Textless;
                if (textless.Equals("1"))
                {
                    if (clientKey.StartsWith("t1-") || clientKey.StartsWith("t2-"))
                    {
                        return;
                    }
                    else
                    {
                        posterType = posterType.Replace("poster-", "textless-");
                    }
                }
            }

            var url = string.Format(Plugin.BaseUrl, clientKey, idType, posterType, movieId, fallback);

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
