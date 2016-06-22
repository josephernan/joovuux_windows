using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JooVuuX
{
    public static class DesignMetods
    {
        public static ImageList imageList = null;
        public static bool showIcon = false;
        //public static event EventHandler HeaderChanged;

        public static void DrawListViewItem(object sender, DrawListViewSubItemEventArgs e)
        {

            int deltaX = 0;
            int deltaIcon = 0;
            //listView1.BeginUpdate();
            //if (((e.ItemState & ListViewItemStates.Selected) > 0) || (e.Item == FocusedIt))
            ListView lv = (ListView)sender;
            if (lv.CheckBoxes == true)
            {
                deltaX = 10;
            }
            
            if ((e.ItemState & ListViewItemStates.Selected) > 0)
            {
                Color mColor = Color.LightSteelBlue;
                if (e.Item.ToolTipText == "") e.Item.ToolTipText = "0";

                //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(47, 121, 138)), e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 4); //LightSteelBlue
                e.Graphics.FillRectangle(new SolidBrush(Color.LightSteelBlue), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                
                if (e.Item.SubItems[0] == e.SubItem)
                {
                    if ((Convert.ToInt32(e.Item.ToolTipText) > 0) && (imageList != null)) deltaIcon = 10;
                    StringFormat strFmt = new StringFormat();
                    if (e.Bounds.Height < 24) strFmt.FormatFlags = StringFormatFlags.NoWrap;
                    strFmt.Trimming = StringTrimming.Word;
                    RectangleF rectF;
                    if (e.Header.TextAlign == HorizontalAlignment.Center)
                    {
                        strFmt.Alignment = StringAlignment.Center;
                        //strFmt.LineAlignment = StringAlignment.Center;
                        rectF = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);
                    }
                    else rectF = new RectangleF(e.Bounds.X + 12 + deltaX + deltaIcon, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);


                    //Font font1 = new Font("Verdana", 11f, FontStyle.Regular);
                    Font font1 = new Font("Tahoma", 10f, FontStyle.Regular);

                    SolidBrush drawBrush = new SolidBrush(e.SubItem.ForeColor);
                    e.Graphics.DrawString(e.SubItem.Text, font1, drawBrush, rectF, strFmt);
                    //e.Graphics.DrawString(e.SubItem.Text, font1, SystemBrushes.ControlText, rectF, strFmt);

                    if ((Convert.ToInt32(e.Item.ToolTipText) > 0) && (imageList != null))
                    {
                        e.Graphics.DrawImage(imageList.Images[Convert.ToInt32(e.Item.ToolTipText)], e.SubItem.Bounds.Location.X + 4, e.SubItem.Bounds.Location.Y + 2);
                    }


                }
                else if (e.Item.SubItems[1] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[2] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[3] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[4] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[5] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[6] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[7] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[8] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[9] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[10] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[11] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[12] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[13] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[14] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[15] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[16] == e.SubItem)
                {
                    DrawSelectedItem(sender, e, deltaX, mColor);
                }
            }
            else
            {
                Color mColor;
                if (e.Item.ToolTipText == "") e.Item.ToolTipText = "0";

                if (e.ItemIndex % 2 == 0) mColor = Color.White;
                else mColor = Color.FromArgb(240, 244, 247);  //mColor = Color.WhiteSmoke; 

                //Рисование выбранной строки, но не подсвеченной!!
                ListView markLV = (ListView)sender;
                if (markLV.SelectedItems.Count > 0)
                {
                    if (markLV.SelectedItems[0].Index == e.ItemIndex) mColor = Color.LightSteelBlue;
                }

                e.Graphics.FillRectangle(new SolidBrush(mColor), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                //e.Graphics.DrawRectangle(new Pen(Color.LightGray), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 1);
                if (e.Item.SubItems[0] == e.SubItem)
                {
                    if ((Convert.ToInt32(e.Item.ToolTipText) > 0) && (imageList != null)) deltaIcon = 10;
                    StringFormat strFmt = new StringFormat();
                    if (e.Bounds.Height < 24) strFmt.FormatFlags = StringFormatFlags.NoWrap;
                    strFmt.Trimming = StringTrimming.Word;
                    RectangleF rectF;
                    if (e.Header.TextAlign == HorizontalAlignment.Center)
                    {
                        strFmt.Alignment = StringAlignment.Center;
                        //strFmt.LineAlignment = StringAlignment.Center;
                        rectF = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);
                    }
                    else rectF = new RectangleF(e.Bounds.X + 12 + deltaX + deltaIcon, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);


                    //Font font1 = new Font("Verdana", 11f, FontStyle.Regular);
                    Font font1 = new Font("Tahoma", 10f, FontStyle.Regular);

                    //var flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
                    //var bounds = new Rectangle(e.Bounds.X + 12 + deltaX + deltaIcon, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);
                    //TextRenderer.DrawText(e.Graphics, e.SubItem.Text, font1, bounds, Color.Black, flags);

                    //SizeF stringSize = new SizeF();
                    //stringSize = e.Graphics.MeasureString(e.SubItem.Text, font1, (int)rectF.Width);

                    SolidBrush drawBrush = new SolidBrush(e.SubItem.ForeColor);
                    e.Graphics.DrawString(e.SubItem.Text, font1, drawBrush, rectF, strFmt);
                    //e.Graphics.DrawString(e.SubItem.Text, font1, SystemBrushes.ControlText, rectF, strFmt);
                    //e.Graphics.DrawString(e.SubItem.Text, font1, SystemBrushes.ControlText, rectF, strFmt);

                    if ((Convert.ToInt32(e.Item.ToolTipText) > 0) && (imageList != null))
                    {
                        e.Graphics.DrawImage(imageList.Images[Convert.ToInt32(e.Item.ToolTipText)], e.SubItem.Bounds.Location.X + 4, e.SubItem.Bounds.Location.Y + 2);
                    }

                    ////Рисование маркера для выбранной строки но не подсвеченной
                    //ListView markLV = (ListView)sender;
                    //if (markLV.SelectedItems.Count > 0)
                    //{
                    //    if (markLV.SelectedItems[0].Index == e.ItemIndex)
                    //    {
                    //        //int x = e.Bounds.X;
                    //        //int y = e.Bounds.Y;
                    //        //mColor = Color.Black;
                    //        //e.Graphics.DrawLine(new Pen(mColor, 1), x + 2, y + 5, x + 2, y + 15);
                    //        //e.Graphics.DrawLine(new Pen(mColor, 1), x + 2, y + 5, x + 7, y + 10);
                    //        //e.Graphics.DrawLine(new Pen(mColor, 1), x + 7, y + 10, x + 2, y + 15);
                    //        //---------------------------------------------------------
                    //        //e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.Bounds.X - 1, e.Bounds.Y, 4, e.Bounds.Height);
                    //        //---------------------------------------------------------
                    //        e.Graphics.FillRectangle(new SolidBrush(Color.LightSteelBlue), e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                    //    }
                    //}
 
                }
                else if (e.Item.SubItems[1] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[2] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[3] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[4] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[5] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[6] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[7] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[8] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[9] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[10] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[11] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[12] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[13] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[14] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[15] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
                else if (e.Item.SubItems[16] == e.SubItem)
                {
                    DrawItem(sender, e, deltaX, mColor);
                }
             
             
            }


            if (lv.CheckBoxes == true)
            {
                if (e.Item.Checked) ControlPaint.DrawCheckBox(e.Graphics, 4, e.Bounds.Top + 4, 15, 15, ButtonState.Flat | ButtonState.Checked);
                else ControlPaint.DrawCheckBox(e.Graphics, 4, e.Bounds.Top + 4, 15, 15, ButtonState.Flat);
            }
        }

        public static void DrawItem(object sender, DrawListViewSubItemEventArgs e, int deltaX, Color mColor)
        {
            e.Graphics.FillRectangle(new SolidBrush(mColor), e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height);
            if (e.SubItem.Text.ToLower() == "true")
            {
                drawCheckBoxe((Graphics)e.Graphics, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height, true);
            }
            else if (e.SubItem.Text.ToLower() == "false")
            {
                drawCheckBoxe((Graphics)e.Graphics, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height, false);
            }
            else
            {
                StringFormat strFmt = new StringFormat();
                strFmt.FormatFlags = StringFormatFlags.NoWrap;
                strFmt.Trimming = StringTrimming.Word;
                RectangleF rectF;
                if (e.Header.TextAlign == HorizontalAlignment.Center)
                {
                    strFmt.Alignment = StringAlignment.Center;
                    //strFmt.LineAlignment = StringAlignment.Center;
                    rectF = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);
                }
                else rectF = new RectangleF(e.Bounds.X + 12 + deltaX, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);


                //Font font1 = new Font("Verdana", 11f, FontStyle.Regular);
                //SystemBrushes.ControlText
                Font font1 = new Font("Tahoma", 10f, FontStyle.Regular);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                if ((e.SubItem.ForeColor != Color.Black) && (e.SubItem.ForeColor != SystemColors.WindowText)) drawBrush = new SolidBrush(e.SubItem.ForeColor);
                else if (Convert.ToString(e.Header.Tag) != "")
                {
                    if (Convert.ToString(e.Header.Tag) == "Gray") drawBrush = new SolidBrush(Color.Gray);
                    else if (Convert.ToString(e.Header.Tag).Contains("Mark"))
                    {
                        string[] arMark = Convert.ToString(e.Header.Tag).Split('_');
                        if (arMark.Length > 2)
                        {
                            if (e.Item.Checked == true) drawBrush = new SolidBrush(Color.FromName(arMark[1]));
                            else drawBrush = new SolidBrush(Color.FromName(arMark[2]));
                            
                        }
                    }
                }
                else
                {
                    drawBrush = new SolidBrush(e.SubItem.ForeColor);
                }
                e.Graphics.DrawString(e.SubItem.Text, font1, drawBrush, rectF, strFmt);
            }
        }

        public static void DrawSelectedItem(object sender, DrawListViewSubItemEventArgs e, int deltaX, Color mColor)
        {
            e.Graphics.FillRectangle(new SolidBrush(mColor), e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height);
            if (e.SubItem.Text.ToLower() == "true")
            {
                drawCheckBoxe((Graphics)e.Graphics, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height, true);
            }
            else if (e.SubItem.Text.ToLower() == "false")
            {
                drawCheckBoxe((Graphics)e.Graphics, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height, false);
            }
            else
            {
                StringFormat strFmt = new StringFormat();
                strFmt.FormatFlags = StringFormatFlags.NoWrap;
                strFmt.Trimming = StringTrimming.Word;
                RectangleF rectF;
                if (e.Header.TextAlign == HorizontalAlignment.Center)
                {
                    strFmt.Alignment = StringAlignment.Center;
                    //strFmt.LineAlignment = StringAlignment.Center;
                    rectF = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);
                }
                else rectF = new RectangleF(e.Bounds.X + 12 + deltaX, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);


                Font font1 = new Font("Tahoma", 10f, FontStyle.Regular); //Gadugi
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                if ((e.SubItem.ForeColor != Color.Black) && (e.SubItem.ForeColor != SystemColors.WindowText)) drawBrush = new SolidBrush(e.SubItem.ForeColor);
                else if (Convert.ToString(e.Header.Tag) != "")
                {
                    if (Convert.ToString(e.Header.Tag) == "Gray") drawBrush = new SolidBrush(Color.Gray);
                    else if (Convert.ToString(e.Header.Tag).Contains("Mark"))
                    {
                        string[] arMark = Convert.ToString(e.Header.Tag).Split('_');
                        if (arMark.Length > 2)
                        {
                            if (e.Item.Checked == true) drawBrush = new SolidBrush(Color.FromName(arMark[1]));
                            else drawBrush = new SolidBrush(Color.FromName(arMark[2]));

                        }
                    }
                }
                else
                {
                    drawBrush = new SolidBrush(e.SubItem.ForeColor);
                }
                e.Graphics.DrawString(e.SubItem.Text, font1, drawBrush, rectF, strFmt);
                //e.Graphics.DrawString(e.SubItem.Text, font1, SystemBrushes.ControlText, rectF, strFmt);
            }
        }

        public static void drawCheckBoxe(Graphics g, int x, int y, int width, int height, bool isCheck)
        {
            Color mColor;
            int center = (int)(width / 2);
            mColor = Color.Gray;
            //g.DrawRectangle(new Pen(mColor, 1), x + center - 8 + 2, y + 5, 14, 14);
            g.DrawRectangle(new Pen(mColor, 1), x + center - 8, y + 3, 14, 14);
            if (isCheck == true)
            {
                mColor = Color.Black;
                g.DrawLine(new Pen(mColor, 2), x + center - 4 - 2, y + 10, x + center - 2, y + 14);
                g.DrawLine(new Pen(mColor, 2), x + center - 2, y + 14, x + center + 8 - 2, y + 6);
                //g.DrawLine(new Pen(mColor, 2), x + center - 4, y + 12, x + center, y + 16);
                //g.DrawLine(new Pen(mColor, 2), x + center, y + 16, x + center + 8, y + 8);
            }
        }

        public static void DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color mColor;
            //mColor = Color.FromArgb(240, 244, 247);  //
            mColor = Color.FromArgb(246, 246, 246);
            int deltaX = 0;
            DrawHeader(sender, e, deltaX, mColor);

            //if (HeaderChanged != null) HeaderChanged(sender, e);

            //if ((e.ColumnIndex == 0))
            //{
            //    DrawHeader(sender, e, deltaX, mColor);
            //}
            //else if ((e.ColumnIndex == 1))
            //{
            //    DrawHeader(sender, e, deltaX, mColor);
            //}
            //else if ((e.ColumnIndex == 2))
            //{
            //    DrawHeader(sender, e, deltaX, mColor);
            //}
            //else if ((e.ColumnIndex == 3))
            //{
            //    DrawHeader(sender, e, deltaX, mColor);
            //}
        }

        public static void DrawHeader(object sender, DrawListViewColumnHeaderEventArgs e, int deltaX, Color mColor)
        {
            e.Graphics.FillRectangle(new SolidBrush(mColor), e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height);
            StringFormat strFmt = new StringFormat();
            strFmt.FormatFlags = StringFormatFlags.NoWrap;
            strFmt.Trimming = StringTrimming.Word;
            RectangleF rectF;
            if (e.Header.TextAlign == HorizontalAlignment.Center)
            {
                strFmt.Alignment = StringAlignment.Center;
                //strFmt.LineAlignment = StringAlignment.Center;
                rectF = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);
            }
            else rectF = new RectangleF(e.Bounds.X + 6 + deltaX, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);
            
            //Allwaice center
            //strFmt.Alignment = StringAlignment.Center;
            //rectF = new RectangleF(e.Bounds.X, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height);

            Font font1 = new Font("Tahoma", 10f, FontStyle.Regular);

            // Measure string.
            SizeF stringSize = new SizeF();
            stringSize = e.Graphics.MeasureString(e.Header.Text, font1);
            if (stringSize.Width > e.Bounds.Width)
            {
                //Font font_small = new Font("Tahoma", 8f, FontStyle.Regular);
                //rectF = new RectangleF(e.Bounds.X + 6 + deltaX, e.Bounds.Y - 3, e.Bounds.Width - 8, e.Bounds.Height);

                //StringFormat strFmt_small = new StringFormat();
                //strFmt_small.Trimming = StringTrimming.Word;
                //e.Graphics.DrawString(e.Header.Text, font_small, SystemBrushes.ControlText, rectF, strFmt_small);

                Font font_small = new Font("Tahoma", 8f, FontStyle.Regular);                
                StringFormat strFmt_small = new StringFormat();
                strFmt_small.Trimming = StringTrimming.Word;
                if (e.Header.TextAlign == HorizontalAlignment.Center)
                {
                    strFmt_small.Alignment = StringAlignment.Center;
                    rectF = new RectangleF(e.Bounds.X, e.Bounds.Y - 6, e.Bounds.Width - 8, e.Bounds.Height + 3);
                }
                else rectF = new RectangleF(e.Bounds.X + 6 + deltaX, e.Bounds.Y - 6, e.Bounds.Width - 8, e.Bounds.Height + 3);
                e.Graphics.DrawString(e.Header.Text, font_small, SystemBrushes.ControlText, rectF, strFmt_small);

                //Clear first lines
                e.Graphics.FillRectangle(new SolidBrush(mColor), e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height - 15);

                //first line 
                //rectF = new RectangleF(e.Bounds.X + 6 + deltaX, e.Bounds.Y - 3, e.Bounds.Width - 8, e.Bounds.Height);
                StringFormat strFmt_small_1 = new StringFormat();
                strFmt_small_1.FormatFlags = StringFormatFlags.NoWrap;
                strFmt_small_1.Trimming = StringTrimming.Word;
                if (e.Header.TextAlign == HorizontalAlignment.Center)
                {
                    strFmt_small_1.Alignment = StringAlignment.Center;
                    rectF = new RectangleF(e.Bounds.X, e.Bounds.Y - 2, e.Bounds.Width - 8, e.Bounds.Height);
                }
                else rectF = new RectangleF(e.Bounds.X + 6 + deltaX, e.Bounds.Y - 2, e.Bounds.Width - 8, e.Bounds.Height);
                e.Graphics.DrawString(e.Header.Text, font_small, SystemBrushes.ControlText, rectF, strFmt_small_1);
            }
            else e.Graphics.DrawString(e.Header.Text, font1, SystemBrushes.ControlText, rectF, strFmt);

            
            e.Graphics.DrawLine(new Pen(Color.LightGray, 1), e.Bounds.X - 1, e.Bounds.Height - 1, e.Bounds.X + e.Bounds.Width + 2, e.Bounds.Height - 1);

            float[] dashValues = { 1, 2, 1, 2 };
            Pen mPen = new Pen(Color.LightGray, 1);
            mPen.DashPattern = dashValues;
            e.Graphics.DrawLine(mPen, e.Bounds.X - 1, 0, e.Bounds.X - 1, e.Bounds.Height - 4);
        }

        /*
             
             
        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            
            //listView1.BeginUpdate();
            //if (((e.ItemState & ListViewItemStates.Selected) > 0) || (e.Item == FocusedIt))
            if ((e.ItemState & ListViewItemStates.Selected) > 0)
            {
                //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(47, 121, 138)), e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height - 4); //LightSteelBlue
                e.Graphics.FillRectangle(new SolidBrush(Color.LightSteelBlue), e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 2, e.Bounds.Height - 4);
                if (e.Item.SubItems[0] == e.SubItem)
                {
                    //string[] arr_s = e.SubItem.Text.Split('\n');
                    string[] arr_s = e.SubItem.Text.Split('n');
                    if (arr_s.Length > 1)
                    {
                        Font font1 = new Font("Gadugi", 13.5f, FontStyle.Regular);
                        Font font2 = new Font("Gadugi", 11.0f, FontStyle.Regular);
                        string s1 = arr_s[0];
                        string s2 = arr_s[1];

                        StringFormat strFmt = new StringFormat();
                        strFmt.FormatFlags = StringFormatFlags.NoWrap;
                        strFmt.Trimming = StringTrimming.Word;
                        RectangleF rectF = new RectangleF(e.Bounds.X + 16, e.Bounds.Y + 8, e.Bounds.Width - 8, e.Bounds.Height);
                        e.Graphics.DrawString(s1, font1, SystemBrushes.ControlText, rectF, strFmt);

                        e.Graphics.DrawString(s2, font2, SystemBrushes.ControlDark, e.Bounds.X + 16, e.Bounds.Y + 30);
                    }
                    else
                    {
                        StringFormat strFmt = new StringFormat();
                        strFmt.FormatFlags = StringFormatFlags.NoWrap;
                        strFmt.Trimming = StringTrimming.Word;
                        RectangleF rectF = new RectangleF(e.Bounds.X + 16, e.Bounds.Y + 18, e.Bounds.Width - 8, e.Bounds.Height);

                        Font font1 = new Font("Gadugi", 13.5f, FontStyle.Regular);
                        e.Graphics.DrawString(e.SubItem.Text, font1, SystemBrushes.ControlText, rectF, strFmt);
                    }
                }
                else if (e.Item.SubItems[1] == e.SubItem)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.LightSteelBlue), e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 2, e.Bounds.Height - 4);
                  //  e.Graphics.FillRectangle(new SolidBrush(Color.Red), e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 2, e.Bounds.Height - 4);
                }
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 12, e.Bounds.Height - 4);
                if (e.Item.SubItems[0] == e.SubItem)
                {
                    //string[] arr_s = e.SubItem.Text.Split('\n');
                    string[] arr_s = e.SubItem.Text.Split('n');
                    if (arr_s.Length > 1)
                    {
                        Font font1 = new Font("Gadugi", 13.5f, FontStyle.Regular);
                        Font font2 = new Font("Gadugi", 11.0f, FontStyle.Regular);
                        string s1 = arr_s[0];
                        string s2 = arr_s[1];

                        StringFormat strFmt = new StringFormat();
                        strFmt.FormatFlags = StringFormatFlags.NoWrap;
                        strFmt.Trimming = StringTrimming.Word;
                        RectangleF rectF = new RectangleF(e.Bounds.X + 16, e.Bounds.Y + 8, e.Bounds.Width - 8, e.Bounds.Height);
                        e.Graphics.DrawString(s1, font1, SystemBrushes.ControlText, rectF, strFmt);
                        e.Graphics.DrawString(s2, font2, SystemBrushes.ControlDark, e.Bounds.X + 16, e.Bounds.Y + 30);
                    }
                    else
                    {
                        StringFormat strFmt = new StringFormat();
                        strFmt.FormatFlags = StringFormatFlags.NoWrap;
                        strFmt.Trimming = StringTrimming.Word;
                        RectangleF rectF = new RectangleF(e.Bounds.X + 16, e.Bounds.Y + 18, e.Bounds.Width - 8, e.Bounds.Height);

                        Font font1 = new Font("Gadugi", 13.5f, FontStyle.Regular);
                        e.Graphics.DrawString(e.SubItem.Text, font1, SystemBrushes.ControlText, rectF, strFmt);
                    }

                }
                else if (e.Item.SubItems[1] == e.SubItem)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds.X - 1, e.Bounds.Y, e.Bounds.Width + 1, e.Bounds.Height - 4);
                }
                if (FocusedIt != null)
                {
                    //FocusedIt.Focused = true;
                    //FocusedIt.Selected = true;
                }
             
            }
             */

    }
}
