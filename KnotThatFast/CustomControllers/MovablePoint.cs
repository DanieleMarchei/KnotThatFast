using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnotThatFast.CustomControllers
{
    public partial class MovablePoint : UserControl
    {

        public Point Position { get { return new Point(Location.X + Width/2, Location.Y + Height/2); } }
        public int X { get { return Location.X; } }
        public int Y { get { return Location.Y; } }

        private bool IsMoving = false;
        private Point CursorOffset = new Point();

        public MovablePoint(Point p)
        {
            InitializeComponent();
            Location = new Point(p.X - Width/2, p.Y - Height/2);
        }

        public MovablePoint(Point p, Color color) : this(p)
        {
            BackColor = color;
        }

        private void MovablePoint_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                IsMoving = true;
                CursorOffset = e.Location;
            }
        }

        private void MovablePoint_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMoving)
            {
                Point p = Parent.PointToClient(Cursor.Position);
                Location = new Point(p.X - CursorOffset.X - Width/2, p.Y - CursorOffset.Y - Height/2);
            }
        }

        private void MovablePoint_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsMoving)
                IsMoving = false;
        }
    }
}
