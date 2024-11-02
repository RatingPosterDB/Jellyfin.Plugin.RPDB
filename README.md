# Jellyfin Plugin for RPDB

Jellyfin Plugin for [Rating Poster Database](https://ratingposterdb.com/).


Currently supports:
- Choosing poster type (Tier 1+): 4 available options
- Textless posters (Tier 1+)
- Poster Language (Tier 2+)
- Backdrops (Tier 3+)


To install manually on Jellyfin Server:

- [download plugin](https://github.com/jaruba/Jellyfin.Plugin.RPDB-bin/releases/latest/download/Jellyfin.Plugin.RPDB.zip) (built for **Jellyfin v10.7.6**)
- unpack it
- move the entire "RPDB_1.0.0.3" folder to Jellyfin's "plugins" folder
- restart Jellyfin


Setting up the plugin:

- go to Settings > Advanced > Plugins
- click the RPDB plugin to set it up
- click "Save" to save settings
- go to Settings > Server > Library
- hover the libraries that you want to use RPDB with
- click the "..." to access the library's settings
- scroll down to "Movie Image Fetchers"
- enable the RPDB plugin from the list
- move the RPDB plugin to the top of the "Movie Image Fetchers" list
- click the "..." for the same library again
- select "Refresh Metadata"

Result screenshot:

![jellyfin](https://user-images.githubusercontent.com/1777923/115111376-c7b79d00-9f88-11eb-85f5-af2107e697b4.jpg)

Settings screenshot:

![Jellyfin-Plugin-Settings](https://user-images.githubusercontent.com/1777923/124392733-f0d20900-dcff-11eb-9d83-4b25154d57f9.png)
