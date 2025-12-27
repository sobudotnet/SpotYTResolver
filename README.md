# SpotYT

Spotify YouTube Resolver - Find your Spotify songs on YouTube instantly. Just drag a Spotify link in and boom, YouTube results.

## Getting Started (Super Easy)

### Step 1: Get Your Spotify Keys

1. Head to [Spotify Developer Dashboard](https://developer.spotify.com/dashboard)
2. Sign up or log in (free account works fine)
3. Create a new app
4. (When you create your id, check `Web API` when making your api creds)
5. Grab your **Client ID** and **Client Secret** - that's all you need

### Step 2: Feed It Your Credentials

Pick whichever sounds easiest:

**Option 1: Let The App Ask You (Laziest)**
- Open the app
- A dialog pops up asking for your credentials
- Paste 'em in and click save
- Done, seriously that's it

**Option 2: Run The Script**
1. Right-click `setup-spotify-credentials.bat`
2. Pick "Run as administrator"
3. Type in your ID and Secret 
4. Close and reopen the app

**Option 3: Edit The Config File**
1. Open `app.config`
2. Throw in your credentials:
   ```xml
   <add key="SpotifyClientId" value="YOUR_ID" />
   <add key="SpotifyClientSecret" value="YOUR_SECRET" />
   ```
3. Save and restart

### Step 3: Use It

1. Open the app
2. Drag a Spotify link into the text box (or just paste it)
3. Check out the YouTube results with album art and everything
4. Click "Open" to watch it on YouTube or "Copy" to grab the link

Done. That's literally the whole thing.

## What Spotify Links Work?

- `https://open.spotify.com/track/...` (web links)
- `spotify:track:...` (desktop app)
- Any share link from Spotify

## Cool Stuff It Does

- Drag & drop straight from spotify (super convenient)
- Actually gets real song info from Spotify
- Shows you the top 10 YouTube results
- Dark theme (easy on the eyes)
- Doesn't spy on you or track anything
- Works offline after you paste a link (well, for the search part)

## Something Broke?

| What's Wrong | How to Fix It |
|---------|----------|
| Says credentials aren't set up | Just run the in-app setup or the batch script |
| Can't find the song you want | Try searching a different way or check your internet |
| Album art isn't showing | Probably a connection thing - app works fine without it anyway |
| Icon looks weird | Make sure `spotyt.ico` is in the app folder and rebuild |

## How It Works (If You Care)

Built with:
- .NET 9.0 (Windows Forms)
- Spotify's official API
- Some YouTube scraping magic

What it doesn't do:
- Ask you to sign in to anything
- Require accounts
- Collect data
- Track you

## The Code

Everything's in the `SpotYTResolver/` folder. Main stuff:
- `MainForm.vb` - the UI and main logic
- `SpotifyAuthService.vb` - talks to Spotify
- `YouTubeSearchService.vb` - finds videos on YouTube
- `ConfigManager.vb` - stores your credentials

## License

MIT - do whatever you want with it

---

Made by someone who was tired of typing in song titles manually to find them on YouTube. Enjoy!
