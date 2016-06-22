using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace JooVuuX
{
    public partial class Control_ViewTable : UserControl
    {
        public string filterCaption = "Поиск по названию";

        //public SelectElement selectElement;
        public SelectElement selectElement = new SelectElement();
        //public List<FormatElement> formatDetail = new List<FormatElement>();

        public bool hasCheckBox = false;
        public string Select_ID_Detail = "0";
        public string Select_Detail_Name = "";
        public bool isDetailSelected = false;
        public int ItemsTextLineCount = 1;
        public Color defaultRowColor = Color.Black;

        public event EventHandler SelectEvent;
        public event EventHandler DoubleClickEvent;
        public string root_Folder_Name = "";
        public string sql_d = "";
        public List<TableStructure> tableStructure = new List<TableStructure>();
        public List<CompareList> compareList = new List<CompareList>();

        public string ID_Field_Name = "ID";
        //List<string> path = new List<string>();

        public ListViewItem selectLVI = null;
        public bool isStructureLoaded = false;

        TreeNode tree = new TreeNode();
        //public static TreeNode Select_Node = null;
        //public List<DataDetail> dataDetail = new List<DataDetail>();

        public bool canSave = true;

        public System.Windows.Forms.ContextMenuStrip popupMenuListView = null;
        //public DevExpress.XtraBars.PopupMenu popupMenuListView = null;




        public class SelectElement
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string Tag { get; set; }
            public string ID { get; set; }

            public SelectElement(string Type, string Name, string Tag, string ID)
            {
                this.Type = Type;
                this.Name = Name;
                this.Tag = Tag;
                this.ID = ID;
            }

            public SelectElement()
            {
                this.Type = "";
                this.Name = "";
                this.Tag = "";
                this.ID = "0";
            }


        }

        //Список полей и значений для сравнения
        public class CompareList
        {
            public string FieldName { get; set; }   //Поле, значение которого сравнивается
            public string Value { get; set; }       //Значение, с которым происходит сравнение или название поля
            public string Sign { get; set; }        //Знак для срвнения < > =
            public Color ColorMark { get; set; }    //Цвет после сравнения, если условие выполняеся
            public bool Error { get; set; }         //Признак ошибки

            public CompareList(string FieldName, string Sign, string Value, Color ColorMark, bool Error)
            {
                this.FieldName = FieldName;
                this.Sign = Sign;
                this.Value = Value;
                this.ColorMark = ColorMark;
                this.Error = Error;
            }
        }

        public class TableStructure
        {
            public string FieldName { get; set; }
            public string DataType { get; set; }
            public string HeaderText { get; set; }
            public int Width { get; set; }
            public HorizontalAlignment Aligment { get; set; }
            public int Id { get; set; }
            public string ColorName { get; set; }

            public TableStructure(string FieldName, string DataType, string HeaderText, int Width, HorizontalAlignment Aligment, int Id)
            {
                this.FieldName = FieldName;
                this.DataType = DataType;
                this.HeaderText = HeaderText;
                this.Width = Width;
                this.Aligment = Aligment;
                this.Id = Id;
                this.ColorName = "";
            }

            public TableStructure(string FieldName, string DataType, string HeaderText, int Width, HorizontalAlignment Aligment, int Id, string ColorName)
            {
                this.FieldName = FieldName;
                this.DataType = DataType;
                this.HeaderText = HeaderText;
                this.Width = Width;
                this.Aligment = Aligment;
                this.Id = Id;
                this.ColorName = ColorName;
            }
        }

        public Control_ViewTable()
        {
            InitializeComponent();
        }

        private void listView_DoubleBuffered1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems != null)
            {
                if (this.DoubleClickEvent != null) this.DoubleClickEvent(this, e);

            }
        }

        private void listView_DoubleBuffered1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            DesignMetods.DrawColumnHeader(sender, e);
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            DesignMetods.DrawListViewItem(sender, e);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (selectElement == null) return;

            if (e.IsSelected == true)
            {
                Select_ID_Detail = Convert.ToString((Int32)e.Item.Tag);
                Select_Detail_Name = e.Item.Text;
                isDetailSelected = true;

                selectElement.ID = Convert.ToString((Int32)e.Item.Tag);
                selectElement.Name = e.Item.Text;
                selectElement.Tag = Convert.ToString((Int32)e.Item.Tag);
                selectElement.Type = "";

                if (this.SelectEvent != null) this.SelectEvent(this, e);
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if ((listView1.SelectedItems == null) || (listView1.SelectedItems.Count == 0))
            {
                isDetailSelected = false;
                Select_Detail_Name = "";

                selectElement.ID = "0";
                selectElement.Name = "";
                selectElement.Tag = "";
                selectElement.Type = "";

                if (this.SelectEvent != null) this.SelectEvent(this, e);
            }
        }

        private void Control_ViewTable_Load(object sender, EventArgs e)
        {
            if (hasCheckBox == true) listView1.CheckBoxes = true;
            else listView1.CheckBoxes = false;
            ListViewHelper.EnableDoubleBuffer(listView1);
        }

        //Заполнение из списка
        public void FillDataList(object dataList, string typename)
        {
            if (ItemsTextLineCount == 1) listView1.SmallImageList = imageList1;
            else if (ItemsTextLineCount == 2) listView1.SmallImageList = imageList2;

            listView1.BeginUpdate();
            listView1.Items.Clear();

            //Type type = Type.GetType(typename);
            //object objList = Activator.CreateInstance(type);

            //var rows = MakeList(objList);

            foreach (var item in (IEnumerable)dataList)
            {
                int index = 0;
                Color mColor = defaultRowColor;
                ListViewItem lvi = new ListViewItem();
                foreach (TableStructure ts in tableStructure)
                {
                    if (index == 0)
                    {
                        lvi.Text = getValue(item, ts.FieldName);
                    }
                    else
                    {
                        //test type field
                        if (ts.DataType == "constant")
                        {
                            lvi.SubItems.Add(getValue(item, ts.FieldName));
                        }
                        else if (ts.DataType == "bool")
                        {
                            bool b = Convert.ToBoolean(getValue(item, ts.FieldName));
                            if (b == true) lvi.SubItems.Add("Да");
                            else lvi.SubItems.Add("Нет");
                        }
                        else if (ts.DataType == "decimal")
                        {
                            decimal dValue = Convert.ToDecimal(getValue(item, ts.FieldName));
                            lvi.SubItems.Add(dValue.ToString());
                            if (dValue == 0)
                            {
                                if (Convert.ToString(ts.ColorName).Contains("Mark"))
                                {
                                    string[] arMark = Convert.ToString(ts.ColorName).Split('_');
                                    if (arMark.Length > 2)
                                    {
                                        Color fontColor = Color.FromName(arMark[1]);
                                        lvi.SubItems[lvi.SubItems.Count - 1].ForeColor = fontColor;
                                    }
                                }
                            }
                        }
                        else
                        {
                            lvi.SubItems.Add(getValue(item, ts.FieldName));
                        }
                    }
                    //if (FieldCompare(ts.FieldName, item) == true) lvi.SubItems[index].ForeColor = Color.Red;
                    //lvi.SubItems[index].ForeColor = FieldCompare(ts.FieldName, item);
                    if (ts.DataType != "constant")
                    {
                        if (ts.DataType == "string")
                        {
                            if (mColor == defaultRowColor) mColor = FieldCompare(ts.FieldName, getValue(item, ts.FieldName));
                        }
                        else
                        {
                            if (mColor == defaultRowColor) mColor = FieldCompare(ts.FieldName, item);
                        }
                    }
                    index++;
                }
                //lvi.ForeColor = Color.Red;
                //Set color for all subitems
                for (int i = 0; i < lvi.SubItems.Count; i++)
                {
                    if (lvi.SubItems[i].ForeColor == SystemColors.WindowText) lvi.SubItems[i].ForeColor = mColor;
                }
                lvi.Tag = Convert.ToInt32(getValue(item, ID_Field_Name));
                lvi.Selected = false;
                if (Select_ID_Detail == "0")
                {
                    if (listView1.Items.Count == 0) lvi.Selected = true;
                }
                else
                {
                    if (Select_ID_Detail == getValue(item, ID_Field_Name)) lvi.Selected = true;
                }
                listView1.Items.Add(lvi);
            }
            listView1.EndUpdate();
            listView1.CheckBoxes = hasCheckBox;

        }

        public Color FieldCompare(string fieldName, string comp_value)
        {
            canSave = true;
            Color result = defaultRowColor;
            foreach (CompareList itemComp in compareList)
            {
                if (fieldName == itemComp.FieldName)
                {
                    string FieldName = itemComp.FieldName;

                    //string Value = "0";
                    //if (FieldNameExist(itemComp.Value) == true)
                    //{
                    //    Value = getValue(item, itemComp.Value);
                    //}
                    //else Value = itemComp.Value; 

                    string Value = itemComp.Value;
                    string Sign = itemComp.Sign;
                    Color mColor = itemComp.ColorMark;
                    bool error = itemComp.Error;

                    if ((Sign == "<") || (Sign == ">"))
                    {
                        if (comp_value == "") comp_value = "0";
                        if (comp_value.Contains('%')) comp_value = comp_value.Trim('%');
                        comp_value = comp_value.Trim();
                        decimal dVal_A = Convert.ToDecimal(comp_value.Replace('.', ','));

                        if (Value == "") Value = "0";
                        if (Value.Contains('%')) Value = Value.Trim('%');
                        Value = Value.Trim();
                        decimal dVal_B = Convert.ToDecimal(Value.Replace('.', ','));

                        if (Sign == "<") { if (dVal_A < dVal_B) { result = mColor; if (error) canSave = !error; } }
                        else if (Sign == ">") { if (dVal_A > dVal_B) { result = mColor; if (error) canSave = !error; } }
                        else if (Sign == "=") { if (dVal_A == dVal_B) { result = mColor; if (error) canSave = !error; } }
                    }
                    else
                    {
                        comp_value = comp_value.Trim();
                        //Сравниваем текст
                        if (comp_value == Value) { result = mColor; if (error) canSave = !error; }
                    }
                }
            }

            return result;
        }

        public Color FieldCompare(string fieldName, dynamic item)
        {
            canSave = true;
            Color result = defaultRowColor;
            foreach (CompareList itemComp in compareList)
            {
                if (fieldName == itemComp.FieldName)
                {
                    string FieldName = itemComp.FieldName;

                    //string Value = "0";
                    //if (FieldNameExist(itemComp.Value) == true)
                    //{
                    //    Value = getValue(item, itemComp.Value);
                    //}
                    //else Value = itemComp.Value;  

                    string Value = itemComp.Value;
                    string Sign = itemComp.Sign;
                    Color mColor = itemComp.ColorMark;
                    bool error = itemComp.Error;
                    if (FieldNameExist(Value) == true)
                    {
                        //поле найдено, значит нужно получить из него значение и сравнить с текущим полем
                        string Val_A = getValue(item, FieldName);
                        string Val_B = getValue(item, Value);

                        if (Val_A == "") Val_A = "0";
                        decimal dVal_A = Convert.ToDecimal(Val_A.Replace('.', ','));

                        if (Val_B == "") Val_B = "0";
                        decimal dVal_B = Convert.ToDecimal(Val_B.Replace('.', ','));

                        if (Sign == "<") { if (dVal_A < dVal_B) { result = mColor; if (error) canSave = !error; } }
                        else if (Sign == ">") { if (dVal_A > dVal_B) { result = mColor; if (error) canSave = !error; } }
                        else if (Sign == "=") { if (dVal_A == dVal_B) { result = mColor; if (error) canSave = !error; } }

                    }
                    else
                    {
                        //поле НЕ найдено, значит нужно сравнить со значением в списке
                        string Val_A = getValue(item, FieldName);
                        string Val_B = Value;

                        if (Val_A == "") Val_A = "0";
                        decimal dVal_A = Convert.ToDecimal(Val_A.Replace('.', ','));

                        if (Val_B == "") Val_B = "0";
                        decimal dVal_B = Convert.ToDecimal(Val_B.Replace('.', ','));

                        if (Sign == "<") { if (dVal_A < dVal_B) { result = mColor; if (error) canSave = !error; } }
                        else if (Sign == ">") { if (dVal_A > dVal_B) { result = mColor; if (error) canSave = !error; } }
                        else if (Sign == "=") { if (dVal_A == dVal_B) { result = mColor; if (error) canSave = !error; } }
                    }
                }
            }

            return result;
        }

        private bool FieldNameExist(string Value)
        {
            bool result = false;

            foreach (TableStructure ts in tableStructure)
            {
                if (ts.FieldName == Value) result = true;
            }

            return result;
        }


        public string getValue(object item, string nameField)
        {
            string result = "0";
            foreach (var propertyInfo in item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (propertyInfo.Name == nameField)
                {
                    result = propertyInfo.GetValue(item, null).ToString();
                }
            }
            return result;
        }

        public void createStructure()
        {
            if (isStructureLoaded == true) return;
            //this.barManager1.SetPopupContextMenu(listView1, popupMenuListView);
            int max_width = 0;
            //Build structure
            listView1.Columns.Clear();
            listView1.Clear();

            foreach (TableStructure ts in tableStructure)
            {
                listView1.Columns.Add(ts.HeaderText, ts.Width, ts.Aligment);
                if (ts.ColorName != "") listView1.Columns[listView1.Columns.Count - 1].Tag = ts.ColorName;
                max_width = max_width + ts.Width;
            }
            if ((max_width < listView1.ClientSize.Width) && (max_width > 0))
            {
                int delta = listView1.ClientSize.Width - max_width;
                listView1.Columns[listView1.Columns.Count - 1].Width = listView1.Columns[listView1.Columns.Count - 1].Width + delta;
            }
            isStructureLoaded = true;
        }
    }
}
