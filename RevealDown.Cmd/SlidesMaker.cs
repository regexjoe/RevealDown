﻿using System.Text;
using System.Text.RegularExpressions;

namespace RevealDown.Cmd
{
    internal class SlidesMaker
    {
        private const bool AddHeaderFooterDefault = true;
        private const int SlideLevelDefault = 1;
        private const bool HorizontalRuleBreaksSlideDefault = false;

        internal static Regex BodyLineRegex = new Regex(@"<body(?=(>| ))",
                                                        RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture |
                                                        RegexOptions.Compiled);


        internal SlidesMaker()
        {
            AddHeaderFooter = AddHeaderFooterDefault;
            SlideLevel = SlideLevelDefault;
            HorizontalRuleBreaksSlide = HorizontalRuleBreaksSlideDefault;
            Header = TemplateDefaults.RevealJsHeader;
            Footer = TemplateDefaults.RevealJsFooter;
        }


        public bool AddHeaderFooter { get; set; }
        public int SlideLevel { get; set; }
        public bool HorizontalRuleBreaksSlide { get; set; }
        public string Header { get; set; }
        public string Footer { get; set; }


        public string GetSections(string[] htmlLines)
        {
            var sectionsBuilder = new StringBuilder();

            if (AddHeaderFooter) sectionsBuilder.Append(Header);

            sectionsBuilder.Append(TemplateDefaults.RevealJsSlidesOpen);

            sectionsBuilder.AppendLine("<section style=\"level1\">");
            sectionsBuilder.AppendLine("<section style=\"level2\">");

            var foundBody = false;
            var awaitingFirstBreak = true;
            var firstAttempt = true;
            while (!foundBody)
            {
                if (!firstAttempt) foundBody = true;
                foreach (var line in htmlLines)
                {
                    if (!foundBody)
                    {
                        foundBody = BodyLineRegex.IsMatch(line);
                        continue;
                    }
                    if (Regex.IsMatch(line, @"</body>", RegexOptions.IgnoreCase))
                    {
                        break;
                    }

                    var lineType = GetLineType(line);

                    //don't append breaktags for the first breaking line, since the section was opened before this loop
                    if (awaitingFirstBreak && lineType != LineType.NoBreak)
                    {
                        awaitingFirstBreak = false;
                    }
                    else
                    {
                        AppendBreakTags(sectionsBuilder, lineType);
                    }

                    if (lineType != LineType.SectionBreakWhichReplacesLine)
                    {
                        sectionsBuilder.AppendLine(line);
                    }
                }
                firstAttempt = false;
            }

            sectionsBuilder.AppendLine("</section>");
            sectionsBuilder.AppendLine("</section>");

            sectionsBuilder.Append(TemplateDefaults.RevealJsSlidesClose);

            if (AddHeaderFooter) sectionsBuilder.Append(Footer);

            return sectionsBuilder.ToString();
        }


        private static void AppendBreakTags(StringBuilder sectionsBuilder, LineType lineType)
        {
            switch (lineType)
            {
                case LineType.SlideBreak:
                    {
                        sectionsBuilder.AppendLine("</section>");
                        sectionsBuilder.AppendLine("</section>");
                        sectionsBuilder.AppendLine("<section style=\"level1\">");
                        sectionsBuilder.AppendLine("<section style=\"level2\">");
                        break;
                    }
                case LineType.SectionBreak:
                case LineType.SectionBreakWhichReplacesLine:
                    {
                        sectionsBuilder.AppendLine("</section>");
                        sectionsBuilder.AppendLine("<section style=\"level2\">");
                        break;
                    }
            }
        }

        private LineType GetLineType(string line)
        {
            //support for ---- 
            if (HorizontalRuleBreaksSlide &&
                Regex.IsMatch(line, @"^\s*<hr(?=(>| ))", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
            {
                return LineType.SectionBreakWhichReplacesLine;
            }

            //support for <!---->
            if ( Regex.IsMatch(line, @"^\s*<!-- *-->", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture))
            {
                return LineType.SectionBreakWhichReplacesLine;
            }

            //support for headings
            var heading = Regex.Match(line, @"(?<=^\s*<h)\d(?=(>| ))", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (heading.Success)
            {
                var headingLevel = int.Parse(heading.Value);
                if (headingLevel <= SlideLevel) return LineType.SlideBreak;
                if (headingLevel == SlideLevel + 1) return LineType.SectionBreak;
            }

            return LineType.NoBreak;
        }
    }
}
