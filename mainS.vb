Imports SpeechLib
Imports System.Text.RegularExpressions

Public Class mainS
    Friend caller As pluginS
    Dim nf As NextFEMapi.API
    Public WithEvents vox As New SpVoice
    Dim lang As String = "en"
    Private Sub mainS_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' choose voice
        nf = caller.API

        Dim langDict As New Dictionary(Of String, Integer)
        lang = nf.getLanguage
        Dim vcs As ISpeechObjectTokens = vox.GetVoices()
        For i = 0 To vcs.Count - 1
            Dim st As ISpeechObjectToken = vcs(i)
            If st.GetDescription.Contains("Zira") Or st.GetDescription.Contains("David") Or st.GetDescription.Contains("Mark") _
                Or st.GetDescription.Contains("George") Or st.GetDescription.Contains("Hazel") Or st.GetDescription.Contains("Susan") Or
                st.GetDescription.Contains("James") Or st.GetDescription.Contains("Shaun") Or st.GetDescription.Contains("Richard") Or
                st.GetDescription.Contains("Catherine") Or st.GetDescription.Contains("Linda") Or st.GetDescription.Contains("Ravi") Or
                st.GetDescription.Contains("Heera") Then
                If Not langDict.ContainsKey("en") Then langDict.Add("en", i)
            ElseIf st.GetDescription.Contains("Elsa") Or st.GetDescription.Contains("Cosimo") Then
                If Not langDict.ContainsKey("it") Then langDict.Add("it", i)
            ElseIf st.GetDescription.Contains("Pablo") Or st.GetDescription.Contains("Helena") Or
                st.GetDescription.Contains("Raul") Or st.GetDescription.Contains("Sabina") Or st.GetDescription.Contains("Laura") Then
                If Not langDict.ContainsKey("es") Then langDict.Add("es", i)
            End If
        Next
        ' re-assign voice upon choice
        If langDict.ContainsKey(lang) Then
            vox.Voice = vcs.Item(langDict(lang)) ' riassegna voce

            text2sp.Text = ""
            ' compile text
            Select Case lang
                Case "en"
                    text2sp.Text = "Element connected to nodes "
                    ' connected nodes
                    text2sp.Text &= "##text.elementconnectivity(§selElement§)##" ' String.Join(" and ", nf.getElementConnectivity(st))
                Case "it"
                    text2sp.Text = "Elemento connesso ai nodi "
                    ' connected nodes
                    text2sp.Text &= "##text.elementconnectivity(§selElement§)##" ' String.Join(" e ", nf.getElementConnectivity(st))
                Case "es"
                    text2sp.Text = "Elemento conectado a los nodos "
                    ' connected nodes
                    text2sp.Text &= "##text.elementconnectivity(§selElement§)##" ' String.Join(" y ", nf.getElementConnectivity(st))
                Case Else
                    GoTo usc
            End Select
        Else
usc:        MsgBox("No suitable voice found, exiting", MsgBoxStyle.Critical, caller.PluginName)
            Me.Close()
        End If

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            vox.Speak("", SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak)
        Catch ex As Exception
        End Try
    End Sub
    Dim kw As String = "##"
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        ' compile text for selected elements
        Dim nds() As String = nf.selectedNodes
        Dim els() As String = nf.selectedElements
        If els.Length = 0 And nds.Length = 0 Then
            MsgBox("Select element(s) to proceed and turn audio on", MsgBoxStyle.Information, caller.PluginName)
            Exit Sub
        End If
        Dim str As String = text2sp.Text

        ' replace els(0) and nds(0)
        If nds.Length > 0 Then str = str.Replace("§selNode§", nds(0))
        If els.Length > 0 Then str = str.Replace("§selElement§", els(0))

        ' keys
        Dim stt As String = str ' stringa di lavoro
        For Each lr In Regex.Matches(str, kw & ".*?" & kw, RegexOptions.IgnoreCase)
            Dim ls As String = lr.ToString.Replace(kw, "")
            If ls = "text.name" Then
                stt = stt.Replace(kw & ls & kw, nf.ModelName)
            ElseIf ls = "text.nodesnumber" Then
                stt = stt.Replace(kw & ls & kw, nf.nodesList.Count)
            ElseIf ls = "text.elementsnumber" Then
                stt = stt.Replace(kw & ls & kw, nf.elemsList.Count)
            ElseIf ls = "text.length" Then
                stt = stt.Replace(kw & ls & kw, nf.getLenUnit)
            ElseIf ls = "text.force" Then
                stt = stt.Replace(kw & ls & kw, nf.getForceUnit)
            ElseIf ls.StartsWith("text.beamdiagram") Then
                Dim ss() As String = ls.Replace("text.beamdiagram(", "").Replace(")", "").Split(","c)
                Dim eleForces As String = nf.getBeamForce(ss(0), ss(1), ss(2), ss(3), ss(4))
                If eleForces Is Nothing Then Exit Sub
                stt = stt.Replace(kw & ls & kw, eleForces)
            ElseIf ls.StartsWith("text.elementconnectivity") Then
                Dim ss() As String = ls.Replace("text.elementconnectivity(", "").Replace(")", "").Split(","c)
                Dim eleConn() As String = nf.getElementConnectivity(ss(0))
                If eleConn Is Nothing Then Exit Sub
                stt = stt.Replace(kw & ls & kw, String.Join(" ", eleConn))
            End If
        Next
        Try
            vox.Speak(stt, SpeechVoiceSpeakFlags.SVSFlagsAsync)
        Catch ex As Exception
            MsgBox(stt & vbNewLine & ex.Message & vbNewLine & ex.StackTrace)
        End Try

    End Sub
End Class