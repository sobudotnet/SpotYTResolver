Imports System.Text.RegularExpressions
Imports System.Threading.Tasks

Public Class SpotifyUrlParser

    Public Shared Function ExtractTrackInfo(spotifyUrl As String) As TrackInfo
        If String.IsNullOrWhiteSpace(spotifyUrl) Then
            Return Nothing
        End If

        Try
            Dim uri As New Uri(spotifyUrl)
            Dim host = uri.Host.ToLower()

            ' handling spotify urls
            If host.Contains("open.spotify.com") Then
                Return ParseOpenSpotifyUrl(spotifyUrl)
            End If

            ' track url
            If spotifyUrl.StartsWith("spotify:track:") Then
                Return ParseSpotifyUri(spotifyUrl)
            End If

        Catch
            ' uwu pattern matching as fallback
            Return ParseByPattern(spotifyUrl)
        End Try

        Return Nothing
    End Function

    Private Shared Function ParseOpenSpotifyUrl(spotifyUrl As String) As TrackInfo
        ' URL format: https://open.spotify.com/track/[trackId]?si=[shareId]
        Dim match = Regex.Match(spotifyUrl, "spotify\.com/track/([a-zA-Z0-9]+)")
        If match.Success Then
            Dim trackId = match.Groups(1).Value
            Return New TrackInfo With {.TrackId = trackId}
        End If
        Return Nothing
    End Function

    Private Shared Function ParseSpotifyUri(spotifyUrl As String) As TrackInfo
        ' god this took too long
        Dim match = Regex.Match(spotifyUrl, "spotify:track:([a-zA-Z0-9]+)")
        If match.Success Then
            Dim trackId = match.Groups(1).Value
            Return New TrackInfo With {.TrackId = trackId}
        End If
        Return Nothing
    End Function

    Private Shared Function ParseByPattern(spotifyUrl As String) As TrackInfo
        ' extract track id from pattern
        Dim match = Regex.Match(spotifyUrl, "[a-zA-Z0-9]{22}")
        If match.Success Then
            Return New TrackInfo With {.TrackId = match.Value}
        End If
        Return Nothing
    End Function

    Public Shared Async Function GetSpotifyTrackMetadataAsync(trackId As String, trackService As SpotifyTrackService) As Task(Of TrackInfo)
        If String.IsNullOrWhiteSpace(trackId) Then
            Return Nothing
        End If

        Try
            Dim metadata = Await trackService.GetTrackAsync(trackId)
            If metadata Is Nothing Then
                Return Nothing
            End If

            Return New TrackInfo With {
                .TrackId = metadata.TrackId,
                .TrackName = metadata.TrackName,
                .ArtistName = metadata.ArtistName,
                .AlbumName = metadata.AlbumName,
                .ImageUrl = metadata.ImageUrl,
                .DurationMs = metadata.DurationMs
            }
        Catch ex As Exception
            Console.WriteLine($"Error getting Spotify metadata: {ex.Message}")
            Return Nothing
        End Try
    End Function
End Class

Public Class TrackInfo
    Public Property TrackId As String
    Public Property TrackName As String
    Public Property ArtistName As String
    Public Property AlbumName As String
    Public Property ImageUrl As String
    Public Property DurationMs As Integer
End Class
