Imports System.IO
Imports Newtonsoft.Json

Public Class ConfigManager
    Private Shared ReadOnly ConfigFileName As String = "spotifyconfig.json"
    Private Shared ReadOnly ConfigPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName)

    Public Class SpotifyCredentials
        <JsonProperty("clientId")>
        Public Property ClientId As String

        <JsonProperty("clientSecret")>
        Public Property ClientSecret As String

        <JsonProperty("lastUpdated")>
        Public Property LastUpdated As DateTime
    End Class

    Public Shared Sub Initialize()
        If Not File.Exists(ConfigPath) Then
            ' Create default config with empty creds
            Dim defaultConfig = New SpotifyCredentials With {
                .ClientId = "",
                .ClientSecret = "",
                .LastUpdated = DateTime.Now
            }
            SaveConfig(defaultConfig)
        End If
    End Sub

    Public Shared Function LoadCredentials() As SpotifyCredentials
        Try
            If Not File.Exists(ConfigPath) Then
                Initialize()
                Return New SpotifyCredentials()
            End If

            Dim json = File.ReadAllText(ConfigPath)
            Dim credentials = JsonConvert.DeserializeObject(Of SpotifyCredentials)(json)
            Return If(credentials, New SpotifyCredentials())
        Catch ex As Exception
            Console.WriteLine($"Error loading config: {ex.Message}")
            Return New SpotifyCredentials()
        End Try
    End Function

    Public Shared Sub SaveCredentials(clientId As String, clientSecret As String)
        Try
            Dim credentials = New SpotifyCredentials With {
                .ClientId = clientId,
                .ClientSecret = clientSecret,
                .LastUpdated = DateTime.Now
            }
            SaveConfig(credentials)
        Catch ex As Exception
            Throw New Exception($"Failed to save credentials: {ex.Message}", ex)
        End Try
    End Sub

    Private Shared Sub SaveConfig(credentials As SpotifyCredentials)
        Try
            Dim json = JsonConvert.SerializeObject(credentials, Formatting.Indented)
            File.WriteAllText(ConfigPath, json)
        Catch ex As Exception
            Throw New Exception($"Failed to write config file: {ex.Message}", ex)
        End Try
    End Sub

    Public Shared Function AreCredentialsConfigured() As Boolean
        Dim creds = LoadCredentials()
        Return Not String.IsNullOrEmpty(creds.ClientId) AndAlso Not String.IsNullOrEmpty(creds.ClientSecret)
    End Function

    Public Shared Function GetConfigPath() As String
        Return ConfigPath
    End Function
End Class
