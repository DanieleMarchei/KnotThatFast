using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KnotThatFast.CustomControllers;

namespace KnotThatFast
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void saveImg_btn_Click(object sender, EventArgs e)
        {
            Image img = knotCanvas.GetImage();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "Save knot image";
            saveDialog.Filter = "PNG | *.png";
            if(saveDialog.ShowDialog() == DialogResult.OK)
            {
                img.Save(saveDialog.FileName,ImageFormat.Png);
            }
        }

        private void clear_btn_Click(object sender, EventArgs e)
        {
            knotCanvas.Clear();
        }

        private void close_btn_Click(object sender, EventArgs e)
        {
            knotCanvas.CloseKnot();
        }
    }
}
