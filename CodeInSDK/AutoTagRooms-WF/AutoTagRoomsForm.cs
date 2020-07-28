using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace AutoTagRoomsWF
{
    public partial class AutoTagRoomsForm : System.Windows.Forms.Form
    {
        private RoomsData m_roomsData;

        public AutoTagRoomsForm()
        {
            InitializeComponent();
        }

        public AutoTagRoomsForm(RoomsData roomsData) : this()
        {
            m_roomsData = roomsData;
            InitRoomListView();
        }

        private void AutoTagRoomsForm_Load(object sender, EventArgs e)
        {
            // levelsComboBox
            this.levelsComboBox.DataSource = m_roomsData.Levels;
            this.levelsComboBox.DisplayMember = "Name";
            this.levelsComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.levelsComboBox.Sorted = true;
            this.levelsComboBox.DropDown += new EventHandler(levelsComboBox_DropDown);
        }

        void tagTypesComboBox_DropDown(object sender, EventArgs e)
        {
            AdjustWidthComboBox_DropDown(sender, e);
        }

        void levelsComboBox_DropDown(object sender, EventArgs e)
        {
            AdjustWidthComboBox_DropDown(sender, e);
        }


        private void InitRoomListView()
        {
            this.roomsListView.Columns.Clear();

            // Create the columns of the roomsListView
            this.roomsListView.Columns.Add("Room Name");
            foreach (RoomTagType type in m_roomsData.RoomTagTypes)
            {
                this.roomsListView.Columns.Add(type.Name);
            }

            this.roomsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.roomsListView.FullRowSelect = true;
        }

        private void UpdateRoomsList()
        {
            // when update the RoomsListView, clear all the items first
            this.roomsListView.Items.Clear();

            foreach (Room tmpRoom in m_roomsData.Rooms)
            {
                Level level = this.levelsComboBox.SelectedItem as Level;

                if (tmpRoom.LevelId.IntegerValue == level.Id.IntegerValue)
                {
                    ListViewItem item = new ListViewItem(tmpRoom.Name);

                    // Shows the number of each type of RoomTags that the room has
                    foreach (RoomTagType type in m_roomsData.RoomTagTypes)
                    {
                        int count = m_roomsData.GetTagNumber(tmpRoom, type);
                        string str = count.ToString();
                        item.SubItems.Add(str);
                    }

                    this.roomsListView.Items.Add(item);
                }
            }
        }

        private void autoTagButton_Click(object sender, EventArgs e)
        {
            Level level = this.levelsComboBox.SelectedItem as Level;
            RoomTagType tagType = this.tagTypesComboBox.SelectedItem as RoomTagType;
            if (level != null && tagType != null)
            {
                m_roomsData.AutoTagRooms(level, tagType);
            }

            UpdateRoomsList();
        }


        private void levelsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRoomsList();
        }

        private void AdjustWidthComboBox_DropDown(object sender, System.EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox) sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                    ? SystemInformation.VerticalScrollBarWidth
                    : 0;

            int newWidth;
            foreach (Autodesk.Revit.DB.Element element in ((ComboBox) sender).Items)
            {
                string s = element.Name;
                newWidth = (int) g.MeasureString(s, font).Width
                    + vertScrollBarWidth;
                if (width < newWidth)
                {
                    width = newWidth;
                }
            }
            senderComboBox.DropDownWidth = width;
        }
    }
}