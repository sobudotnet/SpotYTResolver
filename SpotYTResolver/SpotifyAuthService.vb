Imports System.Threading.Tasks
Imports SpotifyAPI.Web
Imports SpotifyAPI.Web.Auth

Public Class SpotifyAuthService
    Private Shared _instance As SpotifyAuthService
    Private _spotifyClient As SpotifyClient
    Private ReadOnly _clientId As String
    Private ReadOnly _clientSecret As String

    Public Shared Function GetInstance(clientId As String, clientSecret As String) As SpotifyAuthService
        If _instance Is Nothing Then
            _instance = New SpotifyAuthService(clientId, clientSecret)
        End If
        Return _instance
    End Function

    Public Shared Async Function AuthenticateAsync(clientId As String, clientSecret As String) As Task(Of SpotifyClient)
        Try
            Dim request = New ClientCredentialsRequest(clientId, clientSecret)
            Dim response = Await New OAuthClient().RequestToken(request)
            Return New SpotifyClient(response.AccessToken)
        Catch ex As Exception
            Throw New Exception($"Failed to authenticate with Spotify: {ex.Message}", ex)
        End Try
    End Function

    Private Sub New(clientId As String, clientSecret As String)
        _clientId = clientId
        _clientSecret = clientSecret
    End Sub

    Public Async Function InitializeAsync() As Task
        Try
            _spotifyClient = Await AuthenticateAsync(_clientId, _clientSecret)
        Catch ex As Exception
            Throw New Exception("Failed to initialize Spotify client", ex)
        End Try
    End Function

    Public ReadOnly Property Client As SpotifyClient
        Get
            If _spotifyClient Is Nothing Then
                Throw New InvalidOperationException("Spotify client not initialized. Call InitializeAsync first.")
            End If
            Return _spotifyClient
        End Get
    End Property

    Public ReadOnly Property IsAuthenticated As Boolean
        Get
            Return _spotifyClient IsNot Nothing
        End Get
    End Property
End Class
