Sub MergePDFs(MyPath As String, MyFiles As String, Optional DestFile As String = "MergedFile.pdf")
' ZVI:2013-08-27 http://www.vbaexpress.com/forum/showthread.php?47310-Need-code-to-merge-PDF-files-in-a-folder-using-adobe-acrobat-X
' Reference required: VBE - Tools - References - Acrobat

    Dim a As Variant, i As Long, n As Long, ni As Long, p As String
    Dim AcroApp As New Acrobat.AcroApp, PartDocs() As Acrobat.CAcroPDDoc

    ' Adjust MyPath string if needed.
    If Right(MyPath, 1) = "\" Then p = MyPath Else p = MyPath & "\"
    a = Split(MyFiles, ",")
    ReDim PartDocs(0 To UBound(a))

    ' Save to MyPath folder if target folder for merged PDF file was not input. (ken)
    If InStr(DestFile, "\") = 0 Then DestFile = p & DestFile

    On Error GoTo exit_
    If Len(Dir(DestFile)) Then Kill DestFile
    For i = 0 To UBound(a)
        ' Check PDF file presence
        If Dir(p & Trim(a(i))) = "" Then
            MsgBox "File not found" & vbLf & p & a(i), vbExclamation, "Canceled"
            Exit For
        End If
        ' Open PDF document
        Set PartDocs(i) = CreateObject("AcroExch.PDDoc")
        PartDocs(i).Open p & Trim(a(i))
        If i Then
            ' Merge PDF to PartDocs(0) document
            ni = PartDocs(i).GetNumPages()
            If Not PartDocs(0).InsertPages(n - 1, PartDocs(i), 0, ni, True) Then
                MsgBox "Cannot insert pages of" & vbLf & p & a(i), vbExclamation, "Canceled"
            End If
            ' Calc the number of pages in the merged document
            n = n + ni
            ' Release the memory
            PartDocs(i).Close
            Set PartDocs(i) = Nothing
        Else
            ' Calc the number of pages in PartDocs(0) document
            n = PartDocs(0).GetNumPages()
        End If
    Next

    If i > UBound(a) Then
        ' Save the merged document to DestFile
        If Not PartDocs(0).Save(PDSaveFull, DestFile) Then
            MsgBox "Cannot save the resulting document" & vbLf & DestFile, vbExclamation, "Canceled"
        End If
    End If

exit_:

    ' Inform about error/success
    If Err Then
        MsgBox Err.Description, vbCritical, "Error #" & Err.Number
    ElseIf i > UBound(a) Then
        MsgBox "The resulting file was created in:" & vbLf & p & DestFile, vbInformation, "Done"
    End If

    ' Release the memory
    If Not PartDocs(0) Is Nothing Then PartDocs(0).Close
    Set PartDocs(0) = Nothing

    ' Quit Acrobat application
    AcroApp.Exit
    Set AcroApp = Nothing

End Sub