using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreManagement
{
    public partial class ModifyForm : Form
    {
        private OperationDB operationDB;
        private DataSet dataSet;
        public ModifyForm()
        {
            InitializeComponent();
            InitDataSet();
            this.dataGridViewRecord.DataSource = dataSet.Tables[0];

            Config config = new Config();
            this.operationDB = new OperationDB(config.dbStr);
            this.operationDB.Open();
        }

        public ModifyForm(DataSet data)
        {
            InitializeComponent();
            dataSet = data;
            this.dataGridViewRecord.DataSource = dataSet.Tables[0];

            Config config = new Config();
            this.operationDB = new OperationDB(config.dbStr);
            this.operationDB.Open();
        }

        ~ModifyForm() {
            try
            {
                this.operationDB.Close();
            }
            catch
            {
                ;
            }
        }

        private void InitDataSet()
        {
            this.dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("学号");
            dataTable.Columns.Add("姓名");
            dataTable.Columns.Add("专业");
            dataTable.Columns.Add("性别");
            dataTable.Columns.Add("出生日期");
            dataTable.Columns.Add("总分");
            dataTable.Columns.Add("备注");
            this.dataSet.Tables.Add(dataTable);
        }

        private bool check()
        {
            bool sign = false;

            if (this.textBoxNum.Text != "" && this.textBoxName.Text != "" && this.textBoxBirth.Text != "")
            {
                sign = true;
            }

            return sign;
        }

        private string getGender()
        {
            if (this.radioButtonMan.Checked)
            {
                return "男";
            }
            if (this.radioButtonWoman.Checked)
            {
                return "女";
            }
            return null;
        }

        private void setGender(string gender)
        {
            if (gender == "女")
            {
                this.radioButtonMan.Checked = false;
                this.radioButtonWoman.Checked = true;
            }
            else if (gender == "男")
            {
                this.radioButtonMan.Checked = true;
                this.radioButtonWoman.Checked = false;
            }
            else
            {
                this.radioButtonMan.Checked = false;
                this.radioButtonWoman.Checked = false;
            }
        }


        private void addDataGridViewRecord(StuInfo stuInfo)
        {
            DataRow dataRow = this.dataSet.Tables[0].NewRow();
            dataRow["学号"] = stuInfo.id;
            dataRow["姓名"] = stuInfo.name;
            dataRow["专业"] = stuInfo.major;
            dataRow["性别"] = stuInfo.gender;
            dataRow["出生日期"] = stuInfo.birth;
            dataRow["总分"] = stuInfo.totalScore;
            dataRow["备注"] = stuInfo.remark;
            this.dataSet.Tables[0].Rows.Add(dataRow);
        }

        private bool subDataGridViewRecord(StuInfo stuInfo)
        {
            for (int i = 0; i < this.dataSet.Tables[0].Rows.Count; i++)
            {
                if (this.dataSet.Tables[0].Rows[i][0].ToString() == stuInfo.id)
                {
                    this.dataSet.Tables[0].Rows[i].Delete();
                    return true;
                }
            }
            return false;
        }


        private StuInfo getStuInfo()
        {
            StuInfo stu;
            stu.id = this.textBoxNum.Text;
            stu.name = this.textBoxName.Text;
            stu.gender = this.getGender();
            stu.birth = this.textBoxBirth.Text;
            stu.major = this.textBoxMajor.Text;
            stu.totalScore = Convert.ToSingle(this.textBoxTotalScore.Text);
            stu.remark = this.textBoxRemark.Text;
            return stu;
        }

        private void setStuInfo(StuInfo stu)
        {
            this.textBoxNum.Text = stu.id;
            this.textBoxName.Text = stu.name;
            this.setGender(stu.gender);
            this.textBoxBirth.Text = stu.birth;
            this.textBoxMajor.Text = stu.major;
            this.textBoxTotalScore.Text = stu.totalScore.ToString();
            this.textBoxRemark.Text = stu.remark;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (!check())
            {
                MessageBox.Show("加*为必填字段", "注意");
                return;
            }

            StuInfo stu;
            StuInfo getStu = new StuInfo();
            stu = this.getStuInfo();
            int result;

            if (this.operationDB.GetOneStuInfoStruct(stu.id, ref getStu))
            {
                result = this.operationDB.UpdateStuInfo(stu);
            }
            else
            {
                result = this.operationDB.InsertStuInfo(stu);
            }

            if (result == -1)
            {
                MessageBox.Show("更新失败。", "提示");
            }
            else
            {
                this.subDataGridViewRecord(stu);
                this.addDataGridViewRecord(stu);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            if (!check())
            {
                MessageBox.Show("加*为必填字段", "注意");
                return;
            }

            StuInfo stu = this.getStuInfo();
            StuInfo getStu = new StuInfo();
            if (this.operationDB.GetOneStuInfoStruct(stu.id, ref getStu))
            {
                if (this.operationDB.DeleteStuInfo(stu) == -1)
                {
                    MessageBox.Show("删除失败。", "提示");
                }
                else
                {
                    if (!this.subDataGridViewRecord(stu))
                    {
                        MessageBox.Show("删除成功。", "提示");
                    }
                }
            }
            
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            StuInfo stu = new StuInfo();
            this.setStuInfo(stu);
        }

        private void dataGridViewRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                this.textBoxNum.Text = this.dataGridViewRecord.Rows[e.RowIndex].Cells[0].Value.ToString();
                this.textBoxName.Text = this.dataGridViewRecord.Rows[e.RowIndex].Cells[1].Value.ToString();
                this.textBoxMajor.Text = this.dataGridViewRecord.Rows[e.RowIndex].Cells[2].Value.ToString();
                this.setGender(this.dataGridViewRecord.Rows[e.RowIndex].Cells[3].Value.ToString());
                this.textBoxBirth.Text = this.dataGridViewRecord.Rows[e.RowIndex].Cells[4].Value.ToString();
                this.textBoxTotalScore.Text = this.dataGridViewRecord.Rows[e.RowIndex].Cells[5].Value.ToString();
                this.textBoxRemark.Text = this.dataGridViewRecord.Rows[e.RowIndex].Cells[6].Value.ToString();
            }
            catch { }
        }
    }
}
