Imports System.Configuration

Public Class SpotifyConfig

    Public Shared Function GetClientId() As String

        Dim creds = ConfigManager.LoadCredentials()
        If Not String.IsNullOrEmpty(creds.ClientId) Then
            Return creds.ClientId
        End If


        Dim clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID")
        If Not String.IsNullOrEmpty(clientId) Then
            Return clientId
        End If

        Try
            clientId = ConfigurationManager.AppSettings("SpotifyClientId")
            If Not String.IsNullOrEmpty(clientId) Then
                Return clientId
            End If
        Catch

        End Try

        Return Nothing
    End Function

    Public Shared Function GetClientSecret() As String

        Dim creds = ConfigManager.LoadCredentials()
        If Not String.IsNullOrEmpty(creds.ClientSecret) Then
            Return creds.ClientSecret
        End If


        Dim clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET")
        If Not String.IsNullOrEmpty(clientSecret) Then
            Return clientSecret
        End If


        Try
            clientSecret = ConfigurationManager.AppSettings("SpotifyClientSecret")
            If Not String.IsNullOrEmpty(clientSecret) Then
                Return clientSecret
            End If
        Catch

        End Try

        Return Nothing
    End Function

    Public Shared Function AreCredentialsConfigured() As Boolean
        Return ConfigManager.AreCredentialsConfigured()
    End Function
End Class
