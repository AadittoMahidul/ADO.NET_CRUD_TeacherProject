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
    public partial class AddTeacher : Form
    {
        string filePath = "";
        List<Teacher> Teachers = new List<Teacher>();
        public AddTeacher()
        {
            InitializeComponent();
        }
        public ICrossDataFormLoadSync FormToReload { get; set; }
        private void AddTeacher_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = this.GetNewTeacherId().ToString();
        }
        private int GetNewTeacherId()
        {
            using (SqlConnection con = new SqlConnection(ConnectionHelper.ConString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(teacherid), 0) FROM Teachers", con))
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

                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Teachers 
                                            (teacherid, name, joiningdate, salary, address,phone, isWorking, picture) VALUES
                                            (@i, @n, @d, @s, @a, @p, @w,@b)", con, tran))
                    {
                        cmd.Parameters.AddWithValue("@i", int.Parse(textBox1.Text));
                        cmd.Parameters.AddWithValue("@n", textBox2.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@s", decimal.Parse(textBox3.Text));
                        cmd.Parameters.AddWithValue("@a", textBox4.Text);
                        cmd.Parameters.AddWithValue("@p", textBox5.Text);
                        cmd.Parameters.AddWithValue("@w", checkBox1.Checked);
                        string ext = Path.GetExtension(this.filePath);
                        string fileName = $"{Guid.NewGuid()}{ext}";
                        string savePath = Path.Combine(Path.GetFullPath(@"..\..\Pictures"), fileName);
                        File.Copy(filePath, savePath, true);
                        cmd.Parameters.AddWithValue("@b", fileName);

                        try
                        {
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show("Data Saved", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                Teachers.Add(new Teacher
                                {
                                    TeacherId = int.Parse(textBox1.Text),
                                    Name = textBox2.Text,
                                    JoinDate = dateTimePicker1.Value,
                                    Salary = decimal.Parse(textBox3.Text),
                                    Address = textBox4.Text,
                                    Phone = textBox5.Text,
                                    IsWorking = checkBox1.Checked,
                                    Picture = fileName
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.filePath = this.openFileDialog1.FileName;
                this.label7.Text = Path.GetFileName(this.filePath);
                this.pictureBox1.Image = Image.FromFile(this.filePath);
            }
        }

        private void AddTeacher_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.FormToReload.ReloadData(this.Teachers);
        }
    }
}
