using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;


namespace ScoreManagement
{
    class OperationDB
    {
        private string conStr;
        private MySqlConnection sqlConnection;
        public OperationDB(string conStr)
        {
            this.conStr = conStr;
        }

        public bool Open()
        {
            this.sqlConnection = new MySqlConnection(this.conStr);
            try
            {
                this.sqlConnection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                this.sqlConnection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataSet GetAllStuInfo(string para1, string para2, string para3)
        {
            string sql = @"SELECT
                        	s.id '学号', s.name '姓名', m.name '专业', s.gender '性别', 
                            DATE_FORMAT(s.birth, '%Y/%m/%d') '出生日期', s.totalscore '总分', 
                            s.remark '备注'
                        FROM
                        	scoremanagement.stu_info s
                        INNER JOIN
                        	scoremanagement.major m 
                        ON
                        	s.major_id = m.id
                        WHERE s.id LIKE @para1
                        AND s.name LIKE @para2
                        AND m.name LIKE @para3
                        ORDER BY s.id;";
            return this.ExecDataSet(sql, para1, para2, para3);
        }

        public DataSet GetStuScoreDataSet(string id)
        {
            string sql = @"SELECT
                            st.`id` '学号', st.`name` '姓名', co.`name` '课程', sc.`grade` '成绩', co.`credit` '学分', co.`remark` '备注'
                        FROM
                            scoremanagement.`stu_info` st
                        INNER JOIN
                            scoremanagement.`score` sc
                        INNER JOIN
                            scoremanagement.`course` co
                        ON
                            sc.`stu_id` = st.`id`
                        AND
                            sc.`cou_id` = co.`id`
                        WHERE
                            sc.`stu_id` = @para1; ";
            return this.ExecDataSet(sql, id);
        }

        public bool GetOneStuInfoStruct(string id, ref StuInfo stu)
        {
            DataSet dataSet = this.GetAllStuInfo(id, "%", "%");
            try
            {
                stu.id = dataSet.Tables[0].Rows[0][0].ToString();
                stu.name = dataSet.Tables[0].Rows[0][1].ToString();
                stu.major = dataSet.Tables[0].Rows[0][2].ToString();
                stu.gender = dataSet.Tables[0].Rows[0][3].ToString();
                stu.birth = dataSet.Tables[0].Rows[0][4].ToString();
                stu.totalScore = Convert.ToSingle(dataSet.Tables[0].Rows[0][5]);
                stu.remark = dataSet.Tables[0].Rows[0][6].ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public DataSet GetAllCourse()
        {
            string sql = @"SELECT `id`, `name`, `credit`, `remark` FROM `scoremanagement`.`course`;";
            return this.ExecDataSet(sql);
        }


        public int UpdateStuInfo(StuInfo stu)
        {
            string sql = @"UPDATE
                        	scoremanagement.`stu_info` st
                        SET
                        	st.`name` = @para1,
                        	st.`gender` = @para2,
                        	st.`birth` = STR_TO_DATE(@para3,'%Y/%m/%d'),
                        	st.`major_id` = (
                        	SELECT 
                        		id 
                        	FROM 
                        		scoremanagement.`major` ma 
                        	WHERE 
                        		ma.`NAME` = @para4
                        	),
                        	st.`totalscore` = @para5,
                        	st.`remark` = @para6
                        WHERE
                        	st.`id` = @para7;";
            return this.ExecNonQuery(sql, stu.name, stu.gender, stu.birth, stu.major, 
                stu.totalScore.ToString(), stu.remark, stu.id);
        }

        public int DeleteStuInfo(StuInfo stu)
        {
            string sql = @"DELETE FROM
                            	scoremanagement.`stu_info`
                            WHERE
                            	id = @para1;";
            return this.ExecNonQuery(sql, stu.id);
        }

        public int InsertStuInfo(StuInfo stu)
        {
            string sql = @"INSERT INTO
                            	scoremanagement.`stu_info`(`id`, `name`, `major_id`, `gender`, `remark`, `birth`, `totalscore`)
                            VALUES
                            	(@para1, @para2, (
                            	SELECT 
                            		id 
                            	FROM 
                            		scoremanagement.`major` ma 
                            	WHERE 
                            		ma.`name` = @para3
                            	), @para4, @para5, STR_TO_DATE(@para6,'%Y/%m/%d'), @para7);";
            return this.ExecNonQuery(sql, stu.id, stu.name, stu.major, stu.gender, stu.remark, stu.birth, stu.totalScore.ToString());
        }
        
        public int InesrtScoreRecord(Score score)
        {
            string sql = @"INSERT INTO 
                            	`scoremanagement`.`score`(`stu_id`, `cou_id`, `grade`, `remark`)
                            VALUES (@para1, 
                            	(SELECT 
                            		id 
                            	FROM 
                            		`scoremanagement`.`course` 
                            	WHERE 
                            		name = @para2
                            	), @para3, @para4);";
            return this.ExecNonQuery(sql, score.stuId, score.courName, score.grade.ToString(), score.remark);
        }


        public int UpdateScoreRecord(Score score)
        {
            string sql = @"UPDATE
                            	`scoremanagement`.`score` sc
                            INNER JOIN
                            	`scoremanagement`.`course` co
                            ON
                            	sc.`cou_id` = co.`id`
                            SET
                            	sc.`grade` = @para3
                            WHERE
                            	sc.`stu_id` = @para1
                            AND
                            	co.`name` = @para2;";
            return this.ExecNonQuery(sql, score.stuId, score.courName, score.grade.ToString());
        }

        public int DeleteScoreRecord(Score score)
        {
            string sql = @"DELETE 
                            	`scoremanagement`.`score`
                            FROM 
                            	`scoremanagement`.`score`
                            INNER JOIN
                            	`scoremanagement`.`course`
                            ON 
                            	score.`cou_id` = course.`id`
                            WHERE
                            	score.`stu_id` = @para1
                            AND
                            	course.`name` = @para2;";
            return this.ExecNonQuery(sql, score.stuId, score.courName);
        }

        public bool SelectScoreRecord(ref Score score)
        {
            string sql = @"SELECT
                            	sc.`stu_id` 'stuId', co.`name` 'courName', sc.`grade` 'grade', sc.`remark` 'remark'
                            FROM
                            	`scoremanagement`.`score` sc
                            INNER JOIN
                            	`scoremanagement`.`course` co
                            ON
                            	sc.`cou_id` = co.`id`
                            WHERE
                            	sc.`stu_id` = @para1
                            AND
                            	co.`name` = @para2;";
            MySqlDataReader mySqlDataReader = this.ExecDataReader(sql, score.stuId, score.courName);
            if (mySqlDataReader == null)
            {
                return false;
            }
            if (mySqlDataReader.HasRows)
            {
                try
                {
                    mySqlDataReader.Read();
                    score.stuId = mySqlDataReader.GetString("stuId");
                    score.courName = mySqlDataReader.GetString("courName");
                    score.grade = Convert.ToSingle(mySqlDataReader.GetString("grade"));
                    score.remark = mySqlDataReader.GetString("remark");
                }
                catch { }
                finally
                {
                    mySqlDataReader.Close();
                }
                return true;
            }
            mySqlDataReader.Close();
            return false;
        }


        public int InesrtCourseRecord(Course course)
        {
            string sql = @"INSERT INTO 
                            	`scoremanagement`.`course`(`id`, `name`, `credit`, `remark`)
                            VALUES (@para1, @para2, @para3, @para4);";
            return this.ExecNonQuery(sql, course.courId, course.courName, course.credit.ToString(), course.remark);
        }


        public int UpdateCourseRecord(Course course)
        {
            string sql = @"UPDATE
                            	`scoremanagement`.`course` cou
                            SET
                            	cou.`name` = @para2,
                            	cou.`credit` = @para3,
                            	cou.`remark` = @para4
                            WHERE
                            	cou.`id` = @para1;";
            return this.ExecNonQuery(sql, course.courId, course.courName, course.credit.ToString(), course.remark);
        }

        public int DeleteCourseRecord(Course course)
        {
            string sql = @"DELETE 
                            	`scoremanagement`.`course`
                            FROM 
                            	`scoremanagement`.`course`
                            WHERE
                            	course.`id` = @para1;";
            return this.ExecNonQuery(sql, course.courId);
        }

        public bool SelectCourseRecord(ref Course course)
        {
            string sql = @"SELECT
                            	cou.`id` '课程号', cou.`name` '课程名', cou.`credit` '学分', cou.`remark` '备注'
                            FROM
                            	`scoremanagement`.`course` cou
                            WHERE
                            	cou.`id` = @para1;";
            MySqlDataReader mySqlDataReader = this.ExecDataReader(sql, course.courId);
            if (mySqlDataReader == null)
            {
                return false;
            }
            if (mySqlDataReader.HasRows)
            {
                try
                {
                    mySqlDataReader.Read();
                    course.courId = mySqlDataReader.GetString("课程号");
                    course.courName = mySqlDataReader.GetString("课程名");
                    course.credit = Convert.ToSingle(mySqlDataReader.GetString("学分"));
                    course.remark = mySqlDataReader.GetString("备注");
                }
                catch { }
                finally
                {
                    mySqlDataReader.Close();
                }
                return true;
            }
            mySqlDataReader.Close();
            return false;
        }


        public DataSet GetAllCourseInfo(string id, string name)
        {
            string sql = @"SELECT
                            	co.`id` '课程号', co.`name` '课程名', co.`credit` '学分', co.`remark` '备注'
                            FROM
                            	`scoremanagement`.`course` co
                            WHERE
                            	co.`id` LIKE @para1
                            AND
                            	co.`name` LIKE @para2;";
            return this.ExecDataSet(sql, id, name);
        }

        public Dictionary<string, string> GetMapIdName(string key, string value ,string table)
        {
            string sql = "SELECT `" + key + "`, `" + value + "` FROM `scoremanagement`.`" + table + "`;";
            MySqlDataReader mySqlDataReader = this.ExecDataReader(sql, table);
            if (mySqlDataReader != null && mySqlDataReader.HasRows)
            {
                Dictionary<string, string> map = new Dictionary<string, string>();
                while (mySqlDataReader.Read())
                {
                    map.Add(mySqlDataReader.GetString(key), mySqlDataReader.GetString(value));
                }
                mySqlDataReader.Close();
                return map;
            }
            return null;
        }

        public int ExecNonQuery(string sql, params string[] paras)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, this.sqlConnection);
            int index = 1;
            string prefix = "para";
            foreach (string para in paras)
            {
                string replace = prefix + index.ToString();
                try
                {
                    mySqlCommand.Parameters.AddWithValue(replace, para);
                }
                catch
                {
                    break;
                }
                index++;
            }
            //try
            //{
                int result = mySqlCommand.ExecuteNonQuery();
                return result;
            //}
            //catch
            //{
            //    return -1;
            //}
        }

        public MySqlDataReader ExecDataReader(string sql, params string[] paras)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, this.sqlConnection);
            int index = 1;
            string prefix = "para";
            foreach(string para in paras)
            {
                string replace = prefix + index.ToString();
                try
                {
                    mySqlCommand.Parameters.AddWithValue(replace, para);
                }
                catch
                {
                    break;
                }
                index++;
            }
            try
            {
                MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
                return mySqlDataReader;
            }
            catch
            {
                return null;
            }
        }

        public DataSet ExecDataSet(string sql, params string[] paras)
        {
            MySqlCommand mySqlCommand = new MySqlCommand(sql, this.sqlConnection);
            int index = 1;
            string prefix = "para";
            foreach (string para in paras)
            {
                string replace = prefix + index.ToString();
                try
                {
                    mySqlCommand.Parameters.AddWithValue(replace, para);
                }
                catch
                {
                    break;
                }
                index++;
            }
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter(mySqlCommand);
            DataSet dataSet = new DataSet();
            try
            {
                mySqlDataAdapter.Fill(dataSet);
                return dataSet;
            }
            catch
            {
                return null;
            }
        }
    }
}
