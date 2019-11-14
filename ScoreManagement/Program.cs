using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreManagement
{
    public struct StuInfo
    {
        public string id;
        public string name;
        public string major;
        public string birth;
        public string gender;
        public float totalScore;
        public string remark;
    }

    public struct Score
    {
        public string stuId;
        public string sutName;
        public string courName;
        public float credit;
        public float grade;
        public string remark;
    }

    public struct Course
    {
        public string courId;
        public string courName;
        public string remark;
        public float credit;
    }

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Action());
        }
    }
}
