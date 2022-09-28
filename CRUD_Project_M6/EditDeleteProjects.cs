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
    public partial class EditDeleteProjects : Form
    {
        string action = "Edit";
        Project project;
        public EditDeleteProjects()
        {
            InitializeComponent();
        }
        public int ProjectToEditDelete { get; set; }
        public ICrossDataFormLoadSync FormToReloaded { get; set; }

        private void EditDeleteProjects_Load(object sender, EventArgs e)
        {
            ShowData();
        }

        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM projects WHERE projectid =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.ProjectToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        textBox3.Text = dr.GetDecimal(2).ToString("0.00");
                        checkBox1.Checked = dr.GetBoolean(3);
                        textBox4.Text = dr.GetInt32(4).ToString();
                    }
                    con.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Save";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE Projects  
                                            SET  projectname=@n, budget=@b, isRunning= @r, teacherid=@ei 
                                            WHERE projectid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@b", textBox3.Text);
                        cmd.Parameters.AddWithValue("@r", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@ei", int.Parse (textBox4.Text));
                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                project = new Project
                                {
                                    ProjectId = int.Parse(textBox1.Text),
                                    ProjectName = textBox2.Text,
                                    Budget = decimal.Parse(textBox3.Text),
                                    IsRunning = checkBox1.Checked,
                                    TeacherId = int.Parse(textBox4.Text),
                                };
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

        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.action = "Delete";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"DELETE  projects  
                                            WHERE projectid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));




                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Deleted", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void EditDeleteProjects_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "edit")
                this.FormToReloaded.UpdateProject(project);
            else
                this.FormToReloaded.RemoveProject(Int32.Parse(this.textBox1.Text));
        }

        private void button9_Click(object sender, EventArgs e)
        {
          
        }
    }
}
