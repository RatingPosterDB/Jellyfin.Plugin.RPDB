# Jellyfin.Plugin.RPDB

Jellyfin Plugin for [Rating Poster Database](https://ratingposterdb.com/).


Currently supports:
- Choosing poster type (Tier 1+): 4 available options
- Textless posters (Tier 3+)
- Backdrops (Tier 3+)


To install manually on Jellyfin Server:

- clone or download this repository
- ensure you have .NET Core SDK setup and installed
- build plugin with following command:
```
dotnet publish --configuration Release --output bin
```
- place the resulting file in the plugins folder under the program data directory or inside the portable install directory


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

