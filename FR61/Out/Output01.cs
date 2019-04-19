using System.Text.RegularExpressions;

namespace RiggVar.FR
{

    public class TOutput01
    {
        readonly TTokenParser TokenParser;
        private TEventNode Node;
        private readonly TEventReportGrid ColGrid;
        private readonly int Layout;

        public TOutput01()
        {
            ColGrid = new TEventReportGrid
            {
                Grid = new TSimpleHashGrid(),
                ColorSchema = TColGridColorSchema.colorRed,
                OnGetBaseNode = new TEventColGrid.TGetBaseNodeFunction(EventNodeFunction),
                UseHTML = true
            };
            TokenParser = new TTokenParser();
            Layout = 1;
        }

        TEventNode EventNodeFunction() => Node;

        private void GetMsg(TStrings SL, int ReportID)
        {
            switch (ReportID)
            {
                case 93: FinishReport(SL); break;
                case 94: PointsReport(SL); break;

                case 95: FinishTable(SL); break;
                case 96: PointsTable(SL); break;

                case 98: CssReport(SL); break;
                case 99: IndexReport(SL); break;
                default:
                    break;
            }
        }

        public void IndexReport(TStrings SL)
        {
            int k;
            char sep;

            TEventRowCollection cl;
            TEventRowCollectionItem cr;
            TEventColProp cp;

            SL.Clear();
            sep = ',';

            //Shortcuts
            TEventReportGrid g = ColGrid;
            TEventBO ev = TMain.BO.EventBO;
            Node = ev.CurrentNode;
            cl = Node.Collection;

            //Setup
            g.SetColBOReference(Node.ColBO);
            g.ColsAvail.Init();
            ev.InitColsActiveLayout(g, Layout);

            //first column is ID-column, always counting from 1 upwards
            for (int r = 0; r < cl.Count; r++)
            {
                SL.Add(Utils.IntToStr(r + 1));
            }

            //from second column to last column
            for (int c = 1; c < g.ColsActive.Count; c++)
            {
                cp = g.ColsActive[c];

                //retain default sort-order for unsortable columns
                if (!cp.Sortable)
                {
                    for (int r = 0; r < cl.Count; r++)
                    {
                        SL[r] = SL[r] + sep + Utils.IntToStr(r + 1);
                    }
                    continue;
                }

                g.InitDisplayOrder(c);
                for (int r = 0; r < cl.Count; r++)
                {
                    k = g.DisplayOrder.GetByIndex(r);
                    cr = cl[k];
                    SL[r] = SL[r] + sep + cr.BaseID.ToString();
                }
            }

            for (int r = 0; r < cl.Count - 1; r++)
            {
                SL[r] = '[' + SL[r] + "],";
            }
            int z = cl.Count - 1;
            SL[z] = '[' + SL[z] + ']';
            SL.Insert(0, "[");
            SL.Add("]");
        }

        public void CssReport(TStrings SL)
        {
            SL.Clear();

            string crlf = "";

            Regex rx = new Regex(@"col_R(\d)+");
            Match m;

            TEventBO ev = TMain.BO.EventBO;
            TEventNode en = TMain.BO.EventNode;
            TEventRowCollectionItem cr;
            TEventRowCollection cl = en.Collection;
            TEventColProp cp;

            Node = en; //instance variable

            //Grid Setup
            TEventReportGrid g = ColGrid; //shortcut            
            g.SetColBOReference(Node.ColBO);
            g.ColsAvail.Init();
            ev.InitColsActiveLayout(g, Layout);
            g.ColsActive.SortColIndex = 0;
            g.AlwaysShowCurrent = false; //true=with Error-Colors and Current-Colors
            g.UpdateAll();

            SL.Add("<div id='results'>");
            SL.Add("<table class='sortable fr results'>");
            SL.Add(crlf);

            //if (aCaption != "")
            //{
            //    SL.Add("<caption>" + aCaption + "</caption>");
            //    SL.Add(crlf);
            //}

            string s;
            string css = "";
            string clss;

            //header row
            SL.Add("<thead>");
            SL.Add("<tr>");
            for (int c = 0; c < g.Grid.ColCount; c++)
            {
                cp = g.ColsActive[c];
                if (cp == null)
                {
                    continue;
                }

                //content of field
                s = g.Grid[c, 0];
                if (s == "")
                {
                    s = "&nbsp;";
                }

                //css for field
                css = "h";
                if (cp.Alignment == TColAlignment.taLeftJustify)
                {
                    css += "l";
                }

                clss = string.Format(@" class=""{0}""", css);

                //add field
                SL.Add(string.Format("<th{0}>{1}</th>", clss, s));
            }
            SL.Add("</tr>");
            SL.Add("</thead>");
            SL.Add(crlf);

            int race;
            string srace;
            int fleet;

            //normal rows
            for (int r = 1; r < g.Grid.RowCount; r++)
            {
                cr = cl[r - 1];

                SL.Add("<tr>");
                for (int c = 0; c < g.Grid.ColCount; c++)
                {
                    cp = g.ColsActive[c];
                    if (cp == null)
                    {
                        continue;
                    }

                    //content of field
                    s = g.Grid[c, r];
                    if (s == "")
                    {
                        s = "&nbsp;";
                    }

                    //css for field
                    clss = TColGridColors.CSSClass(g.CellProps[c, r]);

                    m = rx.Match(cp.NameID);
                    if (m.Success && m.Groups.Count == 2)
                    {
                        srace = m.Groups[1].Value;
                        race = Utils.StrToIntDef(srace, -1);
                        if (race > 0)
                        {
                            fleet = cr.Race[race].Fleet;
                            css = string.Format("g{0}", fleet);
                            clss = string.Format(@" class=""{0}""", css);
                        }
                    }
                    else
                    {
                        if (clss.IndexOf("bgcolor") > 0)
                        {
                            css = "n";
                            clss = string.Format(@" class=""{0}""", css);
                        }
                    }

                    //add field
                    SL.Add(string.Format("<td{0}>{1}</td>", clss, s));
                }
                SL.Add("</tr>");
                SL.Add(crlf);
            }

            SL.Add("</table>");
            SL.Add("</div>");
        }

        protected void TableSortReport(TStrings SL, int Modus)
        {
            SL.Clear();
            //SL.Add("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0");
            //SL.Add("  Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            //SL.Add("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            //SL.Add("<head>");
            //SL.Add("<title>rvts-VS</title>");
            //SL.Add(string.Format("<link rel=\"stylesheet\" href=\"{0}stylesheets/fr42.css\" type=\"text/css\" />", TMain.ContextPath));
            //SL.Add(string.Format("<script type=\"text/javascript\" src=\"{0}javascripts/core.js\"></script>", TMain.ContextPath));
            //SL.Add(string.Format("<script type=\"text/javascript\" src=\"{0}javascripts/rvts.js\"></script>", TMain.ContextPath));
            //SL.Add("</head>");
            //SL.Add("<body>");
            SL.Add("<h2>" + TMain.BO.EventProps.EventName + "</h2>");
            SL.Add("");

            if (Modus == 1)
            {
                TMain.BO.EventNode.WebLayout = 1; //Finish
            }
            else
            {
                TMain.BO.EventNode.WebLayout = 2; //Points
            }

            TableSortData(SL);
            TMain.BO.EventNode.WebLayout = 0;


            SL.Add("");
            //SL.Add("</body>");
            //SL.Add("</html>");
        }

        public void TableSortData(TStrings SL)
        {
            TStringList SL1 = new TStringList();
            CssReport(SL1);
            for (int i = 0; i < SL1.Count; i++)
            {
                SL.Add(SL1[i]);
            }
            SL.Add("");
            WritePropertyTable(SL);
            SL.Add("");
            SL.Add("<div id=\"index_table\"><pre>");
            IndexReport(SL1);
            for (int i = 0; i < SL1.Count; i++)
            {
                SL.Add(SL1[i]);
            }
            SL.Add("</pre></div>");
        }

        public void FinishTable(TStrings SL)
        {
            SL.Clear();
            TMain.BO.EventNode.WebLayout = 1;
            TableSortData(SL);
            TMain.BO.EventNode.WebLayout = 0;
        }

        public void PointsTable(TStrings SL)
        {
            SL.Clear();
            TMain.BO.EventNode.WebLayout = 2;
            TableSortData(SL);
            TMain.BO.EventNode.WebLayout = 0;
        }

        public void FinishReport(TStrings SL)
        {
            TableSortReport(SL, 1);
        }

        public void PointsReport(TStrings SL)
        {
            TableSortReport(SL, 2);
        }

        private void Tr(TStrings SL, string n, string v)
        {
            SL.Add(string.Format(PropertyTableRowFormatString, n, v));
        }

        readonly string PropertyTableRowFormatString = "<tr><td>{0}</td><td>{1}</td></tr>";

        private void WritePropertyTable(TStrings SL)
        {
            TEventProps EP = TMain.BO.EventProps;
            SL.Add("<table class='fr properties'>");
            SL.Add("<thead><tr><th>Name</th><th>Value</th></tr></thead>");
            Tr(SL, "HR.MD5", ResultHash.Value);
            Tr(SL, "EP.Name", EP.EventName);
            Tr(SL, "EP.ScoringSystem", TEventProps.ScoringSystemStruct[EP.ScoringSystem]);
            Tr(SL, "EP.Throwouts", Utils.IntToStr(EP.Throwouts));
            if (EP.FirstIs75)
            {
                Tr(SL, "EP.FirstIs75", Utils.BoolStr[EP.FirstIs75]);
            }

            if (EP.ReorderRAF == false)
            {
                Tr(SL, "EP.ReorderRAF", Utils.BoolStr[EP.ReorderRAF]);
            }

            Tr(SL, "EP.FieldCount", EP.FieldCount);
            Tr(SL, "EP.NameFieldSchema", EP.NameSchema);
            Tr(SL, "EP.NameFieldCount", EP.NameFieldCount);
            Tr(SL, "EP.NameFieldOrder", EP.NameFieldOrder);
            Tr(SL, "EP.FieldCaptions", EP.FieldCaptions);
            Tr(SL, "EP.UseFleets", Utils.BoolStr[EP.UseFleets]);
            Tr(SL, "EP.TargetFleetSize", Utils.IntToStr(EP.TargetFleetSize));
            Tr(SL, "EP.FirstFinalRace", Utils.IntToStr(EP.FirstFinalRace));
            SL.Add("</table>");
        }
    }

}
