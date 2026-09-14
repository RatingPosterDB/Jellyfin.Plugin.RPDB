# Jellyfin Plugin for RPDB

Jellyfin Plugin for [Rating Poster Database](https://ratingposterdb.com/).


Currently supports:
- Image Rating Themes (Tier 1+)
- Choose Rating Position (Tier 1+)
- Textless posters (Tier 1+)
- Default Poster Language (Tier 1+)
- Custom Rating Order (from 11 rating sources) (Tier 1+)
- Backdrops (Tier 2+)
- Logos (Tier 2+)
- Badges (Tier 2+)
- Font Size / Weight (Tier 2+)
- Image Styling (size, bar position, bar color, font color, etc) (Tier 3+)

## Installation

- Repository for Jellyfin:
  - Add this URL to the list:
    - For **Jellyfin 12.0**
      ```
      https://raw.githubusercontent.com/RatingPosterDB/Jellyfin.Plugin.RPDB/refs/heads/main/repo/jellyfin_12.0.json
      ```
    - For **Jellyfin 10.11**
      ```
      https://raw.githubusercontent.com/RatingPosterDB/Jellyfin.Plugin.RPDB/refs/heads/main/repo/jellyfin_10.11.json
      ```
    - For **Jellyfin 10.10**
      ```
      https://raw.githubusercontent.com/RatingPosterDB/Jellyfin.Plugin.RPDB/refs/heads/main/repo/jellyfin_10.10.json
      ```
    - For **Jellyfin 10.9**
      ```
      https://raw.githubusercontent.com/RatingPosterDB/Jellyfin.Plugin.RPDB/refs/heads/main/repo/jellyfin_10.9.json
      ```

## Manual installation

To install manually on Jellyfin Server:

- [download plugin](https://github.com/jaruba/Jellyfin.Plugin.RPDB/releases/) for the appropriate Jellyfin version
- unpack it
- move the entire "RPDB_1.x.x.x" folder to Jellyfin's "plugins" folder
- restart Jellyfin

## Setting up the plugin

- disable IPv6 support on the device where you run your server (this is important as it can cause very high load times and even fail the image download)
- go to Settings > Advanced > Plugins
- click the RatingPosterDB plugin to set it up
- click "Save" to save settings
- go to Settings > Server > Library
- hover the libraries that you want to use RPDB with
- click the "..." to access the library's settings
- scroll down to "Movie Image Fetchers" (for series this is "Series Image Fetchers")
- enable the RatingPosterDB plugin from the list
- move the RatingPosterDB plugin to the top of the "Movie Image Fetchers" list
- click the "..." for the same library again
- select "Scan Library"
- under "Refresh mode" choose "Replace all metadata"
- enable "Replace existing images"

### Result screenshot

![jellyfin](https://user-images.githubusercontent.com/1777923/115111376-c7b79d00-9f88-11eb-85f5-af2107e697b4.jpg)

### Settings screenshot

![Jellyfin-Plugin-Settings](https://github.com/user-attachments/assets/ed0e43cc-634c-4469-a2a7-c442941385f7)
