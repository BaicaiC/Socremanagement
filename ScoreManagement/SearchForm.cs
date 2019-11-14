using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ScoreManagement
{
    public partial class SearchForm : Form
    {
        private OperationDB operationDB;
        private DataSet dataSet;

        public SearchForm()
        {
            InitializeComponent();
            dataSet = new DataSet();

            Config config = new Config();
            this.operationDB = new OperationDB(config.dbStr);
            this.operationDB.Open();

            searchMajor();
        }

        ~SearchForm()
        {
            try
            {
                operationDB.Close();
            }
            catch
            {
                ;
            }
        }

        private void searchMajor() {
            this.comboBoxMajor.ResetText();
            this.comboBoxMajor.Items.Add("所有专业");
            string sql = "select name from scoremanagement.major;";

            MySqlDataReader mySqlDataReader = operationDB.ExecDataReader(sql);
            if (mySqlDataReader != null && mySqlDataReader.HasRows)
            {
                string majorName;
                while (mySqlDataReader.Read())
                {
                    majorName = mySqlDataReader.GetString("name");
                    this.comboBoxMajor.Items.Add(majorName);
                }
            }
            mySqlDataReader.Close();
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            string para1 = this.textBoxNum.Text == "" ? "%" : this.textBoxNum.Text;
            string para2 = this.textBoxName.Text == "" ? "%" : this.textBoxName.Text;
            string para3 = this.comboBoxMajor.Text == "所有专业" ? "%" : this.comboBoxMajor.Text;

            this.dataSet = this.operationDB.GetAllStuInfo(para1, para2, para3);

            if (dataSet != null)
            {
                dataGridViewResult.DataSource = dataSet.Tables[0];
            }
        }

        private void dataGridViewResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ModifyForm modifyForm = new ModifyForm(this.dataSet);
            try
            {
                modifyForm.textBoxNum.Text = this.dataGridViewResult.Rows[e.RowIndex].Cells[0].Value.ToString();
                modifyForm.textBoxName.Text = this.dataGridViewResult.Rows[e.RowIndex].Cells[1].Value.ToString();
                modifyForm.textBoxMajor.Text = this.dataGridViewResult.Rows[e.RowIndex].Cells[2].Value.ToString();
                string gender = this.dataGridViewResult.Rows[e.RowIndex].Cells[3].Value.ToString();
                modifyForm.textBoxBirth.Text = this.dataGridViewResult.Rows[e.RowIndex].Cells[4].Value.ToString();
                modifyForm.textBoxTotalScore.Text = this.dataGridViewResult.Rows[e.RowIndex].Cells[5].Value.ToString();
                modifyForm.textBoxRemark.Text = this.dataGridViewResult.Rows[e.RowIndex].Cells[6].Value.ToString();
                if (gender == "女")
                {
                    modifyForm.radioButtonMan.Checked = false;
                    modifyForm.radioButtonWoman.Checked = true;
                }
                else
                {
                    modifyForm.radioButtonMan.Checked = true;
                    modifyForm.radioButtonWoman.Checked = false;
                }
                modifyForm.ShowDialog();
            }
            catch { }
        }

        private void dataGridViewResult_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                string id = this.dataGridViewResult.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataSet dataSet = new DataSet();
                dataSet = this.operationDB.GetStuScoreDataSet(id);
                ScoreForm scoreForm = new ScoreForm(dataSet);
                scoreForm.ShowDialog();
            }
            catch { }
        }
    }
}
