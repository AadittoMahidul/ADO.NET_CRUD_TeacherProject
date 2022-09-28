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
    public partial class EditTeachers : Form
    {
        string filePath, oldFile, fileName;
        string action = "Edit";
        Teacher Teacher;
        public EditTeachers()
        {
            InitializeComponent();
        }
        public int TeacherToEditDelete { get; set; }
        public ICrossDataFormLoadSync FormToReload { get; set; }
        
        private void button3_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.action = "Update";
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                con.Open();
                using (SqlTransaction tran = con.BeginTransaction())
                {

                    using (SqlCommand cmd = new SqlCommand(@"UPDATE Teachers  
                                            SET name=@n, joiningdate=@d, salary= @s, address=@a,phone=@p,isWorking=@w, picture=@pi 
                                            WHERE teacherid=@i", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text.ToString()));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@s", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@a", textBox4.Text);
                        cmd.Parameters.AddWithValue("@p", textBox5.Text);
                        cmd.Parameters.AddWithValue("@w", checkBox1.Checked);
                        if (!string.IsNullOrEmpty(this.filePath))
                        {
                            string ext = Path.GetExtension(this.filePath);
                            fileName = $"{Guid.NewGuid()}{ext}";
                            string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                            File.Copy(filePath, savePath, true);
                            cmd.Parameters.AddWithValue("@pi", fileName);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@pi", oldFile);
                        }

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Teacher = new Teacher
                                {
                                    TeacherId = int.Parse(textBox1.Text),
                                    Name = textBox2.Text,
                                    JoinDate = dateTimePicker1.Value,
                                    Salary = decimal.Parse(textBox3.Text),
                                    Address = textBox4.Text,
                                    Phone = textBox5.Text,
                                    IsWorking = checkBox1.Checked,
                                    Picture = filePath == "" ? oldFile : fileName
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void EditTeachers_Load(object sender, EventArgs e)
        {
            ShowData();
        }
        private void ShowData()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Teachers WHERE teacherid =@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", this.TeacherToEditDelete);
                    con.Open();
                    var dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetInt32(0).ToString();
                        textBox2.Text = dr.GetString(1);
                        dateTimePicker1.Value = dr.GetDateTime(2);
                        textBox3.Text = dr.GetDecimal(3).ToString("0.00");
                        textBox4.Text = dr.GetString(4);
                        textBox5.Text = dr.GetString(5);
                        checkBox1.Checked = dr.GetBoolean(6);
                        oldFile = dr.GetString(7).ToString();
                        pictureBox1.Image = Image.FromFile(Path.Combine(@"..\..\Pictures", dr.GetString(7).ToString()));
                    }
                    con.Close();
                }
            }

        }
        private void EditTeachers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.action == "Update")
                this.FormToReload.UpdateTeacher(Teacher);
            else
                this.FormToReload.RemoveProject(Int32.Parse(this.textBox1.Text));
        }
    }
}
