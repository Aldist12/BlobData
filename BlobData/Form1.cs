﻿using System;
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

namespace BlobData
{
    public partial class Form1 : Form
    {
        //Declare the following class level variable :
        Image curImage;
        string curFileName;
        string connectionString = "data source = ALDYST; database = Blob; MultipleActiveResultSets =  True; User ID = sa; password = 123";
        // The savedImageName will store the path of the image file. //intialize the variable 
        string savedImageName = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                curFileName = openDlg.FileName;
                textBox1.Text = curFileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                FileStream file = new FileStream(curFileName, FileMode.OpenOrCreate, FileAccess.Read);
                byte[] rawdata = new byte[file.Length];
                file.Read(rawdata, 0, System.Convert.ToInt32(file.Length));
                file.Close();
                string sql = "SELECT * FROM Mahasiswa";

                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(adapter);
                DataSet ds = new DataSet("Mahasiswa");

                adapter.Fill(ds, "Mahasiswa");
                DataRow row = ds.Tables["Mahasiswa"].NewRow();
                row["Nim"] = 1;
                row["Nama"] = "SQL";
                row["Foto"] = rawdata;
                ds.Tables["Mahasiswa"].Rows.Add(row);
                adapter.Update(ds, "Mahasiswa");
                connection.Close();
                MessageBox.Show("Image saved");
            }
            else MessageBox.Show("Click th Browse button to select an image ");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                string sql = "SELECT Foto FORM Mahasiswa WHERE Nim ='1' ";
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                connection.Open();

                FileStream file;
                BinaryWriter bw;

                int bufferSize = 100;
                byte[] outbyte = new byte[bufferSize];
                long retval;
                long startIndex = 0;
                savedImageName = textBox1.Text;

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader myReader = command.ExecuteReader(CommandBehavior.SequentialAccess);

                while (myReader.Read())
                {
                    file = new FileStream(savedImageName, FileMode.OpenOrCreate, FileAccess.Write);
                    bw = new BinaryWriter(file);
                    startIndex = 0;
                    retval = myReader.GetBytes(0, startIndex, outbyte, 0, bufferSize);
                    while (retval == bufferSize)
                    {
                        bw.Write(outbyte);
                        bw.Flush();
                        startIndex += bufferSize;
                        retval = myReader.GetBytes(0, startIndex, outbyte, 0, bufferSize);
                    }
                    bw.Write(outbyte, 0, (int)retval - 1);
                    bw.Flush();
                    bw.Close();
                    file.Close();
                }
                connection.Close();
                curImage = Image.FromFile(savedImageName);
                pictureBox1.Image = curImage;
                pictureBox1.Invalidate();
                connection.Close();
            }
            else MessageBox.Show("Upload the image first");
        }
    }

}