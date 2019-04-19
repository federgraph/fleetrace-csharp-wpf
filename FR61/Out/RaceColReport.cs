namespace RiggVar.FR
{
    /// <summary>
    /// TRaceColGrid Extensions.
    /// </summary>
    public class TRaceReportGrid : TRaceColGrid
    {
        public TRaceReportGrid()
            : base()
        {
        }

        /// <summary>
        /// Normal Report with css class attributes
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="aCaption">Table caption</param>
        public void FRContent4(TStrings SL, string aCaption)
        {
            SL.Add("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Add(crlf);
            if (aCaption != "")
            {
                SL.Add("<caption>" + aCaption + "</caption>");
                SL.Add(crlf);
            }
            //int sortColIndex_ = this.ColsActive.SortColIndex;
            for (int r = 0; r < Grid.RowCount; r++)
            {
                SL.Add("<tr>");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    TRaceColProp cp = ColsActive[c];
                    if (cp == null)
                    {
                        continue;
                    }

                    string s = Grid[c, r];
                    string sColor;
                    if (s == "")
                    {
                        s = "&nbsp;";
                    }

                    if (r == 0)
                    {
                        sColor = "h";
                        if (cp.Alignment == TColAlignment.taLeftJustify)
                        {
                            sColor += "l";
                        }

                        sColor = string.Format(@" class=""{0}""", sColor);
                        SL.Add(string.Format("<th{0}>{1}</th>", sColor, s));
                    }
                    else
                    {
                        sColor = TColGridColors.CSSClass(CellProps[c, r]);
                        SL.Add("<td" + sColor + ">" + s + "</td>");
                    }
                }
                SL.Add("</tr>");
                SL.Add(crlf);
            }
            SL.Add("</table>");
        }

        /// <summary>
        /// Normal Report with bgcolor attributes
        /// used for Event and Race tables
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="aCaption">Table caption</param>
        public void FRContent5(TStrings SL, string aCaption)
        {
            SL.Add("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Add(crlf);
            if (aCaption != "")
            {
                SL.Add("<caption>" + aCaption + "</caption>");
                SL.Add(crlf);
            }
            for (int r = 0; r < Grid.RowCount; r++)
            {
                SL.Add("<tr align=\"left\">");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    TRaceColProp cp = ColsActive[c];
                    if (cp == null)
                    {
                        continue;
                    }

                    string sColor = CellProps[c, r].HTMLColor;
                    sColor = string.Format(@" bgcolor=""{0}""", sColor);

                    string s = Grid[c, r];
                    if (s == null || s == "")
                    {
                        s = "&nbsp;";
                    }

                    string sAlign = "";
                    if (cp.Alignment == TColAlignment.taRightJustify)
                    {
                        sAlign = @" align=""right""";
                    }

                    if (r == 0)
                    {
                        sColor = @" bgcolor=""Beige""";
                        SL.Add(string.Format("<th{0}{1}>{2}</th>", sAlign, sColor, s));
                    }
                    else
                    {
                        SL.Add(string.Format("<td{0}{1}>{2}</td>", sAlign, sColor, s));
                    }
                }
                SL.Add("</tr>");
                SL.Add(crlf);
            }
            SL.Add("</table>");
        }
        /// <summary>
        /// Report without formatting
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="aCaption">Table caption</param>
        public void FRContent0(TStrings SL, string aCaption)
        {
            SL.Add("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Add(crlf);
            if (aCaption != "")
            {
                SL.Add("<caption>" + aCaption + "</caption>");
                SL.Add(crlf);
            }
            for (int r = 0; r < Grid.RowCount; r++)
            {
                SL.Add("<tr>");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    string s = Grid[c, r];
                    if (s == "")
                    {
                        s = "&nbsp;";
                    }

                    if (r == 0)
                    {
                        SL.Add("<th>" + s + "</th>");
                    }
                    else
                    {
                        SL.Add("<td>" + s + "</td>");
                    }
                }
                SL.Add("</tr>");
                SL.Add(crlf);
            }
            SL.Add("</table>");
        }

    }

}
