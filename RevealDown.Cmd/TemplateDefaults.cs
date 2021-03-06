﻿namespace RevealDown.Cmd
{
    internal static class TemplateDefaults
    {
        internal const string RevealJsHeader = @"<!doctype html>
<html lang=""en"">

	<head>
		<meta charset=""utf-8"">

		<title>Slides</title>

		<link rel=""stylesheet"" href=""reveal.js/css/reveal.min.css"">
		<link rel=""stylesheet"" href=""reveal.js/css/theme/default.css"" id=""theme"">
	</head>

	<body>
";

        internal const string RevealJsFooter = @"		<script src=""reveal.js/lib/js/head.min.js""></script>
		<script src=""reveal.js/js/reveal.min.js""></script>

		<script>

			Reveal.initialize();

		</script>

	</body>
</html>";

        internal const string RevealJsSlidesOpen = @"		<div class=""reveal"">
			<div class=""slides"">
";

        internal const string RevealJsSlidesClose = @"			</div>
		</div>
";

    }
}
