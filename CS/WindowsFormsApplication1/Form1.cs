using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using DevExpress.XtraRichEdit.API.Native;

namespace WindowsFormsApplication1 {
    public partial class Form1 : RibbonForm {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            richEditControl1.BeforePagePaint += richEditControl1_BeforePagePaint;

            richEditControl1.LoadDocument("testDocument.rtf");
            GenerateBookmarks(richEditControl1.Document);
            CreateProtectedRanges(richEditControl1.Document);
        }

        private void CreateProtectedRanges(Document document) {
            DocumentPosition lastNonProtectedPosition = document.Range.Start;
            bool containsProtectedRanges = false;
            RangePermissionCollection rangePermissions = richEditControl1.Document.BeginUpdateRangePermissions();
            for(int i = 0; i < document.Bookmarks.Count; i++) {
                Bookmark protectedBookmark = document.Bookmarks[i];
                if(protectedBookmark.Name.Contains("protectedRange")) {
                    containsProtectedRanges = true;

                    rangePermissions.AddRange(CreateRangePermissions(protectedBookmark.Range, "Admin", "Admin"));
                    if(protectedBookmark.Range.Start.ToInt() > lastNonProtectedPosition.ToInt()) {
                        DocumentRange rangeAfterProtection = richEditControl1.Document.CreateRange(lastNonProtectedPosition, protectedBookmark.Range.Start.ToInt() - lastNonProtectedPosition.ToInt() - 1);
                        rangePermissions.AddRange(CreateRangePermissions(rangeAfterProtection, "User", "User"));
                    }
                    lastNonProtectedPosition = protectedBookmark.Range.End;
                }
            }

            if(document.Range.End.ToInt() > lastNonProtectedPosition.ToInt()) {
                DocumentRange rangeAfterProtection = richEditControl1.Document.CreateRange(lastNonProtectedPosition, document.Range.End.ToInt() - lastNonProtectedPosition.ToInt() - 1);
                rangePermissions.AddRange(CreateRangePermissions(rangeAfterProtection, "User", "User"));
            }
            richEditControl1.Document.EndUpdateRangePermissions(rangePermissions);

            if(containsProtectedRanges) {
                document.Protect("123");
                richEditControl1.Options.Authentication.UserName = "User";
                richEditControl1.Options.Authentication.Group = "User";
                richEditControl1.Options.RangePermissions.Visibility = DevExpress.XtraRichEdit.RichEditRangePermissionVisibility.Hidden;
            }
        }

        private void GenerateBookmarks(Document document) {
            DocumentRange[] protectedRanges = document.FindAll("Protected Range", SearchOptions.None);
            for(int i = 0; i < protectedRanges.Length; i++) {
                DocumentRange protectedParagraph = document.Paragraphs.Get(protectedRanges[i].Start).Range;
                document.Bookmarks.Create(protectedParagraph, "protectedRange" + (i + 1).ToString());
            }
        }

        void richEditControl1_BeforePagePaint(object sender, DevExpress.XtraRichEdit.BeforePagePaintEventArgs e) {
            if(e.CanvasOwnerType == CanvasOwnerType.Printer) {
                return;
            }
            CustomDrawPagePainter customPagePainter = new CustomDrawPagePainter(richEditControl1);
            e.Painter = customPagePainter;
        }

        private static List<RangePermission> CreateRangePermissions(DocumentRange range, string userGroup, params string[] usernames) {
            List<RangePermission> rangeList = new List<RangePermission>();
            foreach(string username in usernames) {
                RangePermission rp = new RangePermission(range);
                rp.Group = userGroup;
                rp.UserName = username;
                rangeList.Add(rp);
            }
            return rangeList;
        }
    }

    public class CustomDrawPagePainter : PagePainter {
        RichEditControl richEditControl;
        public CustomDrawPagePainter(RichEditControl richEdit) : base() { richEditControl = richEdit; }

        public override void DrawPlainTextBox(PlainTextBox plainTextBox) {
            HighlightProtectedRange(plainTextBox.Range, plainTextBox.Bounds);
            base.DrawPlainTextBox(plainTextBox);
        }

        public override void DrawSpaceBox(PlainTextBox spaceBox) {
            HighlightProtectedRange(spaceBox.Range, spaceBox.Bounds);
            base.DrawSpaceBox(spaceBox);
        }

        private void HighlightProtectedRange(FixedRange fixedRange, Rectangle bounds) {
            for(int i = 0; i < richEditControl.Document.Bookmarks.Count; i++) {
                Bookmark protectedBookmark = richEditControl.Document.Bookmarks[i];
                if(protectedBookmark.Name.Contains("protectedRange")) {
                    if(protectedBookmark.Range.Start.ToInt() <= fixedRange.Start && protectedBookmark.Range.End.ToInt() >= (fixedRange.Start + fixedRange.Length)) {
                        RichEditBrush richEditBrush = new RichEditBrush(Color.LightPink);
                        Canvas.FillRectangle(richEditBrush, bounds);
                    }
                }
            }            
        }
    }
}
