using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contracts;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Repository
{
    public class StudentRepository : IStudentRepository
    {
        public object RetrieveStudents(string StudentId)
        {

            string SampleconnString = ConfigurationManager.ConnectionStrings["SampleConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(SampleconnString))
            {
                List<Data.Models.Student> companyList = new List<Data.Models.Student>();
                SqlCommand cmd = new SqlCommand("StudentProcUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentId", StudentId);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                // DataTable dt = new DataTable();
                conn.Open();
                da.Fill(ds);
                // from rows in dataSet.Tables[0].AsEnumerable()
                var studentList = (from rows in ds.Tables[0].AsEnumerable()

                                   select new Data.Models.Student()
                                   {

                                       StudentId = Convert.ToInt32(rows[0]),
                                       StudentUserName = rows[1].ToString(),
                                       Studentpassword = rows[2].ToString(),
                                       StudentAge = Convert.ToInt32(rows[3]),
                                       StudentQualification = rows[4].ToString()
                                   }).ToList();
                return studentList;
            }

        }
    }
}
