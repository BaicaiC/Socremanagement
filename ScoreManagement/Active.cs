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
    public partial class Action : Form
    {
        public Action()
        {
            InitializeComponent();
        }

        private void 查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchForm searchForm = new SearchForm();
            searchForm.ShowDialog();
        }

        private void 修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyForm modifyForm = new ModifyForm();
            modifyForm.ShowDialog();
        }

        private void 录入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputForm inputForm = new InputForm();
            inputForm.ShowDialog();
        }

        private void 查询ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SearchCourseForm searchCourseForm = new SearchCourseForm();
            searchCourseForm.ShowDialog();
        }

        private void 修改ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            UpdateCourseForm updateCourseForm = new UpdateCourseForm();
            updateCourseForm.ShowDialog();
        }
    }
}
