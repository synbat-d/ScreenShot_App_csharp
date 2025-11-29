using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyScreenShotApp
{
    public partial class Form1 : Form
    {
        private string defaultSelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        private bool hideAppWindow = true;
        public Form1()
        {
            InitializeComponent();
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку для сохранения скриншота";
                folderDialog.ShowNewFolderButton = true;

                folderDialog.SelectedPath = defaultSelectedPath;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    defaultSelectedPath = selectedPath;
                    label1.Text = defaultSelectedPath;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = defaultSelectedPath;
            hideWindowCheckbox.Checked = hideAppWindow;
            Rectangle bounds = Screen.GetBounds(Point.Empty);
            int width = bounds.Width;
            int height = bounds.Height;

            resolutionComboBox.Items.Add($"Default ({width}x{height})");
            resolutionComboBox.Items.Add($"{width / 2}x{height / 2} ");
            resolutionComboBox.Items.Add($"{width / 4}x{height / 4} ");
            resolutionComboBox.Items.Add($"{width / 6}x{height / 6} ");

            resolutionComboBox.SelectedIndex = 0;
        }

        private Size GetSelectedResolution(Rectangle bounds) {
            string selected = resolutionComboBox.SelectedItem.ToString();

            if (selected.StartsWith("Default")) {
                return bounds.Size;
            }

            string[] parts = selected.Split('x');
            int w = int.Parse(parts[0]);
            int h = int.Parse(parts[1]);
            return new Size(w,h);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string folderPath = defaultSelectedPath;

                if (!Directory.Exists(folderPath))
                {
                    MessageBox.Show("Указанная папка не существует!");
                    return;
                }

                string fileName = "screenshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
                string fullPath = Path.Combine(folderPath, fileName);

                Rectangle bounds = Screen.GetBounds(Point.Empty);
                if (hideAppWindow) {
                    this.Hide();
                    System.Threading.Thread.Sleep(300);
                }
                Size targetSize = GetSelectedResolution(bounds);

                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    using (Bitmap resized = new Bitmap(bitmap, targetSize))
                    {
                        resized.Save(fullPath, ImageFormat.Png);
                    }
                    //bitmap.Save(fullPath, ImageFormat.Png);
                }

                if (hideAppWindow) {
                    this.Show();
                }

                MessageBox.Show("Скриншот сохранен: " + fullPath);
            }

            catch (Exception ex) {
                MessageBox.Show("Ошибка при сохранении скриншота: " + ex.Message);
            }
        }

        private void hideWindowCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            hideAppWindow = hideWindowCheckbox.Checked;
        }
    }
}
