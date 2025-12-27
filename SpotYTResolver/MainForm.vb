Imports System.Windows.Forms
Imports System.Drawing
Imports System.Configuration
Imports System.Threading.Tasks

Public Class MainForm
    Inherits Form

    Private WithEvents lblTitle As Label
    Private WithEvents lblInstruction As Label
    Private WithEvents txtSpotifyUrl As TextBox
    Private WithEvents btnSearch As Button
    Private WithEvents pnlResults As Panel
    Private WithEvents lblStatus As Label
    Private WithEvents lblLoading As Label
    Private lblSetupError As Label
    Private pnlStatusBar As Panel
    Private lblStatusMessage As Label
    Private lblDebugMessage As Label
    Private _spotifyAuthService As SpotifyAuthService
    Private _spotifyTrackService As SpotifyTrackService
    Private _isInitialized As Boolean = False

    Private Const SPOTIFY_GREEN As Integer = &H1ED760
    Private Const YOUTUBE_RED As Integer = &HFF0000
    Private Const DARK_BG As Integer = &HF0F0F
    Private Const LIGHT_BG As Integer = &H181818
    Private Const CARD_BG As Integer = &H282828
    Private Const TEXT_PRIMARY As Integer = &HFFFFFF
    Private Const TEXT_SECONDARY As Integer = &HB3B3B3

    Public Sub New()
        InitializeComponent()
        SetupDragAndDrop()
        ConfigManager.Initialize()
        SetApplicationIcon()
    End Sub

    Private Sub SetApplicationIcon()
        Try
            Dim iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "spotyt.ico")
            If System.IO.File.Exists(iconPath) Then
                Me.Icon = New System.Drawing.Icon(iconPath)
            End If
        Catch ex As Exception
            Console.WriteLine($"Icon load error: {ex.Message}")
        End Try
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeSpotifyAsync()
    End Sub

    Private Async Sub InitializeSpotifyAsync()
        Try

            Dim creds = ConfigManager.LoadCredentials()

            If String.IsNullOrEmpty(creds.ClientId) OrElse String.IsNullOrEmpty(creds.ClientSecret) Then
                ShowSetupError("Spotify API credentials not configured")
                ShowSetupDialog()
                Return
            End If

            _spotifyAuthService = SpotifyAuthService.GetInstance(creds.ClientId, creds.ClientSecret)
            Await _spotifyAuthService.InitializeAsync()
            _spotifyTrackService = New SpotifyTrackService(_spotifyAuthService)
            _isInitialized = True
            HideSetupError()

        Catch ex As Exception
            ShowSetupError($"Failed to initialize Spotify: {ex.Message}")
        End Try
    End Sub

    Private Sub ShowSetupDialog()
        Dim setupForm = New SetupForm()
        If setupForm.ShowDialog() = DialogResult.OK Then
            ConfigManager.SaveCredentials(setupForm.ClientId, setupForm.ClientSecret)
            ' Reinitialize with new creds
            InitializeSpotifyAsync()
        End If
    End Sub

    Private Sub ShowSetupError(message As String)
        lblSetupError.Text = message
        lblSetupError.Visible = True
        btnSearch.Enabled = False
        UpdateDebugMessage("Setup Error: " & message, Color.Red)
    End Sub

    Private Sub HideSetupError()
        lblSetupError.Visible = False
        btnSearch.Enabled = True
        UpdateDebugMessage("Ready", Color.FromArgb(30, 200, 96))
    End Sub

    Private Sub UpdateDebugMessage(message As String, Optional color As Color = Nothing)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() UpdateDebugMessage(message, color))
            Return
        End If

        lblDebugMessage.Text = message
        If color <> Nothing Then
            lblDebugMessage.ForeColor = color
        End If
    End Sub

    Private Sub InitializeComponent()
        ' main ting
        Me.Text = "SpotYT"
        Me.Size = New Size(1000, 900)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.FromArgb((DARK_BG >> 16) And &HFF, (DARK_BG >> 8) And &HFF, DARK_BG And &HFF)
        Me.ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF)
        Me.DoubleBuffered = True

        ' title label
        lblTitle = New Label With {
            .Text = "SpotYT",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point(25, 20),
            .ForeColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF)
        }
        Me.Controls.Add(lblTitle)

        ' label
        Dim lblSubtitle = New Label With {
            .Text = "Spotify YouTube Resolver",
            .Font = New Font("Segoe UI", 9),
            .AutoSize = True,
            .Location = New Point(25, 50),
            .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF)
        }
        Me.Controls.Add(lblSubtitle)

        ' inst
        lblInstruction = New Label With {
            .Text = "Drag a Spotify link here or paste it below to find it on YouTube",
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True,
            .Location = New Point(25, 73),
            .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF)
        }
        Me.Controls.Add(lblInstruction)

        ' url input box
        txtSpotifyUrl = New TextBox With {
            .Location = New Point(25, 103),
            .Size = New Size(750, 45),
            .Font = New Font("Segoe UI", 11),
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF),
            .AllowDrop = True,
            .Multiline = False,
            .BorderStyle = BorderStyle.FixedSingle
        }
        Me.Controls.Add(txtSpotifyUrl)

        ' search button
        btnSearch = New Button With {
            .Text = "Search on YouTube",
            .Location = New Point(785, 103),
            .Size = New Size(190, 45),
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .BackColor = Color.FromArgb((YOUTUBE_RED >> 16) And &HFF, (YOUTUBE_RED >> 8) And &HFF, YOUTUBE_RED And &HFF),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        btnSearch.FlatAppearance.BorderSize = 0
        btnSearch.FlatAppearance.MouseOverBackColor = Color.FromArgb(&HCC, 0, 0)
        btnSearch.FlatAppearance.MouseDownBackColor = Color.FromArgb(&H990000)
        Me.Controls.Add(btnSearch)

        ' setup err
        lblSetupError = New Label With {
            .Text = "",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(25, 155),
            .Size = New Size(950, 35),
            .ForeColor = Color.FromArgb(&HFF, &H4D, &H4D),
            .Visible = False,
            .AutoSize = False,
            .BackColor = Color.FromArgb(&H331111)
        }
        Me.Controls.Add(lblSetupError)

        ' loading lbl
        lblLoading = New Label With {
            .Text = "Searching...",
            .Font = New Font("Segoe UI", 11),
            .Location = New Point(25, 200),
            .Size = New Size(950, 30),
            .ForeColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF),
            .Visible = False,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblLoading)

        ' results
        pnlResults = New Panel With {
            .Location = New Point(25, 235),
            .Size = New Size(950, 530),
            .BackColor = Color.FromArgb((LIGHT_BG >> 16) And &HFF, (LIGHT_BG >> 8) And &HFF, LIGHT_BG And &HFF),
            .AutoScroll = True,
            .BorderStyle = BorderStyle.None
        }
        Me.Controls.Add(pnlResults)

        ' status bar
        pnlStatusBar = New Panel With {
            .Location = New Point(0, Me.Height - 70),
            .Size = New Size(Me.Width, 70),
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF),
            .BorderStyle = BorderStyle.None,
            .Dock = DockStyle.Bottom
        }
        Me.Controls.Add(pnlStatusBar)

        ' status msg
        lblStatusMessage = New Label With {
            .Text = "Status: Initializing...",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(15, 8),
            .Size = New Size(700, 20),
            .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF),
            .AutoEllipsis = True
        }
        pnlStatusBar.Controls.Add(lblStatusMessage)

        ' debug msg lbl
        lblDebugMessage = New Label With {
            .Text = "Ready",
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .Location = New Point(15, 30),
            .Size = New Size(700, 20),
            .ForeColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF)
        }
        pnlStatusBar.Controls.Add(lblDebugMessage)

        ' handlers
        AddHandler txtSpotifyUrl.DragDrop, AddressOf TxtSpotifyUrl_DragDrop
        AddHandler txtSpotifyUrl.DragOver, AddressOf TxtSpotifyUrl_DragOver
        AddHandler btnSearch.Click, AddressOf BtnSearch_Click
        AddHandler Me.Resize, Sub(sender As Object, e As EventArgs)
                                  ' Adjust results panel height on window resize
                                  pnlResults.Size = New Size(950, Me.Height - 395)
                                  pnlStatusBar.Location = New Point(0, Me.Height - 70)
                                  pnlStatusBar.Size = New Size(Me.Width, 70)
                              End Sub
    End Sub

    Private Sub SetupDragAndDrop()
        txtSpotifyUrl.AllowDrop = True
    End Sub

    Private Sub TxtSpotifyUrl_DragOver(sender As Object, e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.Text) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub TxtSpotifyUrl_DragDrop(sender As Object, e As DragEventArgs)
        Dim droppedText = CStr(e.Data.GetData(DataFormats.Text))
        If droppedText.Contains("spotify") Then
            txtSpotifyUrl.Text = droppedText.Trim()
            BtnSearch_Click(Nothing, Nothing)
        Else
            MessageBox.Show("Please drop a valid Spotify link", "Invalid Link", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Async Sub BtnSearch_Click(sender As Object, e As EventArgs)
        If Not _isInitialized Then
            MessageBox.Show("Spotify is still initializing. Please wait a moment and try again.", "Not Ready", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim spotifyUrl = txtSpotifyUrl.Text.Trim()

        If String.IsNullOrWhiteSpace(spotifyUrl) Then
            MessageBox.Show("Please enter or drag a Spotify link", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' parse spotify url
        Dim trackInfo = SpotifyUrlParser.ExtractTrackInfo(spotifyUrl)

        If trackInfo Is Nothing Then
            MessageBox.Show("Could not parse the Spotify URL. Please check the link.", "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' fetch metadata
        lblLoading.Visible = True
        UpdateDebugMessage("Fetching track from Spotify...", Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF))
        lblStatusMessage.Text = "Status: Fetching Spotify metadata..."
        pnlResults.Controls.Clear()

        Try
            Dim fullTrackInfo = Await SpotifyUrlParser.GetSpotifyTrackMetadataAsync(trackInfo.TrackId, _spotifyTrackService)

            If fullTrackInfo Is Nothing Then
                MessageBox.Show("Could not fetch track information from Spotify API.", "Metadata Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                lblLoading.Visible = False
                UpdateDebugMessage("Metadata fetch failed", Color.Red)
                Return
            End If

            ' build yt query
            Dim searchQuery = $"{fullTrackInfo.TrackName} {fullTrackInfo.ArtistName}"
            lblStatusMessage.Text = $"Status: Searching YouTube for: {searchQuery}"
            UpdateDebugMessage("Searching YouTube...", Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF))

            Dim results = Await YouTubeSearchService.SearchAsync(searchQuery)
            DisplayResults(results, fullTrackInfo)
            UpdateDebugMessage($"Found {results.Count} results", Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF))

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            UpdateDebugMessage($"Error: {ex.Message}", Color.Red)
        Finally
            lblLoading.Visible = False
        End Try
    End Sub

    Private Sub DisplayResults(results As List(Of YouTubeSearchService.YouTubeResult), trackInfo As TrackInfo)
        pnlResults.Controls.Clear()

        If results.Count = 0 Then
            Dim noResultsLabel = New Label With {
                .Text = "No results found. Try a different search query.",
                .Font = New Font("Segoe UI", 11),
                .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF),
                .Dock = DockStyle.Top,
                .TextAlign = ContentAlignment.MiddleCenter,
                .Height = 50
            }
            pnlResults.Controls.Add(noResultsLabel)
            Return
        End If

        ' display track info 
        Dim headerPanel = CreateTrackInfoHeader(trackInfo)
        pnlResults.Controls.Add(headerPanel)

        Dim yPosition = 120

        For Each result In results
            Dim resultPanel = CreateResultPanel(result, yPosition)
            pnlResults.Controls.Add(resultPanel)
            yPosition += 100
        Next
    End Sub

    Private Function CreateTrackInfoHeader(trackInfo As TrackInfo) As Panel
        Dim headerPanel = New Panel With {
            .Location = New Point(10, 10),
            .Size = New Size(920, 100),
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF),
            .BorderStyle = BorderStyle.None
        }

        ' arttt
        Dim picAlbumArt = New PictureBox With {
            .Location = New Point(10, 10),
            .Size = New Size(80, 80),
            .SizeMode = PictureBoxSizeMode.StretchImage,
            .BackColor = Color.FromArgb((LIGHT_BG >> 16) And &HFF, (LIGHT_BG >> 8) And &HFF, LIGHT_BG And &HFF)
        }

        If Not String.IsNullOrEmpty(trackInfo.ImageUrl) Then
            Try
                picAlbumArt.LoadAsync(trackInfo.ImageUrl)
            Catch
                ' no worries no img
            End Try
        End If

        headerPanel.Controls.Add(picAlbumArt)

        ' trackname
        Dim lblTrackName = New Label With {
            .Text = trackInfo.TrackName,
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .Location = New Point(100, 10),
            .Size = New Size(810, 25),
            .ForeColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF),
            .AutoEllipsis = True
        }
        headerPanel.Controls.Add(lblTrackName)

        ' artistname
        Dim lblArtistName = New Label With {
            .Text = trackInfo.ArtistName,
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(100, 38),
            .Size = New Size(810, 20),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF),
            .AutoEllipsis = True
        }
        headerPanel.Controls.Add(lblArtistName)

        ' albumname
        Dim lblAlbumName = New Label With {
            .Text = trackInfo.AlbumName,
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(100, 60),
            .Size = New Size(810, 20),
            .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF),
            .AutoEllipsis = True
        }
        headerPanel.Controls.Add(lblAlbumName)

        Return headerPanel
    End Function

    Private Function CreateResultPanel(result As YouTubeSearchService.YouTubeResult, yPos As Integer) As Panel
        Dim resultPanel = New Panel With {
            .Location = New Point(10, yPos),
            .Size = New Size(920, 90),
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF),
            .BorderStyle = BorderStyle.None,
            .Cursor = Cursors.Hand
        }

        ' thumbnail
        Dim picThumbnail = New PictureBox With {
            .Location = New Point(10, 10),
            .Size = New Size(60, 60),
            .SizeMode = PictureBoxSizeMode.StretchImage,
            .BackColor = Color.FromArgb((LIGHT_BG >> 16) And &HFF, (LIGHT_BG >> 8) And &HFF, LIGHT_BG And &HFF)
        }

        Try
            picThumbnail.LoadAsync(result.ThumbnailUrl)
        Catch
            ' ignore on err
        End Try

        resultPanel.Controls.Add(picThumbnail)

        ' Ttle label
        Dim lblResultTitle = New Label With {
            .Text = result.Title,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .Location = New Point(80, 10),
            .Size = New Size(560, 35),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF),
            .AutoEllipsis = True
        }
        resultPanel.Controls.Add(lblResultTitle)

        ' Channel Label
        Dim lblChannel = New Label With {
            .Text = "YouTube Video",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(80, 50),
            .Size = New Size(560, 20),
            .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF)
        }
        resultPanel.Controls.Add(lblChannel)

        ' copy da url (for my vrchat peeps ykyk)
        Dim btnCopyUrl = New Button With {
            .Text = "Copy",
            .Location = New Point(680, 15),
            .Size = New Size(60, 30),
            .Font = New Font("Segoe UI", 9),
            .BackColor = Color.FromArgb(&H404040),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF),
            .FlatStyle = FlatStyle.Flat,
            .Tag = result.Url
        }
        btnCopyUrl.FlatAppearance.BorderSize = 1
        btnCopyUrl.FlatAppearance.BorderColor = Color.FromArgb(&H666666)

        AddHandler btnCopyUrl.Click, Sub(sender As Object, e As EventArgs)
                                         Clipboard.SetText(result.Url)
                                         MessageBox.Show("URL copied to clipboard!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                     End Sub

        resultPanel.Controls.Add(btnCopyUrl)

        ' open url
        Dim btnOpen = New Button With {
            .Text = "Play",
            .Location = New Point(750, 15),
            .Size = New Size(60, 30),
            .Font = New Font("Segoe UI", 9, FontStyle.Bold),
            .BackColor = Color.FromArgb((YOUTUBE_RED >> 16) And &HFF, (YOUTUBE_RED >> 8) And &HFF, YOUTUBE_RED And &HFF),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Tag = result.Url
        }
        btnOpen.FlatAppearance.BorderSize = 0

        AddHandler btnOpen.Click, Sub(sender As Object, e As EventArgs)
                                      YouTubeSearchService.OpenInBrowser(result.Url)
                                  End Sub

        resultPanel.Controls.Add(btnOpen)

        ' click handla for the wholle thing
        AddHandler resultPanel.Click, Sub(sender As Object, e As EventArgs)
                                          YouTubeSearchService.OpenInBrowser(result.Url)
                                      End Sub

        Return resultPanel
    End Function
End Class
