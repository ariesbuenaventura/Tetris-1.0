Public Class Game
    Enum RotationEnum
        deg0   = 0
        deg90  = 1
        deg180 = 2
        deg270 = 3
    End Enum 

    Enum BlockTypeNum
        block01 = 0
        block02 = 1
        block03 = 2
        block04 = 3
        block05 = 4
        block06 = 5
        block07 = 6
    End Enum 

    Structure StructBlock
        Public angle As RotationEnum
        public type  As BlockTypeNum

        Public Sub New(newAngle As RotationEnum, newType As BlockTypeNum)
            angle = newAngle
            type  = newType
        End Sub 
    End Structure 
    
    Structure StructBlockStyle
        Public color   As ConsoleColor
        Public isBlock As Boolean

        Public Sub New(newColor As ConsoleColor, newIsBlock As Boolean)
            color   = newColor
            isBlock = newIsBlock
        End Sub 
    End Structure 

    Public Class BaseClass
        Protected Const BLOCK_SIZE As Integer = 4 ' size of the block(4v4)
        
        Protected Shared arrBlock(BLOCK_SIZE<<2) As Boolean 
        Protected Shared TetrisField             As WindowRect
        Protected Shared m_blockpos              As Point
        Protected Shared m_block                 As StructBlock
        Protected Shared arrField()              As StructBlockStyle

        Public Sub New()
            m_blockpos = New Point(-1, -1)
        End Sub
    End Class 

    Public Class BlockClass 
        Inherits BaseClass

        Private newPropertyValue As String

        Public Property Angle() As RotationEnum
            Get
                Return m_block.angle 
            End Get

            Set(ByVal value As RotationEnum)
                m_block.angle = value
            End Set
        End Property

        Public Property Type() As BlockTypeNum
            Get
                Return m_block.type
            End Get 

            Set
                m_block.type = value
            End Set 
        End Property 

        Public Property Location() As Point
            Get 
                Return New Point(m_blockpos.x, m_blockpos.y)
            End Get 

            Set
                m_blockpos = value
            End Set 
        End Property 

        Public ReadOnly Property Size() As Integer 
            Get 
                return BLOCK_SIZE
            End Get 
        End Property 

        Public Function Color(typBlock As BlockTypeNum) As ConsoleColor
            ' this function returns the color of the block.
            Select Case typBlock 
                Case BlockTypeNum.block01:
                    Return ConsoleColor.Red
                Case BlockTypeNum.block02:
                    Return ConsoleColor.Blue
                Case BlockTypeNum.block03:
                    Return ConsoleColor.Cyan
                Case BlockTypeNum.block04:
                    Return ConsoleColor.Yellow
                Case BlockTypeNum.block05:
                    Return ConsoleColor.Green
                Case BlockTypeNum.block06:
                    Return ConsoleColor.Magenta
                Case Else
                    Return ConsoleColor.DarkCyan
            End Select
        End Function 

        Public Function Generate() As StructBlock
            Dim rnd As New Random()

            ' pick random pieces
            Return New StructBlock(rnd.Next(0, [Enum].GetNames(GetType(RotationEnum)).Length), _
                                   rnd.Next(0, [Enum].GetNames(GetType(BlockTypeNum)).Length))
        End Function 

        Public Function Rotate(newAngle As RotationEnum) As WindowRect
            Dim wrBlock As New WindowRect()

            Angle = newAngle
            Build()
            Adjustment(wrBlock)
           
            return wrBlock
        End Function 

        Public Sub Build()
            ' Get the data for the block.
            arrBlock = GetBlockData(New StructBlock(Angle, Type))
        End Sub 

        Public Function GetBlockData(structBlock As StructBlock) As Boolean()
            ' 0123
            ' 4567
            ' 8901
            ' 2345

            ' data for 4v4 block shapes
            Dim arrData(BLOCK_SIZE<<2) As Boolean 
            
            Select Case structBlock.type
                Case BlockTypeNum.block01:
                    If structBlock.angle.Equals(RotationEnum.deg0) Or _
                       structBlock.angle.Equals(RotationEnum.deg180) Then
                        arrData(2)  = true ' ..#. 0123
                        arrData(6)  = true ' ..#. 4567
                        arrData(10) = true ' ..#. 8901
                        arrData(14) = true ' ..#. 2345
                    Else
                        arrData(12) = true ' .... 0123
                        arrData(13) = true ' .... 4567
                        arrData(14) = true ' .... 8901
                        arrData(15) = true ' #### 2345
                    End If 
                Case BlockTypeNum.block02:
                    arrData(0)  = true ' ##.. 0123
                    arrData(1)  = true ' ##.. 4567
                    arrData(4)  = true ' .... 8901
                    arrData(5)  = true ' .... 2345
                Case BlockTypeNum.block03:
                    If structBlock.angle.Equals(RotationEnum.deg0) Or _
                       structBlock.angle.Equals(RotationEnum.deg180) Then
                        arrData(5)  = true ' .... 0123
                        arrData(6)  = true ' .##. 4567
                        arrData(8)  = true ' ##.. 8901
                        arrData(9)  = true ' .... 2345
                    Else
                        arrData(1)  = true ' .#.. 0123
                        arrData(5)  = true ' .##. 4567
                        arrData(6)  = true ' ..#. 8901
                        arrData(10) = true ' .... 2345
                    End If 
                Case BlockTypeNum.block04:
                    If structBlock.angle.Equals(RotationEnum.deg0) Or _
                       structBlock.angle.Equals(RotationEnum.deg180) Then
                        arrData(4)  = true ' .... 0123
                        arrData(5)  = true ' ##.. 4567
                        arrData(9)  = true ' .##. 8901
                        arrData(10) = true ' .... 2345
                    Else
                        arrData(2)  = true ' ..#. 0123
                        arrData(5)  = true ' .##. 4567
                        arrData(6)  = true ' .#.. 8901
                        arrData(9)  = true ' .... 2345
                    End If 
                Case BlockTypeNum.block05:
                    If structBlock.angle.Equals(RotationEnum.deg0) Then
                        arrData(4)  = true ' .... 0123
                        arrData(5)  = true ' ###. 4567
                        arrData(6)  = true ' .#.. 8901
                        arrData(9)  = true ' .... 2345
                    Else If structBlock.angle.Equals(RotationEnum.deg90) Then
                        arrData(1)  = true ' .#.. 0123
                        arrData(4)  = true ' ##.. 4567
                        arrData(5)  = true ' .#.. 8901
                        arrData(9)  = true ' .... 2345
                    Else If structBlock.angle.Equals(RotationEnum.deg180) Then
                        arrData(5)  = true ' .... 0123
                        arrData(8)  = true ' .#.. 4567
                        arrData(9)  = true ' ###. 8901
                        arrData(10) = true ' .... 2345
                    Else
                        arrData(1)  = true ' .#.. 0123
                        arrData(5)  = true ' .##. 4567
                        arrData(6)  = true ' .#.. 8901
                        arrData(9)  = true ' .... 2345
                    End If 
                Case BlockTypeNum.block06:
                    If structBlock.angle.Equals(RotationEnum.deg0)
                        arrData(4)  = true ' .... 0123
                        arrData(5)  = true ' ###. 4567
                        arrData(6)  = true ' #... 8901
                        arrData(8)  = true ' .... 2345
                    Else If structBlock.angle.Equals(RotationEnum.deg90) Then
                        arrData(0)  = true ' ##.. 0123
                        arrData(1)  = true ' .#.. 4567
                        arrData(5)  = true ' .#.. 8901
                        arrData(9)  = true ' .... 2345
                    Else If structBlock.angle.Equals(RotationEnum.deg180) Then
                        arrData(6)  = true ' .... 0123
                        arrData(8)  = true ' ..#. 4567
                        arrData(9)  = true ' ###. 8901
                        arrData(10) = true ' .... 2345
                    Else
                        arrData(1)  = true ' .#.. 0123
                        arrData(5)  = true ' .#.. 4567
                        arrData(9)  = true ' .##. 8901
                        arrData(10) = true ' .... 2345
                    End If 
                Case BlockTypeNum.block07:
                    If structBlock.angle.Equals(RotationEnum.deg0) Then
                        arrData(4)  = true ' .... 0123
                        arrData(5)  = true ' ###. 4567
                        arrData(6)  = true ' ..#. 8901
                        arrData(10) = true ' .... 2345
                    Else If structBlock.angle.Equals(RotationEnum.deg90) Then
                        arrData(1)  = true ' .#.. 0123
                        arrData(5)  = true ' .#.. 4567
                        arrData(8)  = true ' ##.. 8901
                        arrData(9)  = true ' .... 2345
                    Else If structBlock.angle.Equals(RotationEnum.deg180) Then
                        arrData(4)  = true ' .... 0123
                        arrData(8)  = true ' #... 4567
                        arrData(9)  = true ' ###. 8901
                        arrData(10) = true ' .... 2345
                    Else
                        arrData(1)  = true ' .##. 0123
                        arrData(2)  = true ' .#.. 4567
                        arrData(5)  = true ' .#.. 8901
                        arrData(9)  = true ' .... 2345
                    End If 
            End Select

            Return arrData
        End Function 

        Public Sub Adjustment(ByRef wrBlock As WindowRect)
            Adjustment(wrBlock, arrBlock)
        End Sub 

        Public Sub Adjustment(ByRef wrBlock As WindowRect, arrData() As Boolean)
            '  This function returns the exact measurement of the block. 
            
            wrBlock = new WindowRect()

            Dim col   As Integer 
            Dim row   As Integer 
            Dim isAdj As Boolean 

            '  Check empty colums from the left-side of the block, and if found, 
            ' increase the left margin.
            isAdj = True
            For col = 0 To BLOCK_SIZE-1
                For row = 0 To BLOCK_SIZE-1
                    If arrData(col+row*BLOCK_SIZE) Then
                        isAdj = false
                        Exit For
                    End If 
                Next row

                If isAdj Then
                    ' left margin
                    wrBlock.left+=1
                Else
                    Exit For
                End If
            Next col
            ' end left adjustment

            '  Check empty rows from the top-side of the block, and if found, 
            ' increse the top margin. 
            isAdj = true
            For row = 0 To BLOCK_SIZE-1
                For col = 0 to BLOCK_SIZE-1
                    If arrData(col+row*BLOCK_SIZE)
                        isAdj = false
                        Exit For
                    End If 
                Next col

                If isAdj Then
                    wrBlock.top+=1
                Else
                    Exit For
                End If
            Next row
            ' end top adjustment

            '  Check empty columns from the right-side of the block, and if found, 
            ' increase the right margin.
            isAdj = true
            For col=BLOCK_SIZE-1 To 0 Step -1
                For row = 0 To BLOCK_SIZE-1
                    If arrData(col+row*BLOCK_SIZE) Then
                        isAdj = False
                        Exit For
                    End If 
                Next row
                
                If isAdj Then
                    wrBlock.width+=1
                Else
                    Exit For
                End If
            Next col    
            
            ' get the exact width of the block
            wrBlock.width = BLOCK_SIZE - (wrBlock.left+wrBlock.width)
            ' end right adjustment

            '  Check empty rows from the bottom-side of the block, and if found, 
            ' increase the bottom.
            isAdj = true
            For row = BLOCK_SIZE-1 To 0 Step -1
                For col = 0 To BLOCK_SIZE-1 
                    If arrData(col+row*BLOCK_SIZE) Then
                        isAdj = False
                        Exit For
                    End If
                Next col

                If isAdj Then
                    ' bottom margin
                    wrBlock.height+=1
                Else
                    Exit For
                End If
            Next row

            ' get the exact height of the block.
            wrBlock.height = BLOCK_SIZE - (wrBlock.top+wrBlock.height)
            ' end top adjustment
        End Sub

        Public Sub Draw(pt As Point, wrBlockAdj As WindowRect, isRotateUpdate As Boolean)
            ' Draw the block.
            If (Not Location.x.Equals(pt.x)) Or Not (Location.y.Equals(pt.y)) Or isRotateUpdate Then
                TetrisClass.DrawField(pt, wrBlockAdj)
                Console.ForegroundColor = Color(Type)
                For row As Integer = wrBlockAdj.top To wrBlockAdj.top+wrBlockAdj.height-1
                    For col As Integer = wrBlockAdj.left To wrBlockAdj.left+wrBlockAdj.width-1
                        If arrBlock(col+row*BLOCK_SIZE) Then
                            Console.SetCursorPosition(pt.x+col-wrBlockAdj.left, pt.y+row-wrBlockAdj.top)
                            Console.Write("#")
                        End If
                    Next col
                Next row
                Console.ResetColor()

                Location = pt
            End If 
        End Sub 

        Public Sub Preview(pt As Point, structBlock As StructBlock)
            ' shows a preview of a block
            Dim wrBlockAdj = New WindowRect()
            Dim arrData()  = GetBlockData(structBlock)
            
            '  retrieve the exact measurement of the block
            ' so we can able to draw the block in correct position.
            Adjustment(wrBlockAdj, arrData)

            Console.ForegroundColor = Color(structBlock.type)
                For row As Integer = wrBlockAdj.top To wrBlockAdj.top+wrBlockAdj.height-1
                    For col As Integer = wrBlockAdj.left To wrBlockAdj.left+wrBlockAdj.width-1
                    If arrData(col+row*BLOCK_SIZE) Then
                        Console.SetCursorPosition(pt.x+col-wrBlockAdj.left-wrBlockAdj.width\2, _
                                                  pt.y+row-wrBlockAdj.top-wrBlockAdj.height\2)
                        Console.Write("#")
                    End If
                Next col
            Next row

            Console.ResetColor()
        End Sub 

        Public Function getNextAngle(rotateOption As Integer) As RotationEnum
            If rotateOption.Equals(0) Then
                ' clockwise
                Select Case Angle
                    Case RotationEnum.deg0:
                        Return RotationEnum.deg90
                    Case RotationEnum.deg90:
                        Return RotationEnum.deg180
                    Case RotationEnum.deg180:
                        Return RotationEnum.deg270
                    Case Else
                        Return RotationEnum.deg0
                    End Select
            Else
                ' counter-clockwise
                Select Case Angle
                    Case RotationEnum.deg0:
                        Return RotationEnum.deg270
                    Case RotationEnum.deg270:
                        Return RotationEnum.deg180
                    Case RotationEnum.deg180:
                        Return RotationEnum.deg90
                    Case Else 
                        Return RotationEnum.deg0
                End Select 
            End If 
        End Function 

        Public Sub Assign(sbNew As StructBlock)
            Angle = sbNew.angle
            Type  = sbNew.type   
        End Sub 
    End Class 

    Public Class TetrisClass 
        Inherits BaseClass

        Public Shared Event ProcessEvent(Byval RowsCompleted)

        Public Block As New BlockClass()

        Public Sub New(wrField As WindowRect)
            TetrisField = wrField
            BuildField()
        End Sub

        Public Sub BuildField()
            Redim arrField(TetrisField.width*TetrisField.height) 
        End Sub 

        Public Shared Sub DrawField(pt As Point, wrBlockAdj As WindowRect)
            Dim w As Integer = TetrisField.width
            Dim h As Integer = TetrisField.height

            For row As Integer = 0 To h-1
                For col As Integer = 0 to w-1
                    If CType(arrField(col+row*w), StructBlockStyle).isBlock Then
                        Console.ForegroundColor = CType(arrField(col+row*w), StructBlockStyle).color
                        Console.SetCursorPosition(TetrisField.left+col, TetrisField.top+row)
                        Console.Write("@")
                    Else
                        Console.SetCursorPosition(TetrisField.left+col, TetrisField.top+row)
                        Console.Write(" ")
                    End If
                Next col
            Next row

            Console.ResetColor()
        End Sub

        Public Function IsCollided(pt As Point, wrBlockAdj As WindowRect) As Boolean
            Dim sx As integer = pt.x - TetrisField.left
            Dim sy As Integer = pt.y - TetrisField.top
            Dim w  As Integer = TetrisField.width

            Dim blockIndex As Integer 
            Dim fieldIndex As integer

            For row As Integer = 0 To wrBlockAdj.height-1
                For col As Integer = 0 To wrBlockAdj.width-1
                    blockIndex = (wrBlockAdj.left+col)+((wrBlockAdj.top+row)*BLOCK_SIZE)
                    fieldIndex = ((sx+sy*w)+col)+row*w

                    If arrBlock(blockIndex) And (CType(arrField(fieldIndex), StructBlockStyle).isBlock)
                       Return True
                    End If 
                Next col
            Next row

            Return False
        End Function 

        Public Sub SendToField(pt As Point, wrBlockAdj As WindowRect)
            ' This function sends the block data to field.
            Dim blockIndex As Integer 
            Dim fieldIndex As Integer 

            For row As Integer = 0 To wrBlockAdj.height-1
                For col As Integer = 0 To wrBlockAdj.width-1
                    blockIndex = (wrBlockAdj.left+col)+ _
                                 (wrBlockAdj.top+row)* _
                                  Block.Size
                    fieldIndex = (pt.x-TetrisField.left+col)+ _
                                 (pt.y-TetrisField.top+row)* _
                                  TetrisField.width

                    If arrBlock(blockIndex)
                        arrField(fieldIndex) = New StructBlockStyle(Block.Color(Block.Type), True)
                    End If 
                Next col
            Next row

            ProcessRows()
        End Sub 

        Public Sub ProcessRows()
            ' This function check to see if rows were completed.
            Dim w          As Integer = TetrisField.width
            Dim h          As Integer = TetrisField.height
            Dim rowCounter As Integer = h-1
            Dim rowTotal   As Integer = 0
            Dim isFullLine As Boolean = True

            ' Store rows that are not completed.
            Dim arrData(TetrisField.width*TetrisField.height) As StructBlockStyle

            For row As Integer = h-1 To 0 Step -1
                For col As Integer = w-1 To 0 Step -1
                    If Not (CType(arrField(col+row*w), StructBlockStyle).isBlock) Then
                        isFullLine = False
                        Exit For 
                    End If
                Next col

                If Not isFullLine Then
                    ' copy the row
                    For col As Integer = w-1 To 0 Step -1
                        arrData(col+rowCounter*w) = arrField(col+row*w)
                    Next col

                    rowCounter-=1
                    isFullLine = True
                Else
                    ' Do not include rows that are completed.
                    rowTotal+=1
                End If
            Next row

            ' get all the rows that are not completed.
            arrField = arrData 

            RaiseEvent ProcessEvent(rowTotal)
        End Sub
    End Class 
End Class

