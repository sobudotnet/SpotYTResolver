Imports System.Windows.Forms
Imports System.Drawing
Imports System.ComponentModel

Public Class SetupForm
    Inherits Form

    Private lblTitle As Label
    Private lblInstructions As Label
    Private lblClientId As Label
    Private txtClientId As TextBox
    Private lblClientSecret As Label
    Private txtClientSecret As TextBox
    Private btnSave As Button
    Private btnCancel As Button
    Private lblNote As Label
    Private linkLabel As LinkLabel
    Private pnlHeader As Panel
    Private pnlFooter As Panel

    ' colors
    Private Const SPOTIFY_GREEN As Integer = &H1ED760
    Private Const DARK_BG As Integer = &HF0F0F
    Private Const CARD_BG As Integer = &H282828
    Private Const TEXT_PRIMARY As Integer = &HFFFFFF
    Private Const TEXT_SECONDARY As Integer = &HB3B3B3

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ClientId As String

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ClientSecret As String

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Spotify API Configuration"
        Me.Size = New Size(700, 550)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.FromArgb((DARK_BG >> 16) And &HFF, (DARK_BG >> 8) And &HFF, DARK_BG And &HFF)
        Me.ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.DoubleBuffered = True

        ' header
        pnlHeader = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 80,
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF)
        }
        Me.Controls.Add(pnlHeader)

        ' title
        lblTitle = New Label With {
            .Text = "Spotify Configuration",
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .Location = New Point(20, 15),
            .Size = New Size(660, 30),
            .ForeColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF),
            .AutoEllipsis = True
        }
        pnlHeader.Controls.Add(lblTitle)

        ' instru
        lblInstructions = New Label With {
            .Text = "Enter your Spotify API credentials to get started",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(20, 48),
            .Size = New Size(660, 20),
            .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF)
        }
        pnlHeader.Controls.Add(lblInstructions)

        ' id label
        lblClientId = New Label With {
            .Text = "Client ID:",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(20, 100),
            .Size = New Size(100, 25),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF)
        }
        Me.Controls.Add(lblClientId)

        ' client id textbox
        txtClientId = New TextBox With {
            .Location = New Point(20, 125),
            .Size = New Size(660, 40),
            .Font = New Font("Segoe UI", 11),
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF),
            .BorderStyle = BorderStyle.FixedSingle,
            .Multiline = False
        }
        Me.Controls.Add(txtClientId)

        ' client secret label
        lblClientSecret = New Label With {
            .Text = "Client Secret:",
            .Font = New Font("Segoe UI", 10),
            .Location = New Point(20, 175),
            .Size = New Size(100, 25),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF)
        }
        Me.Controls.Add(lblClientSecret)

        ' client secret textbox
        txtClientSecret = New TextBox With {
            .Location = New Point(20, 200),
            .Size = New Size(660, 40),
            .Font = New Font("Segoe UI", 11),
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF),
            .BorderStyle = BorderStyle.FixedSingle,
            .UseSystemPasswordChar = True,
            .Multiline = False
        }
        Me.Controls.Add(txtClientSecret)

        ' courtesy note
        lblNote = New Label With {
            .Text = "Credentials will be saved securely in spotifyconfig.json. Keep your Client Secret safe!",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(20, 250),
            .Size = New Size(660, 40),
            .ForeColor = Color.FromArgb((TEXT_SECONDARY >> 16) And &HFF, (TEXT_SECONDARY >> 8) And &HFF, TEXT_SECONDARY And &HFF),
            .AutoSize = False
        }
        Me.Controls.Add(lblNote)

        ' kink to the dev dashboard for spotify api creds
        linkLabel = New LinkLabel With {
            .Text = "Click here to get your credentials from Spotify Developer Dashboard",
            .Font = New Font("Segoe UI", 9),
            .Location = New Point(20, 295),
            .Size = New Size(660, 30),
            .LinkColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF),
            .ActiveLinkColor = Color.FromArgb(&HFF, &HFF, &H00),
            .VisitedLinkColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF)
        }
        AddHandler linkLabel.LinkClicked, AddressOf LinkLabel_LinkClicked
        Me.Controls.Add(linkLabel)

        ' footer panel
        pnlFooter = New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 60,
            .BackColor = Color.FromArgb((CARD_BG >> 16) And &HFF, (CARD_BG >> 8) And &HFF, CARD_BG And &HFF)
        }
        Me.Controls.Add(pnlFooter)

        ' yah save 
        btnSave = New Button With {
            .Text = "Save Credentials",
            .Location = New Point(440, 12),
            .Size = New Size(120, 35),
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .BackColor = Color.FromArgb((SPOTIFY_GREEN >> 16) And &HFF, (SPOTIFY_GREEN >> 8) And &HFF, SPOTIFY_GREEN And &HFF),
            .ForeColor = Color.Black,
            .FlatStyle = FlatStyle.Flat,
            .DialogResult = DialogResult.OK
        }
        btnSave.FlatAppearance.BorderSize = 0
        btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(&H2CD660)
        btnSave.FlatAppearance.MouseDownBackColor = Color.FromArgb(&H1A9D48)
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        pnlFooter.Controls.Add(btnSave)

        ' cancel
        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(570, 12),
            .Size = New Size(110, 35),
            .Font = New Font("Segoe UI", 10),
            .BackColor = Color.FromArgb(&H404040),
            .ForeColor = Color.FromArgb((TEXT_PRIMARY >> 16) And &HFF, (TEXT_PRIMARY >> 8) And &HFF, TEXT_PRIMARY And &HFF),
            .FlatStyle = FlatStyle.Flat,
            .DialogResult = DialogResult.Cancel
        }
        btnCancel.FlatAppearance.BorderSize = 1
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(&H666666)
        pnlFooter.Controls.Add(btnCancel)
    End Sub

    Private Sub LinkLabel_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Try
            Dim psi = New System.Diagnostics.ProcessStartInfo With {
                .FileName = "https://developer.spotify.com/dashboard",
                .UseShellExecute = True
            }
            System.Diagnostics.Process.Start(psi)
        Catch
            MessageBox.Show("Could not open browser. Visit https://developer.spotify.com/dashboard manually.", "Browser Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtClientId.Text) OrElse String.IsNullOrWhiteSpace(txtClientSecret.Text) Then
            MessageBox.Show("Please enter both Client ID and Client Secret.", "Missing Credentials", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ClientId = txtClientId.Text.Trim()
        ClientSecret = txtClientSecret.Text.Trim()

        Try
            MessageBox.Show("Credentials saved successfully! The application will now connect to Spotify.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show($"Error saving credentials: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
