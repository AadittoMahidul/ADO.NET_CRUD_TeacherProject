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
    public partial class FromMain : Form, ICrossDataFormLoadSync
    {
        DataSet ds;
        BindingSource bsTeachers = new BindingSource();
        BindingSource bsProjects = new BindingSource();
        public FromMain()
        {
            InitializeComponent();
        }
        public void LoadData()
        {
            ds = new DataSet();
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Teachers", con))
                {
                    da.Fill(ds, "Teachers");
                    ds.Tables["Teachers"].Columns.Add(new DataColumn("image", typeof(System.Byte[])));
                    for (var i = 0; i < ds.Tables["Teachers"].Rows.Count; i++)
                    {
                        ds.Tables["Teachers"].Rows[i]["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), ds.Tables["Teachers"].Rows[i]["picture"].ToString()));
                    }
                    da.SelectCommand.CommandText = "SELECT * FROM projects";
                    da.Fill(ds, "projects");
                    DataRelation rel = new DataRelation("FK_TEACHER_PROJECT",
                        ds.Tables["Teachers"].Columns["teacherid"],
                        ds.Tables["projects"].Columns["teacherid"]);
                    ds.Relations.Add(rel);
                    ds.AcceptChanges();
                }
            }
        }
        private void BindData()
        {
            bsTeachers.DataSource = ds;
            bsTeachers.DataMember = "Teachers";
            bsProjects.DataSource = bsTeachers;
            bsProjects.DataMember = "FK_TEACHER_PROJECT";
            this.dataGridView1.DataSource = bsProjects;
            lblName.DataBindings.Add(new Binding("Text", bsTeachers, "name"));
            labelDate.DataBindings.Add(new Binding("Text", bsTeachers, "joiningdate"));
            lblSalary.DataBindings.Add(new Binding("Text", bsTeachers, "salary"));
            lblAddress.DataBindings.Add(new Binding("Text", bsTeachers, "address"));
            lblPhone.DataBindings.Add(new Binding("Text", bsTeachers, "phone"));
            checkBox1.DataBindings.Add(new Binding("Checked", bsTeachers, "isWorking"));
            pictureBox1.DataBindings.Add(new Binding("Image", bsTeachers, "image", true));
        }

        private void FromMain_Load(object sender, EventArgs e)
        {
            this.dataGridView1.AutoGenerateColumns = false;
            LoadData();
            BindData();
            showNavigation();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AddTeacher() { FormToReload = this }.ShowDialog();
        }

        public void ReloadData(List<Teacher> Teachers)
        {
            foreach (var e in Teachers)
            {
                DataRow dr = ds.Tables["Teachers"].NewRow();
                dr[0] = e.TeacherId;
                dr["name"] = e.Name;
                dr["joiningdate"] = e.JoinDate;
                dr["salary"] = e.Salary;
                dr["address"] = e.Address;
                dr["phone"] =e.Phone;
                dr["isWorking"] = e.IsWorking;
                dr["picture"] = e.Picture;
                dr["image"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), e.Picture));
                ds.Tables["Teachers"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            bsTeachers.MoveLast();
        }

        public void UpdateTeacher(Teacher Teachers)
        {
            for (var i = 0; i < ds.Tables["Teachers"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Teachers"].Rows[i]["teacherid"] == Teachers.TeacherId)
                {
                    ds.Tables["Teachers"].Rows[i]["name"] = Teachers.Name;
                    ds.Tables["Teachers"].Rows[i]["joiningdate"] = Teachers.JoinDate;
                    ds.Tables["Teachers"].Rows[i]["salary"] = Teachers.Salary;
                    ds.Tables["Teachers"].Rows[i]["address"] = Teachers.Address;
                    ds.Tables["Teachers"].Rows[i]["phone"] = Teachers.Phone;
                    ds.Tables["Teachers"].Rows[i]["picture"] = File.ReadAllBytes(Path.Combine(Path.GetFullPath(@"..\..\Pictures"), Teachers.Picture));
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveTeacher(int id)
        {
            for (var i = 0; i < ds.Tables["Teachers"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Teachers"].Rows[i]["Teacherid"] == id)
                {
                    ds.Tables["Teachers"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bsTeachers.MoveLast();
            showNavigation();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bsTeachers.Position < bsTeachers.Count - 1)
            {
                bsTeachers.MoveNext();
                showNavigation();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bsTeachers.Position > 0)
            {
                bsTeachers.MovePrevious();
                showNavigation();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bsTeachers.MoveFirst();
            showNavigation();
        }
        private void showNavigation()
        {
            this.lblOf.Text = (bsTeachers.Position + 1).ToString();
            this.lblTotal.Text = bsTeachers.Count.ToString();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsTeachers.Current as DataRowView).Row[0];
            new EditTeachers { TeacherToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            new AddTeacher() { FormToReload = this }.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsTeachers.Current as DataRowView).Row[0];
            new EditTeachers { TeacherToEditDelete = id, FormToReload = this }.ShowDialog();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            new AddProjects { FormToSysnc = this }.ShowDialog();
        }

        public void ReloadProject(List<Project> project)
        {
            foreach (var e in project)
            {
                DataRow dr = ds.Tables["projects"].NewRow();
                dr[0] = e.ProjectId;
                dr["projectname"] = e.ProjectName;
                dr["budget"] = e.Budget;
                dr["isRunning"] = e.IsRunning;
                dr["teacherid"] = e.TeacherId;
                ds.Tables["projects"].Rows.Add(dr);

            }
            ds.AcceptChanges();
            bsTeachers.MoveLast();
        }

        public void UpdateProject(Project p)
        {
            for (var i = 0; i < ds.Tables["Projects"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Projects"].Rows[i]["projectid"] == p.ProjectId)
                {
                    ds.Tables["Projects"].Rows[i]["name"] = p.ProjectName;
                    ds.Tables["Projects"].Rows[i]["joiningdate"] = p.Budget;
                    ds.Tables["Projects"].Rows[i]["salary"] = p.IsRunning;
                    ds.Tables["Projects"].Rows[i]["address"] = p.TeacherId;
                    break;
                }
            }
            ds.AcceptChanges();
        }

        public void RemoveProject(int id)
        {
            for (var i = 0; i < ds.Tables["Projects"].Rows.Count; i++)
            {
                if ((int)ds.Tables["Projects"].Rows[i]["projectid"] == id)
                {
                    ds.Tables["Projects"].Rows.RemoveAt(i);
                    break;
                }
            }
            ds.AcceptChanges();
        }
        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            int id = (int)(this.bsProjects.Current as DataRowView).Row[0];
            new EditDeleteProjects { ProjectToEditDelete = id, FormToReloaded = this }.ShowDialog();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddProjects { FormToSysnc = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new EditDeleteProjects { FormToReloaded = this }.ShowDialog();
        }

        private void toolStripMenuItem8_Click_1(object sender, EventArgs e)
        {
            int id = (int)(this.bsProjects.Current as DataRowView).Row[0];
            new EditDeleteProjects { ProjectToEditDelete = id, FormToReloaded = this }.ShowDialog();
        }

        private void groupRptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormTeacherProjectGrpRpt().ShowDialog();
        }

        private void teachersiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormTeacherRpt().ShowDialog();
        }

        private void lblTotal_Click(object sender, EventArgs e)
        {

        }
    }
}
