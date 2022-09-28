using CRUD_Project_M6.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD_Project_M6
{
    public partial class FormTeacherProjectGrpRpt : Form
    {
        public FormTeacherProjectGrpRpt()
        {
            InitializeComponent();
        }

        private void FormTeacherProjectGrpRpt_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Teachers", con))
                {
                    da.Fill(ds, "Teachersi");
                    ds.Tables["Teachersi"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Teachersi"].Rows.Count; i++)
                    {
                        ds.Tables["Teachersi"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Teachersi"].Rows[i]["picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM Projects";
                    da.Fill(ds, "Projects");
                    TeacherProjectGrpRpt rpt = new TeacherProjectGrpRpt();
                    rpt.SetDataSource(ds);
                    crystalReportViewer1.ReportSource = rpt;
                    rpt.Refresh();
                    crystalReportViewer1.Refresh();
                }
            }
        }
    }
}
