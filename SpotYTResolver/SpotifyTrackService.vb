Imports System.Threading.Tasks

Public Class SpotifyTrackService
    Private ReadOnly _authService As SpotifyAuthService

    Public Sub New(authService As SpotifyAuthService)
        _authService = authService
    End Sub

    Public Async Function GetTrackAsync(trackId As String) As Task(Of TrackMetadata)
        If String.IsNullOrWhiteSpace(trackId) Then
            Return Nothing
        End If

        Try
            If Not _authService.IsAuthenticated Then
                Await _authService.InitializeAsync()
            End If

            Dim track = Await _authService.Client.Tracks.Get(trackId)

            If track Is Nothing Then
                Return Nothing
            End If

            Return New TrackMetadata With {
                .TrackId = track.Id,
                .TrackName = track.Name,
                .ArtistName = GetArtistNames(track.Artists),
                .AlbumName = track.Album?.Name,
                .DurationMs = track.DurationMs,
                .ImageUrl = GetImageUrl(track.Album?.Images),
                .ExternalUrl = GetExternalUrl(track.ExternalUrls),
                .PreviewUrl = track.PreviewUrl
            }
        Catch ex As Exception
            Console.WriteLine($"Error fetching track metadata: {ex.Message}")
            Return Nothing
        End Try
    End Function

    Private Function GetArtistNames(artists As List(Of SpotifyAPI.Web.SimpleArtist)) As String
        If artists Is Nothing OrElse artists.Count = 0 Then
            Return "Unknown Artist"
        End If

        Return String.Join(", ", artists.Select(Function(a) a.Name))
    End Function

    Private Function GetImageUrl(images As List(Of SpotifyAPI.Web.Image)) As String
        If images Is Nothing OrElse images.Count = 0 Then
            Return String.Empty
        End If

        Dim firstImage = images.FirstOrDefault()
        If firstImage IsNot Nothing Then
            Return firstImage.Url
        End If
        Return String.Empty
    End Function

    Private Function GetExternalUrl(externalUrls As Dictionary(Of String, String)) As String
        If externalUrls Is Nothing Then
            Return String.Empty
        End If

        If externalUrls.ContainsKey("spotify") Then
            Return externalUrls("spotify")
        End If

        Return String.Empty
    End Function
End Class

Public Class TrackMetadata
    Public Property TrackId As String
    Public Property TrackName As String
    Public Property ArtistName As String
    Public Property AlbumName As String
    Public Property DurationMs As Integer
    Public Property ImageUrl As String
    Public Property ExternalUrl As String
    Public Property PreviewUrl As String
End Class
