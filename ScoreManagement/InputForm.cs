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
    public partial class InputForm : Form
    {
        private Dictionary<string, string> _mapMajor;
        public Dictionary<string, string> mapMajor
        {
            get
            {
                return this._mapMajor;
            }
            set
            {
                this._mapMajor = value;
                this.InitMajor();
            }
        }

        private Dictionary<string, string> _mapCource;
        public Dictionary<string, string> mapCource
        {
            get
            {
                return this._mapCource;
            }
            set
            {
                this._mapCource = value;
                this.InitCourse();
            }
        }

        private Dictionary<string, string> _mapStu;
        public Dictionary<string, string> mapStu
        {
            get
            {
                return this._mapStu;
            }
            set
            {
                this._mapStu = value;
                this.InitStu();
            }
        }

        private OperationDB operationDB;

        private DataSet dataSet;
        public DataRow dataRow
        {
            get
            {
                return this.dataSet.Tables[0].NewRow();
            }
            set
            {
                this.dataSet.Tables[0].Rows.Add(value);
            }
        }
        public InputForm()
        {
            InitializeComponent();
            InitDataSet();
            this.dataGridViewRecord.DataSource = this.dataSet.Tables[0];

            Config config = new Config();
            this.operationDB = new OperationDB(config.dbStr);
            this.operationDB.Open();

            this._mapMajor = new Dictionary<string, string>();
            this.mapMajor = this.operationDB.GetMapIdName("name", "id", "major");
            this._mapCource = new Dictionary<string, string>();
            this.mapCource = this.operationDB.GetMapIdName("name", "credit", "course");
        }

        ~InputForm()
        {
            this.operationDB.Close();
        }

        private void InitDataSet()
        {
            this.dataSet = new DataSet();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("学号");
            dataTable.Columns.Add("姓名");
            dataTable.Columns.Add("课程");
            dataTable.Columns.Add("成绩");
            dataTable.Columns.Add("学分");
            dataTable.Columns.Add("备注");
            this.dataSet.Tables.Add(dataTable);
        }

        private void InitMajor()
        {
            if (this._mapMajor != null)
            {
                this.comboBoxMajor.Items.Clear();
                foreach (string key in this._mapMajor.Keys)
                {
                    this.comboBoxMajor.Items.Add(key);
                }
            }
        }

        private void InitCourse()
        {
            this.comboBoxCourse.Items.Clear();
            foreach(string key in this._mapCource.Keys)
            {
                this.comboBoxCourse.Items.Add(key);
            }
        }

        private void InitStu()
        {
            this.comboBoxNum.Items.Clear();
            foreach(string key in this._mapStu.Keys)
            {
                this.comboBoxNum.Items.Add(key);
            }
        }

        private Score GetScore()
        {
            Score score = new Score();
            try
            {
                score.stuId = this.comboBoxNum.Text;
                score.sutName = this.textBoxName.Text;
                score.credit = Convert.ToSingle(this.textBoxCredit.Text);
                score.courName = this.comboBoxCourse.Text;
                score.remark = this.textBoxRemark.Text;
                score.grade = Convert.ToSingle(this.textBoxGrade.Text);
            }
            catch { }
            return score;
        }

        private void SetScore(Score score)
        {
            this.comboBoxNum.Text = score.stuId;
            this.comboBoxCourse.Text = score.courName;
            this.textBoxName.Text = score.sutName;
            this.textBoxRemark.Text = score.remark;
            this.textBoxCredit.Text = score.credit.ToString();
            this.textBoxGrade.Text = score.grade.ToString();
        }

        private void addDataGridViewRecord(Score score)
        {
            DataRow temp = this.dataRow;
            temp["学号"] = score.stuId;
            temp["姓名"] = score.sutName;
            temp["课程"] = score.courName;
            temp["成绩"] = score.grade;
            temp["学分"] = score.grade.ToString();
            temp["备注"] = score.remark;
            this.dataRow = temp;
        }

        private bool subDataGridViewRecord(Score score)
        {
            for (int i= 0; i < this.dataSet.Tables[0].Rows.Count; i++)
            {
                if (this.dataSet.Tables[0].Rows[i][0].ToString() == score.stuId &&
                    this.dataSet.Tables[0].Rows[i][2].ToString() == score.courName)
                {
                    this.dataSet.Tables[0].Rows[i].Delete();
                    return true;
                }
            }
            return false;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            Score score = this.GetScore();
            Score getScore = this.GetScore();
            int result;
            if (this.operationDB.SelectScoreRecord(ref getScore))
            {
                result = this.operationDB.UpdateScoreRecord(score);
            }
            else
            {
                result = this.operationDB.InesrtScoreRecord(score);
            }
            if (result != -1)
            { 
                this.subDataGridViewRecord(score);
                this.addDataGridViewRecord(score);
            }
        }

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            Score score = this.GetScore();
            if (this.operationDB.DeleteScoreRecord(score) != -1)
            {
                if (!this.subDataGridViewRecord(score))
                {
                    MessageBox.Show("删除成功。", "提示");
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Score score = new Score();
            this.SetScore(score);
        }

        private void comboBoxMajor_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.comboBoxNum.Items.Clear();
            string major = this.comboBoxMajor.Text;
            DataSet dataSet = this.operationDB.GetAllStuInfo("%", "%", major);
            Dictionary<string, string> idAndName = new Dictionary<string, string>();
            if (dataSet != null)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    idAndName.Add(dataSet.Tables[0].Rows[i][0].ToString(), 
                        dataSet.Tables[0].Rows[i][1].ToString());
                }
            }
            this.mapStu = idAndName;
        }

        private void comboBoxNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            string num = this.comboBoxNum.Text;
            if (this.mapStu.ContainsKey(num))
            {
                this.textBoxName.Text = this.mapStu[num];
            }
            else
            {
                this.textBoxName.Text = "";
            }
        }

        private void comboBoxCourse_SelectedIndexChanged(object sender, EventArgs e)
        {
            string course = this.comboBoxCourse.Text;
            if (this.mapCource.ContainsKey(course))
            {
                this.textBoxCredit.Text = this.mapCource[course];
            }
            else
            {
                this.textBoxCredit.Text = "";
            }
        }

        private void dataGridViewRecord_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Score score = new Score();
                score.stuId = this.dataSet.Tables[0].Rows[e.RowIndex][0].ToString();
                score.sutName = this.dataSet.Tables[0].Rows[e.RowIndex][1].ToString();
                score.courName = this.dataSet.Tables[0].Rows[e.RowIndex][2].ToString();
                score.grade = Convert.ToSingle(this.dataSet.Tables[0].Rows[e.RowIndex][3]);
                score.credit = Convert.ToSingle(this.dataSet.Tables[0].Rows[e.RowIndex][4].ToString());
                score.remark = this.dataSet.Tables[0].Rows[e.RowIndex][5].ToString();
                this.SetScore(score);
            }
            catch { }
        }
    }
}
