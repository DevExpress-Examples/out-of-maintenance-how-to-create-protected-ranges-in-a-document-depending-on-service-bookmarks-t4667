Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraBars.Ribbon
Imports DevExpress.XtraRichEdit
Imports DevExpress.XtraRichEdit.API.Layout
Imports DevExpress.XtraRichEdit.API.Native

Namespace WindowsFormsApplication1
	Partial Public Class Form1
		Inherits RibbonForm
		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
			AddHandler richEditControl1.BeforePagePaint, AddressOf richEditControl1_BeforePagePaint

			richEditControl1.LoadDocument("testDocument.rtf")
			GenerateBookmarks(richEditControl1.Document)
			CreateProtectedRanges(richEditControl1.Document)
		End Sub

		Private Sub CreateProtectedRanges(ByVal document As Document)
			Dim lastNonProtectedPosition As DocumentPosition = document.Range.Start
			Dim containsProtectedRanges As Boolean = False
			Dim rangePermissions As RangePermissionCollection = richEditControl1.Document.BeginUpdateRangePermissions()
			For i As Integer = 0 To document.Bookmarks.Count - 1
				Dim protectedBookmark As Bookmark = document.Bookmarks(i)
				If protectedBookmark.Name.Contains("protectedRange") Then
					containsProtectedRanges = True

					rangePermissions.AddRange(CreateRangePermissions(protectedBookmark.Range, "Admin", "Admin"))
					If protectedBookmark.Range.Start.ToInt() > lastNonProtectedPosition.ToInt() Then
						Dim rangeAfterProtection As DocumentRange = richEditControl1.Document.CreateRange(lastNonProtectedPosition, protectedBookmark.Range.Start.ToInt() - lastNonProtectedPosition.ToInt() - 1)
						rangePermissions.AddRange(CreateRangePermissions(rangeAfterProtection, "User", "User"))
					End If
					lastNonProtectedPosition = protectedBookmark.Range.End
				End If
			Next i

			If document.Range.End.ToInt() > lastNonProtectedPosition.ToInt() Then
				Dim rangeAfterProtection As DocumentRange = richEditControl1.Document.CreateRange(lastNonProtectedPosition, document.Range.End.ToInt() - lastNonProtectedPosition.ToInt() - 1)
				rangePermissions.AddRange(CreateRangePermissions(rangeAfterProtection, "User", "User"))
			End If
			richEditControl1.Document.EndUpdateRangePermissions(rangePermissions)

			If containsProtectedRanges Then
				document.Protect("123")
				richEditControl1.Options.Authentication.UserName = "User"
				richEditControl1.Options.Authentication.Group = "User"
				richEditControl1.Options.RangePermissions.Visibility = DevExpress.XtraRichEdit.RichEditRangePermissionVisibility.Hidden
			End If
		End Sub

		Private Sub GenerateBookmarks(ByVal document As Document)
			Dim protectedRanges() As DocumentRange = document.FindAll("Protected Range", SearchOptions.None)
			For i As Integer = 0 To protectedRanges.Length - 1
				Dim protectedParagraph As DocumentRange = document.Paragraphs.Get(protectedRanges(i).Start).Range
				document.Bookmarks.Create(protectedParagraph, "protectedRange" & (i + 1).ToString())
			Next i
		End Sub

		Private Sub richEditControl1_BeforePagePaint(ByVal sender As Object, ByVal e As DevExpress.XtraRichEdit.BeforePagePaintEventArgs)
			If e.CanvasOwnerType = CanvasOwnerType.Printer Then
				Return
			End If
			Dim customPagePainter As New CustomDrawPagePainter(richEditControl1)
			e.Painter = customPagePainter
		End Sub

		Private Shared Function CreateRangePermissions(ByVal range As DocumentRange, ByVal userGroup As String, ParamArray ByVal usernames() As String) As List(Of RangePermission)
			Dim rangeList As New List(Of RangePermission)()
			For Each username As String In usernames
				Dim rp As New RangePermission(range)
				rp.Group = userGroup
				rp.UserName = username
				rangeList.Add(rp)
			Next username
			Return rangeList
		End Function
	End Class

	Public Class CustomDrawPagePainter
		Inherits PagePainter
		Private richEditControl As RichEditControl
		Public Sub New(ByVal richEdit As RichEditControl)
			MyBase.New()
		richEditControl = richEdit
		End Sub

		Public Overrides Sub DrawPlainTextBox(ByVal plainTextBox As PlainTextBox)
			HighlightProtectedRange(plainTextBox.Range, plainTextBox.Bounds)
			MyBase.DrawPlainTextBox(plainTextBox)
		End Sub

		Public Overrides Sub DrawSpaceBox(ByVal spaceBox As PlainTextBox)
			HighlightProtectedRange(spaceBox.Range, spaceBox.Bounds)
			MyBase.DrawSpaceBox(spaceBox)
		End Sub

		Private Sub HighlightProtectedRange(ByVal fixedRange As FixedRange, ByVal bounds As Rectangle)
			For i As Integer = 0 To richEditControl.Document.Bookmarks.Count - 1
				Dim protectedBookmark As Bookmark = richEditControl.Document.Bookmarks(i)
				If protectedBookmark.Name.Contains("protectedRange") Then
					If protectedBookmark.Range.Start.ToInt() <= fixedRange.Start AndAlso protectedBookmark.Range.End.ToInt() >= (fixedRange.Start + fixedRange.Length) Then
						Dim richEditBrush As New RichEditBrush(Color.LightPink)
						Canvas.FillRectangle(richEditBrush, bounds)
					End If
				End If
			Next i
		End Sub
	End Class
End Namespace
