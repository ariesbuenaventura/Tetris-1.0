Imports System.Resources
Imports System.Media
Imports System.IO

Public Structure WindowRect
    Public left   As Integer
    Public top    As Integer
    Public width  As Integer
    Public height As Integer

    Public Sub WindowRect(newLeft As Integer, newTop As Integer, newWidth As Integer, newHeight As Integer)
        left   = newLeft
        top    = newTop
        width  = newWidth
        height = newHeight
    End Sub 
End Structure 

Public Structure Point
    Public x As Integer
    Public y As Integer 

    Public Sub New(newX As Integer, newY As Integer)
        x = newX
        y = newY
    End Sub 
End Structure 

Public Class Sound
    Private sndPlayer As New SoundPlayer()

    Public Sub Play(soundResId As UnmanagedMemoryStream)
        ' retrieve .wav from resource file.
        sndPlayer = new SoundPlayer(soundResId) 
        sndPlayer.Play()
    End Sub 

    Public Sub Halt()
        sndPlayer.Stop()
    End Sub

    Protected Overrides Sub Finalize()
        sndPlayer.Stop()
        sndPlayer.Dispose()
        MyBase.Finalize()
    End Sub 
End Class 