<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/128609814/15.2.14%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T466775)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->
<!-- default file list -->
*Files to look at*:

* [Form1.cs](./CS/WindowsFormsApplication1/Form1.cs) (VB: [Form1.vb](./VB/WindowsFormsApplication1/Form1.vb))
<!-- default file list end -->
# How to create protected ranges in a document depending on service bookmarks


<p>The approach for creating <a href="https://documentation.devexpress.com/#WindowsForms/CustomDocument8580">protected document ranges</a> in code was demonstrated in the following example:<br><a href="https://www.devexpress.com/Support/Center/p/E3017">Protection - How to programmatically create a protected document and apply range permissions</a> <br><br>In this example, we extended this approach by calculating the "protected" and "non-protected" document ranges automatically.<br>For all paragraphs that started with the "Protected Range" text, we generated corresponding bookmarks and created protected document ranges.<br>For better visualization, the "protected" ranges are highlighted using the <a href="https://documentation.devexpress.com/#WindowsForms/CustomDocument114069/CustomDraw">RichEditControl Custom Draw API</a>.</p>

<br/>


