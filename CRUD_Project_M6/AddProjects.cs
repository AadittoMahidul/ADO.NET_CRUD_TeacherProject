using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUD_Project_M6
{
    public partial class AddProjects : Form
    {
        List<Project> project = new List<Project>();
        public AddProjects()
        {
            InitializeComponent();
        }
        public ICrossDataFormLoadSync FormToSysnc { get; set; }
        private void AddProjects_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewProjectId().ToString();
        }
        private int GetNewProjectId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(projectid), 0) FROM projects", con))
                {
                    con.Open();
                    int id = (int)cmd.ExecuteScalar();
                    con.Close();
                    return id + 1;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO projects 
                                            (projectid, projectname, budget, isRunning, teacherid) VALUES
                                            (@i, @n, @b, @r, @e)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@b", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@r", checkBox1.Text);
                        cmd.Parameters.AddWithValue("@e", textBox5.Text);
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                project.Add(new Project
                                {
                                    ProjectId = int.Parse(textBox1.Text),
                                    ProjectName = textBox2.Text,
                                    Budget = decimal.Parse(textBox3.Text),
                                    IsRunning = checkBox1.Checked,
                                    TeacherId = int.Parse(textBox5.Text),
                                }); ;
                                tran.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            tran.Rollback();
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }

                    }
                        
                }

            }
        }

        private void AddProjects_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToSysnc.ReloadProject(this.project);
        }
    }
 }
