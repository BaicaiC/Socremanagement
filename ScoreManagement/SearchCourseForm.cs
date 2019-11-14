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
    public partial class SearchCourseForm : Form
    {
        DataSet dataSet;
        OperationDB operationDB;
        public SearchCourseForm()
        {
            InitializeComponent();
            this.dataSet = new DataSet();

            Config config = new Config();
            this.operationDB = new OperationDB(config.dbStr);
            this.operationDB.Open();
        }

        ~SearchCourseForm()
        {
            this.operationDB.Close();
        }

        private void SearchCourseForm_Load(object sender, EventArgs e)
        {
            this.dataSet = this.operationDB.GetAllCourseInfo("%", "%");
            this.dataGridViewResult.DataSource = this.dataSet.Tables[0];
        }

        private DataSet findRow(string key, string value)
        {
            int index = -1;
            DataSet dataSetLoca = new DataSet();
            dataSetLoca.Tables.Add(this.dataSet.Tables[0]);
            if (key == "id")
            {
                index = 0;
            }
            else if (key == "name")
            {
                index = 1;
            }
            else
            {
                for (int i = 0; i < this.dataSet.Tables[0].Rows.Count; i++)
                {
                    if (this.dataSet.Tables[0].Rows[i][0].ToString() == key &&
                        this.dataSet.Tables[0].Rows[i][1].ToString() == value)
                    {
                        DataRow dataRow = this.dataSet.Tables[0].NewRow();
                        dataRow = this.dataSet.Tables[0].Rows[i];
                        dataSetLoca.Tables[0].Rows.Add(dataRow);
                    }
                }
            }
            if (index != 0 || index != 1)
            {
                return dataSetLoca;
            }
            for (int i = 0; i < this.dataSet.Tables[0].Rows.Count; i++)
            {
                if (this.dataSet.Tables[0].Rows[i][index].ToString() == value)
                {
                    DataRow dataRow = this.dataSet.Tables[0].NewRow();
                    dataRow = this.dataSet.Tables[0].Rows[i];
                    dataSetLoca.Tables[0].Rows.Add(dataRow);
                }
            }
            return dataSetLoca;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string id = this.textBoxId.Text == "" ? "%" : this.textBoxId.Text;
            string name = this.textBoxName.Text == "" ? "%" : this.textBoxName.Text;
            this.dataSet = this.operationDB.GetAllCourseInfo(id, name);
            this.dataGridViewResult.DataSource = this.dataSet.Tables[0];
        }

        private void dataGridViewResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Course course = new Course();
            course.courId = this.dataSet.Tables[0].Rows[e.RowIndex][0].ToString();
            course.courName = this.dataSet.Tables[0].Rows[e.RowIndex][1].ToString();
            course.credit = Convert.ToSingle(this.dataSet.Tables[0].Rows[e.RowIndex][2]);
            course.remark = this.dataSet.Tables[0].Rows[e.RowIndex][3].ToString();

            UpdateCourseForm updateCourseForm = new UpdateCourseForm(this.dataSet, course);
            updateCourseForm.ShowDialog();
        }
    }
}
