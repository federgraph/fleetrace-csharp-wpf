namespace RiggVar.FR
{

    /// <summary>
    /// Extension of TEventColGrid.
    /// </summary>
    public class TEventReportGrid : TEventColGrid
    {
        public TEventReportGrid() : base()
        {
        }

        /// <summary>
        /// DataSet Report
        /// </summary>
        /// <param name="SL">StringList</param>
        public void FRContent6(TStrings SL)
        {
            TEventRowCollection cl = GetBaseRowCollection();
            if (cl != null)
            {

                SL.Add("<e xmlns=\"http://riggvar.net/FR11.xsd\">");
                SL.Add("");
                cl.GetXMLSchema(SL);
                SL.Add("");

                TEventRowCollectionItem cr;
                for (int j = 0; j < cl.Count; j++)
                {
                    int i = j;
                    if (j < DisplayOrder.Count)
                    {
                        i = DisplayOrder.GetByIndex(j);
                    }

                    if ((i >= 0) && (i < cl.Count))
                    {
                        cr = cl[i];

                        string s;
                        s = "<r ID=\"" + cr.BaseID.ToString() + "\" ";
                        s = s + "Pos=\"" + cr.GRank.ToString() + "\" ";
                        s = s + "Bib=\"" + cr.Bib.ToString() + "\" ";
                        s = s + "SNR=\"" + cr.SNR.ToString() + "\" ";
                        s = s + "DN=\"" + cr.DN + "\" ";

                        for (int r = 1; r < cl.RCount; r++)
                        {
                            s = s + "W" + r.ToString() + "=\"" + cr[r] + "\" "; //RaceValue Indexer
                        }
                        s = s + "Pts=\"" + cr.GPoints + "\" ";
                        s = s + "/>";
                        SL.Add(s);
                    }
                }

                SL.Add("");
                cl.GetXMLBackup(SL);
                SL.Add("");
                SL.Add("</e>");
            }
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
                    TEventColProp cp = ColsActive[c];
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
                    TEventColProp cp = ColsActive[c];
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

        /// <summary>
        /// EventReport with bgcolor attributes
        /// 2 Header lines with sort-links
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="aCaption">Table caption</param>
        /// <param name="Modus">Finish or Points</param>
        /// <param name="Props">filled with DetailUrl</param>
        public void FRContent1(TStrings SL, string aCaption, string Modus, TEventProps Props)
        {
            SL.Add("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Add(crlf);
            if (aCaption != "")
            {
                SL.Add("<caption>" + aCaption + "</caption>");
                SL.Add(crlf);
            }
            int sortColIndex_ = this.ColsActive.SortColIndex;
            for (int r_ = -1; r_ < Grid.RowCount; r_++)
            {
                SL.Add("<tr align=\"left\">");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    int r = r_;
                    if (r_ == -1)
                    {
                        r = 0;
                    }

                    TEventColProp cp = ColsActive[c];
                    if (cp == null)
                    {
                        continue;
                    }

                    string s = Grid[c, r];
                    string sColor = CellProps[c, r].HTMLColor;
                    if (s == "")
                    {
                        s = "&nbsp;";
                    }

                    if (r_ == -1)
                    {
                        sColor = @" bgcolor=""Beige""";
                        string sAlign = "";
                        if (cp.Sortable)
                        {
                            if (c == sortColIndex_ && Modus == "Points")
                            {
                                sColor = @" bgcolor=""LightSkyBlue""";
                            }

                            s = string.Format(@"<a href=""{0}?s={1}&amp;m=Points"">{2}</a>", Props.DetailUrl, cp.Index, s);
                        }
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            sAlign = @" align=""right""";
                        }

                        SL.Add(string.Format("<th{0}{1}>{2}</th>", sAlign, sColor, s));
                    }
                    else if (r_ == 0)
                    {
                        sColor = @" bgcolor=""Beige""";
                        string sAlign = "";
                        if (cp.Sortable)
                        {
                            if (c == sortColIndex_ && Modus == "Finish")
                            {
                                sColor = @" bgcolor=""LightSkyBlue""";
                            }

                            s = string.Format(@"<a href=""{0}?s={1}&amp;m=Finish"">{2}</a>", Props.DetailUrl, cp.Index, s);
                        }
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            sAlign = @" align=""right""";
                        }

                        SL.Add(string.Format("<th{0}{1}>{2}</th>", sAlign, sColor, s));
                    }
                    else
                    {
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            SL.Add("<td bgcolor=\"" + sColor + "\" align=\"right\">" + s + "</td>");
                        }
                        else
                        {
                            SL.Add("<td bgcolor=\"" + sColor + "\">" + s + "</td>");
                        }
                    }
                }
                SL.Add("</tr>");
                SL.Add(crlf);
            }
            SL.Add("</table>");
        }

        /// <summary>
        /// EventReport with bgcolor attributes and SortLink - captions
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="aCaption">Table caption</param>
        /// <param name="Modus">Finish or Points</param>
        /// <param name="Props">filled with DetailUrl</param>
        public void FRContent11(TStrings SL, string aCaption, string Modus, TEventProps Props)
        {
            SL.Add("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Add(crlf);
            if (aCaption != "")
            {
                SL.Add("<caption>" + aCaption + "</caption>");
                SL.Add(crlf);
            }
            int sortColIndex_ = this.ColsActive.SortColIndex;
            for (int r = 0; r < Grid.RowCount; r++)
            {

                SL.Add("<tr align=\"left\">");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    TEventColProp cp = ColsActive[c];
                    if (cp == null)
                    {
                        continue;
                    }

                    string s = Grid[c, r];
                    string sColor = CellProps[c, r].HTMLColor;
                    if (s == "")
                    {
                        s = "&nbsp;";
                    }

                    if (r == 0)
                    {
                        string a;
                        if (Modus == "Finish")
                        {
                            a = string.Format(@"<a href=""finish?Sort={0}"">{1}</a>", c, s);
                        }
                        else
                        {
                            a = string.Format(@"<a href=""points?Sort={0}"">{1}</a>", c, s);
                        }

                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            SL.Add(@"<th bgcolor=""" + sColor + @""" align=""right"">" + a + "</th>");
                        }
                        else
                        {
                            SL.Add(@"<th bgcolor=""" + sColor + @""">" + a + "</th>");
                        }
                    }
                    else
                    {
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            SL.Add("<td bgcolor=\"" + sColor + "\" align=\"right\">" + s + "</td>");
                        }
                        else
                        {
                            SL.Add("<td bgcolor=\"" + sColor + "\">" + s + "</td>");
                        }
                    }
                }
                SL.Add("</tr>");
                SL.Add(crlf);
            }
            SL.Add("</table>");
        }

        /// <summary>
        /// EventReport with css class attributes
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="aCaption">Table caption</param>
        /// <param name="Modus">Finish or Points</param>
        /// <param name="Props">filled filled with DetailUrl and EventName</param>
        public void FRContent3(TStrings SL, string aCaption, string Modus, TEventProps Props)
        {
            SL.Add("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Add(crlf);
            if (aCaption != "")
            {
                SL.Add("<caption>" + aCaption + "</caption>");
                SL.Add(crlf);
            }
            int sortColIndex_ = this.ColsActive.SortColIndex;
            int startRow = 0;
            if (Props.DetailUrl != "")
            {
                startRow = -1;
            }

            for (int r_ = startRow; r_ < Grid.RowCount; r_++)
            {
                SL.Add("<tr>");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    int r = r_;
                    if (r_ == -1)
                    {
                        r = 0;
                    }

                    TEventColProp cp = ColsActive[c];
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

                    if (r_ == -1)
                    {
                        sColor = "h";
                        if (cp.Sortable)
                        {
                            if (c == sortColIndex_ && Modus == "Points")
                            {
                                sColor = "sm";
                            }

                            s = string.Format(@"<a href=""{0}?en={1}&s={2}&m=Points"">{3}</a>",
                                Props.DetailUrl, Props.EventNameID, cp.Index, s);
                        }
                        if (cp.Alignment == TColAlignment.taLeftJustify)
                        {
                            sColor += "l";
                        }

                        sColor = string.Format(@" class=""{0}""", sColor);
                        SL.Add(string.Format("<th{0}>{1}</th>", sColor, s));
                    }
                    else if (r_ == 0)
                    {
                        sColor = "h";
                        if (cp.Sortable)
                        {
                            if (c == sortColIndex_ && Modus == "Finish")
                            {
                                sColor = "sm";
                            }

                            if (Props.DetailUrl != "")
                            {
                                s = string.Format(@"<a href=""{0}?en={1}&s={2}&m=Finish"">{3}</a>",
                                    Props.DetailUrl, Props.EventNameID, cp.Index, s);
                            }
                        }
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
        /// EventReport with Sort-gif
        /// </summary>
        /// <param name="SL">StringList</param>
        /// <param name="aCaption">Table caption</param>
        /// <param name="Modus">Finish or Points</param>
        /// <param name="Props">Container for additional properties</param>
        public void FRContent2(TStrings SL, string aCaption, string Modus, TEventProps Props)
        {
            SL.Add("<table border=\"1\" width=\"100%\" cellspacing=\"0\" cellpadding=\"1\">");
            SL.Add(crlf);
            if (aCaption != "")
            {
                SL.Add("<caption>" + aCaption + "</caption>");
                SL.Add(crlf);
            }
            int sortColIndex_ = this.ColsActive.SortColIndex;
            for (int r = 0; r < Grid.RowCount; r++)
            {
                SL.Add("<tr align=\"left\">");
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    TEventColProp cp = ColsActive[c];
                    if (cp == null)
                    {
                        continue;
                    }

                    string s = Grid[c, r];
                    string sColor = CellProps[c, r].HTMLColor;
                    if (s == "")
                    {
                        s = "&nbsp;";
                    }

                    if (r == 0)
                    {
                        sColor = @" bgcolor=""Beige""";
                        string sAlign = "";
                        if (cp.Sortable)
                        {
                            if (c == sortColIndex_ && Modus == "Finish")
                            {
                                sColor = @" bgcolor=""#AA99FF""";
                            }

                            if (c == sortColIndex_ && Modus == "Points")
                            {
                                sColor = @" bgcolor=""#FF99CC""";
                            }

                            s = string.Format(@"<img border=""0"" usemap=""#IM{0}"" src=""F.gif"">&nbsp;{1}", cp.Index, s);
                        }
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            sAlign = @" align=""right""";
                        }

                        SL.Add(string.Format("<th{0}{1}>{2}</th>", sAlign, sColor, s));
                    }
                    else
                    {
                        if (cp.Alignment == TColAlignment.taRightJustify)
                        {
                            SL.Add("<td bgcolor=\"" + sColor + "\" align=\"right\">" + s + "</td>");
                        }
                        else
                        {
                            SL.Add("<td bgcolor=\"" + sColor + "\">" + s + "</td>");
                        }
                    }
                }
                SL.Add("</tr>");
                SL.Add(crlf);
            }
            SL.Add("</table>");

            if (Props.DetailUrl.StartsWith("javascript"))
            {
                //postback
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    string sHREF;
                    SL.Add(string.Format(@"<MAP name=""IM{0}"">", c));
                    sHREF = string.Format(Props.DetailUrl, c, "Finish", Props.EventNameID);
                    SL.Add(string.Format(@"<area id=""F{0}"" href=""{1}"" shape=""RECT"" coords=""0,0,19,15"">", c, sHREF));
                    sHREF = string.Format(Props.DetailUrl, c, "Points", Props.EventNameID);
                    SL.Add(string.Format(@"<area id=""P{0}"" href=""{1}"" shape=""RECT"" coords=""20,0,39,15"">", c, sHREF));
                    SL.Add("</MAP>");
                }
            }
            else
            {
                //querystring
                for (int c = 0; c < Grid.ColCount; c++)
                {
                    SL.Add(string.Format(@"<MAP name=""IM{0}"">", c));
                    SL.Add(string.Format(@"<area id=""F{0}"" href=""{1}?en={2}&s={0}&m=Finish"" shape=""RECT"" coords=""0,0,19,15"">", c, Props.DetailUrl, Props.EventNameID));
                    SL.Add(string.Format(@"<area id=""P{0}"" href=""{1}?en={2}&s={0}&m=Points"" shape=""RECT"" coords=""20,0,39,15"">", c, Props.DetailUrl, Props.EventNameID));
                    SL.Add("</MAP>");
                }
            }
        }

    }

}
