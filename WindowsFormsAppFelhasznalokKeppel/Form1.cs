
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MySql.Data.MySqlClient;

namespace WindowsFormsAppFelhasznalokKeppel
{
    public partial class Form1 : Form
    {
        MySqlConnection conn = null;
        MySqlCommand cmd = null;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
            builder.Server = "localhost";
            builder.UserID = "root";
            builder.Password = "";
            builder.Database = "felhasznalokkeppel";
            conn = new MySqlConnection(builder.ConnectionString);
            try
            {
                conn.Open();
                cmd = conn.CreateCommand();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + "A program leáll!");
                Environment.Exit(0);
            }
            finally
            {
                conn.Close();
            }
            User_list_update();
        }

        private void User_list_update()
        {
            listBox1.Items.Clear();
            cmd.CommandText = "SELECT * FROM `felhasznalokkeppel` WHERE 1";
            conn.Open();
            using (MySqlDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    KepCuccos uj = new KepCuccos(dr.GetInt32("id"), dr.GetString("nev"), dr.GetDateTime("datum"),
                     dr.IsDBNull(3) ? null : Image.FromStream(new MemoryStream(dr.GetFieldValue<byte[]>(3))));
                    listBox1.Items.Add(uj);
                }
            }
            conn.Close();
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxName.Text))
            {
                MessageBox.Show("Adja meg a felhasználó nevét!");
                textBoxName.Focus();
                return;
            }
            if (dateTimePickerDate.Value == DateTime.MinValue)
            {
                MessageBox.Show("Adja meg a szültési dátumát!");
                dateTimePickerDate.Focus();
                return;
            }
            cmd.CommandText = "INSERT INTO `felhasznalokkeppel` (`id`, `nev`, `datum`,`kep`) VALUES (NULL, @nev, @datum, @kep);";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", textBoxID.Text);
            cmd.Parameters.AddWithValue("@nev", textBoxName.Text);
            cmd.Parameters.AddWithValue("@datum", dateTimePickerDate.Value);
            cmd.Parameters.AddWithValue("@kep", Program.ImagesToByte(pictureBoxPicture.Image));
            conn.Open();
            try
            {
                if (cmd.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Sikeresen rögzítve");
                    textBoxID.Text = "";
                    textBoxName.Text = "";
                    dateTimePickerDate.Value = dateTimePickerDate.MinDate;
                }
                else
                {
                    MessageBox.Show("Sikertelen rögzítés!");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            conn.Close();
            User_list_update();
        }

        private void buttonEdit_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show("Nincs kijelölve számla!");
                return;
            }
            cmd.CommandText = "UPDATE `felhasznalokkeppel` SET `nev` = @nev, `datum` = @datum,`kep`=@kep WHERE `felhasznalokkeppel`.`id` = @id;";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", textBoxID.Text);
            cmd.Parameters.AddWithValue("@nev", textBoxName.Text);
            cmd.Parameters.AddWithValue("@datum", dateTimePickerDate.Value);
            cmd.Parameters.AddWithValue("@kep", Program.ImagesToByte(pictureBoxPicture.Image));
            conn.Open();
            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Módosítás sikeres!");
                conn.Close();
                textBoxID.Text = "";
                textBoxName.Text = "";
                dateTimePickerDate.Value = dateTimePickerDate.MinDate;
                User_list_update();
            }
            else
            {
                MessageBox.Show("Az adatok modosítása sikerleten!");
            }
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            cmd.CommandText = "DELETE FROM `felhasznalokkeppel` WHERE `id` = @id";
            cmd.Parameters.AddWithValue("@id", textBoxID.Text);
            cmd.Parameters.AddWithValue("@nev", textBoxName.Text);
            cmd.Parameters.AddWithValue("@datum", dateTimePickerDate.Value);
            cmd.Parameters.AddWithValue("@kep", Program.ImagesToByte(pictureBoxPicture.Image));
            conn.Open();
            if (cmd.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Törlés sikeres!");
                conn.Close();
                textBoxID.Text = "";
                textBoxName.Text = "";
                dateTimePickerDate.Value = dateTimePickerDate.MinDate;
                User_list_update();
            }
            else
            {
                MessageBox.Show("Törlés sikertelen!");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }
            KepCuccos kivalaszott_felhasznalo = (KepCuccos)listBox1.SelectedItem;
            textBoxID.Text = kivalaszott_felhasznalo.Id.ToString();
            textBoxName.Text = kivalaszott_felhasznalo.Nev;
            dateTimePickerDate.Value = kivalaszott_felhasznalo.Datum;
            pictureBoxPicture.Image = kivalaszott_felhasznalo.Kep;
        }

        private void buttonPicture_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "*.jpg|*.jpg|*.png|*.png|*.webp|*.webp";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string kepFajl = openFileDialog1.FileName;
                pictureBoxPicture.Image = Image.FromFile(kepFajl);
            }
        }
    }
}
