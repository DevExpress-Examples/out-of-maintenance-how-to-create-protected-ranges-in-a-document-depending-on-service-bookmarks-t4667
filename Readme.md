# How to create protected ranges in a document depending on service bookmarks


<p>The approach for creating <a href="https://documentation.devexpress.com/#WindowsForms/CustomDocument8580">protected document ranges</a> in code was demonstrated in the following example:<br><a href="https://www.devexpress.com/Support/Center/p/E3017">Protection - How to programmatically create a protected document and apply range permissions</a> <br><br>In this example, we extended this approach by calculating the "protected" and "non-protected" document ranges automatically.<br>For all paragraphs that started with the "Protected Range" text, we generated corresponding bookmarks and created protected document ranges.<br>For better visualization, the "protected" ranges are highlighted using the <a href="https://documentation.devexpress.com/#WindowsForms/CustomDocument114069/CustomDraw">RichEditControl Custom Draw API</a>.</p>

<br/>


