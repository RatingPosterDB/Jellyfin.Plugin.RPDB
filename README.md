# Jellyfin Plugin for RPDB

Jellyfin Plugin for [Rating Poster Database](https://ratingposterdb.com/).


Currently supports:
- Choosing poster type (Tier 1+): 4 available options
- Textless posters (Tier 3+)
- Backdrops (Tier 3+)


To install manually on Jellyfin Server:

- [download plugin](https://github.com/jaruba/Jellyfin.Plugin.RPDB-bin/releases/download/v1.0.1/Jellyfin.Plugin.RPDB.zip) (built for **Jellyfin v10.7.2**)
- unpack it
- move the entire "RPDB" folder to Jellyfin's "plugins" folder
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

![jellyfin](https://user-images.githubusercontent.com/1777923/114306334-5c7f4e00-9ae4-11eb-9856-5eaf40c05309.jpg)
