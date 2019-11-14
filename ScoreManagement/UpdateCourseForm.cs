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
    public partial class UpdateCourseForm : Form
    {
        private DataSet dataSet;
        private OperationDB operationDB;
        public UpdateCourseForm()
        {
            InitializeComponent();
            InitDataSet();
            this.dataGridViewRecord.DataSource = this.dataSet.Tables[0];

            Config config = new Config();
            this.operationDB = new OperationDB(config.dbStr);
            this.operationDB.Open();
        }


        public UpdateCourseForm(DataSet dataSet, Course course)
        {
            InitializeComponent();
            this.setCourseInfo(course);
            this.dataSet = dataSet;
            this.dataGridViewRecord.DataSource = this.dataSet.Tables[0];

            Config config = new Config();
            this.operationDB = new OperationDB(config.dbStr);
            this.operationDB.Open();
        }



        ~UpdateCourseForm()
        {
            this.operationDB.Close();
        }


        private void InitDataSet()
        {
            this.dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("课程号");
            dataTable.Columns.Add("课程名");
            dataTable.Columns.Add("学分");
            dataTable.Columns.Add("备注");
            this.dataSet.Tables.Add(dataTable);
        }


        private Course getCourseInfo()
        {
            Course course = new Course();
            try
            {
                course.courId = this.textBoxId.Text;
                course.courName = this.textBoxName.Text;
                course.credit = Convert.ToSingle(this.textBoxCredit.Text);
                course.remark = this.textBoxRemark.Text;
            }
            catch { }
            return course;
        }

        private void setCourseInfo(Course course)
        {
            try
            {
                this.textBoxId.Text = course.courId;
                this.textBoxName.Text = course.courName;
                this.textBoxCredit.Text = course.credit.ToString();
                this.textBoxRemark.Text = course.remark;
            }
            catch { }
        }


        private void addDataGridViewRecord(Course course)
        {
            DataRow dataRow = this.dataSet.Tables[0].NewRow();
            dataRow["课程号"] = course.courId;
            dataRow["课程名"] = course.courName;
            dataRow["学分"] = course.credit.ToString();
            dataRow["备注"] = course.remark;
            this.dataSet.Tables[0].Rows.Add(dataRow);
        }

        private bool subDataGridViewRecord(Course course)
        {
            for (int i = 0; i < this.dataSet.Tables[0].Rows.Count; i++)
            {
                if (this.dataSet.Tables[0].Rows[i][0].ToString() == course.courId)
                {
                    this.dataSet.Tables[0].Rows[i].Delete();
                    return true;
                }
            }
            return false;
        }


        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            Course course = this.getCourseInfo();
            Course getCourse = this.getCourseInfo();
            int result;
            if (this.operationDB.SelectCourseRecord(ref getCourse))
            {
                result = this.operationDB.UpdateCourseRecord(course);
            }
            else
            {
                result = this.operationDB.InesrtCourseRecord(course);
            }

            if (result == -1)
            {
                MessageBox.Show("更新失败。", "提示");
            }
            else
            {
                this.subDataGridViewRecord(course);
                this.addDataGridViewRecord(course);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            Course course = this.getCourseInfo();
            if (this.operationDB.DeleteCourseRecord(course) != -1)
            {
                if (!this.subDataGridViewRecord(course))
                {
                    MessageBox.Show("删除成功。", "提示");
                }
            }
            else
            {
                MessageBox.Show("删除失败。", "提示");
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Course course = new Course();
            this.setCourseInfo(course);
        }

        private void dataGridViewRecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Course course = new Course();
            course.courId = this.dataSet.Tables[0].Rows[e.RowIndex][0].ToString();
            course.courName = this.dataSet.Tables[0].Rows[e.RowIndex][1].ToString();
            course.credit = Convert.ToSingle(this.dataSet.Tables[0].Rows[e.RowIndex][2]);
            course.remark = this.dataSet.Tables[0].Rows[e.RowIndex][3].ToString();

            this.setCourseInfo(course);
        }
    }
}
