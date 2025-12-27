Imports System.Net.Http
Imports HtmlAgilityPack
Imports System.Text.RegularExpressions
Imports System.Text

Public Class YouTubeSearchService
    Private Shared ReadOnly client As New HttpClient()

    Public Class YouTubeResult
        Public Property Title As String
        Public Property Url As String
        Public Property VideoId As String
        Public Property ThumbnailUrl As String
        Public Property Duration As String
        Public Property Channel As String
    End Class

    Public Shared Async Function SearchAsync(query As String) As Task(Of List(Of YouTubeResult))
        If String.IsNullOrWhiteSpace(query) Then
            Return New List(Of YouTubeResult)()
        End If

        Try
            Dim searchUrl = $"https://www.youtube.com/results?search_query={Uri.EscapeDataString(query)}"

            ' Setting da user agent to mimic a browser so yt doesnt YEET your requests.
            client.DefaultRequestHeaders.Clear()
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36")

            Dim response = Await client.GetAsync(searchUrl)
            response.EnsureSuccessStatusCode()

            Dim content = Await response.Content.ReadAsStringAsync()

            Return ParseSearchResults(content)

        Catch ex As Exception
            Console.WriteLine($"Search error: {ex.Message}")
            Return New List(Of YouTubeResult)()
        End Try
    End Function

    Private Shared Function ParseSearchResults(html As String) As List(Of YouTubeResult)
        Dim results = New List(Of YouTubeResult)()

        Try

            Dim jsonMatch = Regex.Match(html, "var ytInitialData = ({.*?});</script>", RegexOptions.Singleline)

            If jsonMatch.Success Then
                Dim jsonData = jsonMatch.Groups(1).Value
                results = ExtractVideosFromJson(jsonData)
            End If

        Catch ex As Exception
            Console.WriteLine($"Parse error: {ex.Message}")
        End Try

        Return results.Take(10).ToList() ' rtn top 10
    End Function

    Private Shared Function ExtractVideosFromJson(json As String) As List(Of YouTubeResult)
        Dim results = New List(Of YouTubeResult)()
        Dim foundVideoIds = New HashSet(Of String)()

        Try
            ' regex extracting results 
            Dim videoRendererMatches = Regex.Matches(json, """videoRenderer"":\{[^}]*""videoId"":""([a-zA-Z0-9_-]{11})""[^}]*""title"":\{[^}]*?""simpleText"":""([^""]+)""", RegexOptions.IgnoreCase)

            For Each match In videoRendererMatches
                Dim videoId = match.Groups(1).Value
                Dim title = match.Groups(2).Value

                ' Skip if we've already added this video (avoid duplicates)
                If foundVideoIds.Contains(videoId) Then
                    Continue For
                End If

                ' Skip ads
                If title.Contains("Ad") Then
                    Continue For
                End If

                foundVideoIds.Add(videoId)

                results.Add(New YouTubeResult With {
                    .VideoId = videoId,
                    .Title = DecodeHtmlEntities(title),
                    .Url = $"https://www.youtube.com/watch?v={videoId}",
                    .ThumbnailUrl = $"https://img.youtube.com/vi/{videoId}/default.jpg"
                })

                If results.Count >= 10 Then Exit For
            Next

            '  if we didn't get enough results, try simpler pattern
            If results.Count < 3 Then
                results.Clear()
                foundVideoIds.Clear()

                Dim simpleMatches = Regex.Matches(json, """videoId"":""([a-zA-Z0-9_-]{11})""", RegexOptions.IgnoreCase)
                Dim titleMatches = Regex.Matches(json, """title"":\{[^}]*""simpleText"":""([^""]+)""", RegexOptions.IgnoreCase)

                For i As Integer = 0 To Math.Min(simpleMatches.Count, titleMatches.Count) - 1
                    Dim videoId = simpleMatches(i).Groups(1).Value
                    Dim title = titleMatches(i).Groups(1).Value

                    If foundVideoIds.Contains(videoId) OrElse videoId.Length <> 11 Then
                        Continue For
                    End If

                    If title.Contains("Ad") Then
                        Continue For
                    End If

                    foundVideoIds.Add(videoId)

                    results.Add(New YouTubeResult With {
                        .VideoId = videoId,
                        .Title = DecodeHtmlEntities(title),
                        .Url = $"https://www.youtube.com/watch?v={videoId}",
                        .ThumbnailUrl = $"https://img.youtube.com/vi/{videoId}/default.jpg"
                    })

                    If results.Count >= 10 Then Exit For
                Next
            End If

        Catch ex As Exception
            Console.WriteLine($"JSON extraction error: {ex.Message}")
        End Try

        Return results
    End Function

    Private Shared Function DecodeHtmlEntities(text As String) As String
        If String.IsNullOrEmpty(text) Then
            Return text
        End If

        ' Decode html
        Return System.Net.WebUtility.HtmlDecode(text)
    End Function

    Public Shared Function GetYouTubeUrl(videoId As String) As String
        Return $"https://www.youtube.com/watch?v={videoId}"
    End Function

    Public Shared Sub OpenInBrowser(url As String)
        Try
            Dim psi = New System.Diagnostics.ProcessStartInfo With {
                .FileName = url,
                .UseShellExecute = True
            }
            System.Diagnostics.Process.Start(psi)
        Catch ex As Exception
            Console.WriteLine($"Error opening browser: {ex.Message}")
        End Try
    End Sub
End Class
