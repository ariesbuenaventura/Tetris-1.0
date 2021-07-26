' email: ariesbuenaventura2019@gmail.com

Module Program
    Private Const MAX_SPEED As Integer = 9
    Private Const DEFAULT_SPEED As Double = 0.9 ' 0.9 seconds

    Private Const msg1 As String = "   GAME OVER!!!   "
    Private Const msg2 As String = " Press any key to exit. "
    Private Const msg3 As String = " Congratulations!!! "
    Private Const msg5 As String = "PRESS ANY KEY TO START."
    Private Const msg6 As String = "PRESS ANY KEY TO CONTINUE."

    Dim Speed As Integer = 1 ' Speed of the game.
    Dim Score As Integer = 0 ' Total score.
    Dim Lines As Integer = 0 ' Total lines completed.

    Dim ptBlock As Point      ' The x and y positions of the block.
    Dim wrBlockAdj As WindowRect ' Block adjustment.
    Dim PlayWindow As WindowRect ' The size of the field.    
    Dim sndEffect As New Sound  ' Sound effect.
    Dim isRows = False       ' Is full row?
    Dim isGameExit = False       ' Terminate the game.

    Dim nextBlock As New Game.StructBlock()
    Dim currBlock As New Game.StructBlock()

    Dim kb As ConsoleKeyInfo ' keyboard input

    Dim WithEvents Tetris As Game.TetrisClass

    Sub Main()
        ' Much better to use than GetTickCount because it's more precise.
        Dim stopWatch As New Stopwatch()

        GameDesign()

        Tetris = New Game.TetrisClass(PlayWindow)

        ShowStatus()
        ' Set up the next block
        ShowNextBlock()
        ' Set up the first block
        PlayBlock(currBlock, True)

        ' Start the game.
        stopWatch.Start()
        While Not isGameExit ' continue looping until escape key has been pressed.
            Dim ts As TimeSpan = stopWatch.Elapsed
            Dim newPos As Point = ptBlock           ' Temporary variable for new position.
            Dim isCanRotate As Boolean = False

            If Console.KeyAvailable Then
                ' get key input
                kb = Console.ReadKey(True)

                Select Case kb.Key
                    Case ConsoleKey.Enter ' pause, resume
                        ' PRESS ANY KEY TO CONTINUE.
                        Console.ForegroundColor = ConsoleColor.White
                        Console.SetCursorPosition((Console.WindowWidth - msg6.Length) / 2, _
                                                   PlayWindow.height + PlayWindow.top + 2)
                        Console.Write(msg6)
                        Console.ResetColor()
                        Console.ReadKey()
                        ' clear the message from the bottom window.
                        ClearBottomLine()
                    Case ConsoleKey.LeftArrow ' move left
                        ' Could go left?
                        If PlayWindow.left < newPos.x Then
                            newPos.x -= 1
                        End If
                    Case ConsoleKey.RightArrow ' move top
                        ' Could go right?
                        If (PlayWindow.width + PlayWindow.left) > (newPos.x + wrBlockAdj.width) Then
                            newPos.x += 1
                        End If
                    Case ConsoleKey.DownArrow ' move down 
                        ' Could go down?
                        If ((PlayWindow.height + PlayWindow.top) > (newPos.y + wrBlockAdj.height)) Then
                            newPos.y += 1
                        End If
                    Case ConsoleKey.Spacebar, ConsoleKey.UpArrow ' rotate
                        Dim newBlockAdj As New WindowRect()

                        ' Save old angle.
                        Dim saveAngle As Game.RotationEnum = Tetris.Block.Angle

                        ' try clockwise
                        newBlockAdj = Tetris.Block.Rotate(Tetris.Block.getNextAngle(0))

                        If newPos.x + newBlockAdj.width > PlayWindow.width + PlayWindow.left Then
                            newPos.x = (PlayWindow.width + PlayWindow.left) - newBlockAdj.width
                        End If
                        If newPos.y + newBlockAdj.height > PlayWindow.height + PlayWindow.top Then
                            newPos.y = (PlayWindow.height + PlayWindow.top) - newBlockAdj.height
                        End If

                        If Tetris.IsCollided(New Point(newPos.x, newPos.y), newBlockAdj) Then
                            ' try counter-clockwise
                            newBlockAdj = Tetris.Block.Rotate(Tetris.Block.getNextAngle(1))

                            If newPos.x + newBlockAdj.width > PlayWindow.width + PlayWindow.left Then
                                newPos.x = (PlayWindow.width + PlayWindow.left) - newBlockAdj.width
                            End If
                            If newPos.y + newBlockAdj.height > PlayWindow.height + PlayWindow.top Then
                                newPos.x = (PlayWindow.height + PlayWindow.top) - newBlockAdj.height
                            End If

                            If Tetris.IsCollided(New Point(newPos.x, newPos.y), newBlockAdj) Then
                                isCanRotate = False
                            Else
                                isCanRotate = True
                            End If
                        Else
                            isCanRotate = True
                        End If

                        If isCanRotate Then
                            ' can rotate, apply the new settings.
                            ptBlock = newPos
                            wrBlockAdj = newBlockAdj
                        Else
                            ' can't rotate, restore old angle.
                            Tetris.Block.Rotate(saveAngle)
                        End If
                    Case ConsoleKey.Escape
                        isGameExit = True
                End Select

                If Not kb.Key.Equals(ConsoleKey.Spacebar) Then
                    If Not Tetris.IsCollided(New Point(newPos.x, newPos.y), wrBlockAdj) Then
                        ptBlock = newPos
                    End If
                End If

                If ts.TotalSeconds < (DEFAULT_SPEED - (Speed - 1) / 10.0) Then
                    Tetris.Block.Draw(ptBlock, wrBlockAdj, isCanRotate)
                End If
            End If

            If ts.TotalSeconds >= (DEFAULT_SPEED - (Speed - 1) / 10.0) Then
                If (PlayWindow.height + PlayWindow.top) > (ptBlock.y + wrBlockAdj.height) Then
                    If Tetris.IsCollided(New Point(ptBlock.x, ptBlock.y + 1), wrBlockAdj) Then
                        ' The block has collided, set the next block.
                        PlayBlock(nextBlock, False)
                    Else
                        ' move down
                        ptBlock.y += 1
                    End If
                Else
                    ' Were at the bottom, set the next block.
                    PlayBlock(nextBlock, False)
                End If

                Tetris.Block.Draw(ptBlock, wrBlockAdj, isCanRotate)
                stopWatch.Reset()
                stopWatch.Start()
            End If
        End While

        stopWatch.Stop()

        ' Press any key to exit.
        Console.ForegroundColor = ConsoleColor.White
        Console.SetCursorPosition((Console.WindowWidth - msg2.Length) / 2, _
                                   Console.WindowHeight / 2)
        Console.Write(msg2)
        Console.ReadKey()
        Console.ResetColor()

        sndEffect.Halt()
    End Sub

    Private Sub Tetris_ProcessEvent(ByVal RowsCompleted As Object) Handles Tetris.ProcessEvent
        If RowsCompleted > 0 Then
            isRows = True
            If RowsCompleted > 1 Then
                Score += 15
            Else
                Score += 10
            End If

            Lines += RowsCompleted

            ' Increase the speed according to the number of lines completed.
            If (Lines >= 11) And (Lines <= 20) Then
                Speed = 2
            ElseIf ((Lines >= 21) And (Lines <= 30)) Then
                Speed = 3
            ElseIf ((Lines >= 31) And (Lines <= 40)) Then
                Speed = 4
            ElseIf ((Lines >= 41) And (Lines <= 50)) Then
                Speed = 5
            ElseIf ((Lines >= 51) And (Lines <= 60)) Then
                Speed = 6
            ElseIf ((Lines >= 61) And (Lines <= 70)) Then
                Speed = 7
            ElseIf ((Lines >= 71) And (Lines <= 80)) Then
                Speed = 8
            ElseIf ((Lines >= 81) And (Lines <= 90)) Then
                Speed = 9
            End If

            ShowStatus()
        End If
    End Sub

    Sub ShowStatus()
        Console.ForegroundColor = ConsoleColor.White
        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 3, 2)
        Console.Write("Score")
        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 3, 5)
        Console.Write("Lines")
        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 3, 17)
        Console.Write("Speed")
        Console.ResetColor()

        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 2, 3)
        Console.Write(String.Format("{0:D8}", Score))
        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 2, 6)
        Console.Write(String.Format("{0:D8}", Lines))
        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 5, 18)
        Console.Write(String.Format("{0:D2}", Speed))

        If Lines >= 100 Then
            ' CONGRATULATIONS!!!
            Console.ForegroundColor = ConsoleColor.White
            Console.SetCursorPosition((Console.WindowWidth - msg3.Length) / 2, _
                                       Console.WindowHeight / 2)
            Console.Write(msg3)
            Console.ReadKey()
            Console.ResetColor()

            sndEffect.Play(Global.prjTetris.My.Resources.S100)
            isGameExit = True
        End If
    End Sub

    Sub ShowNextBlock()
        nextBlock = Tetris.Block.Generate() ' get next block

        Console.ForegroundColor = ConsoleColor.White
        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 4, 8)
        Console.Write("Next")
        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 2, 9)
        Console.Write("¤¤¤¤¤¤¤¤")

        For i As Integer = 1 To 6
            Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 2, i + 9)
            Console.Write("¤      ¤")
        Next i

        Console.SetCursorPosition(PlayWindow.width + PlayWindow.left + 2, 15)
        Console.Write("¤¤¤¤¤¤¤¤")
        Console.ResetColor()

        Tetris.Block.Preview(New Point(PlayWindow.width + PlayWindow.left + 6, 12), nextBlock)
    End Sub

    Sub PlayBlock(ByVal sbBlock As Game.StructBlock, ByVal isNew As Boolean)
        If isNew Then
            ' create new block
            sbBlock = Tetris.Block.Generate()
        Else
            Tetris.SendToField(ptBlock, wrBlockAdj)
        End If

        Tetris.Block.Assign(sbBlock)
        Tetris.Block.Build()
        Tetris.Block.Adjustment(wrBlockAdj)

        ptBlock.x = (PlayWindow.left - wrBlockAdj.left) + (PlayWindow.width - wrBlockAdj.width) / 2
        ptBlock.y = PlayWindow.top

        Tetris.Block.Draw(ptBlock, wrBlockAdj, True)
        ShowNextBlock()

        If Tetris.IsCollided(ptBlock, wrBlockAdj) Then
            sndEffect.Play(Global.prjTetris.My.Resources.S102)
            Console.SetCursorPosition((Console.WindowWidth - msg3.Length) / 2, _
                                       Console.WindowHeight / 2)
            Console.Write(msg3)

            isGameExit = True
        Else
            If isRows Then
                ' rows completed
                sndEffect.Play(Global.prjTetris.My.Resources.S103)
                isRows = False
            Else
                sndEffect.Play(Global.prjTetris.My.Resources.S100)
            End If
        End If
    End Sub

    Sub GameDesign()
        Const dsgnTB As String = "▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒"
        Const dsgnLR As String = "▒              ▒"

        ' Define window size.
        PlayWindow.left = 33
        PlayWindow.width = dsgnTB.Length - 2
        PlayWindow.top = 2
        PlayWindow.height = 19

        ' hide the cursor
        Console.CursorVisible = False
        ' help
        Console.SetCursorPosition(1, 1)
        Console.Write("LEFT, RIGHT, DOWN -> Move")
        Console.SetCursorPosition(1, 2)
        Console.Write("UP, SPACE BAR -> Rotate")
        Console.SetCursorPosition(1, 3)
        Console.Write("ENTER -> PAUSE, RESUME")

        ' draw the top border
        Console.SetCursorPosition(PlayWindow.left - 1, PlayWindow.top - 1)
        Console.Write(dsgnTB)
        ' draw the bottom border
        Console.SetCursorPosition(PlayWindow.left - 1, PlayWindow.top + PlayWindow.height)
        Console.Write(dsgnTB)

        ' draw the left and right border
        For i As Integer = PlayWindow.top To PlayWindow.height + PlayWindow.top - 1
            Console.SetCursorPosition(PlayWindow.left - 1, i)
            Console.Write(dsgnLR)
        Next i

        sndEffect.Play(Global.prjTetris.My.Resources.S101)

        ' PRESS ANY KEY TO START.
        Console.ForegroundColor = ConsoleColor.White
        Console.SetCursorPosition((Console.WindowWidth - msg5.Length) / 2, _
                                   PlayWindow.height + PlayWindow.top + 2)
        Console.Write(msg5)
        Console.ResetColor()
        Console.ReadKey()

        ' clear the message from the bottom window
        ClearBottomLine()
    End Sub

    Sub ClearBottomLine()
        For i As Integer = 1 To Console.WindowWidth - 1
            Console.SetCursorPosition(i, PlayWindow.height + PlayWindow.top + 2)
            Console.Write(" ")
        Next i
    End Sub
End Module



