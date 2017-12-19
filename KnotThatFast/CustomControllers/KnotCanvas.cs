using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KnotThatFast.Models;

namespace KnotThatFast.CustomControllers
{
    public partial class KnotCanvas : UserControl
    {
        private Graphics g = null;
        private Bitmap drawArea = null;
        private List<MovablePoint> points = new List<MovablePoint>();
        public bool closed = false;

        public KnotCanvas()
        {
            InitializeComponent();
        }

        private void KnotCanvas_Load(object sender, EventArgs e)
        {
            drawArea = new Bitmap(canvas_pic.Size.Width, canvas_pic.Size.Height);
            g = Graphics.FromImage(drawArea);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
        }

        private void canvas_pic_MouseClick(object sender, MouseEventArgs e)
        {
            if(!closed)
            {
                MovablePoint movPoint = new MovablePoint(new Point(e.X, e.Y));
                movPoint.MouseMove += MovPoint_MouseMove;
                points.Add(movPoint);
                canvas_pic.Controls.Add(movPoint);

                if (points.Count >= 2)
                {
                    g.Clear(Color.White);
                    g.DrawCurve(new Pen(Color.Black), points.Select(p => p.Position).ToArray());

                }
                canvas_pic.Image = drawArea; 
            }
        }

        private void MovPoint_MouseMove(object sender, MouseEventArgs e)
        {
            if (points.Count >= 2)
            {
                g.Clear(Color.White);
                if(!closed)
                    g.DrawCurve(new Pen(Color.Black), points.Select(p => p.Position).ToArray());
                else
                    g.DrawClosedCurve(new Pen(Color.Black), points.Select(p => p.Position).ToArray());
                canvas_pic.Image = drawArea;
            }
        }

        private void LineIntersect(Point p0, Point p1, Point p2, Point p3, out Point? intersectP)
        {
            intersectP = null;
            Point s1 = new Point(p1.X - p0.X, p1.Y - p0.Y);
            Point s2 = new Point(p3.X - p2.X, p3.Y - p2.Y);

            float denom = -s2.X * s1.Y + s1.X * s2.Y;
            float s = (-s1.Y * (p0.X - p2.X) + s1.X * (p0.Y - p2.Y)) / denom;
            float t = (s2.X * (p0.Y - p2.Y) - s2.Y * (p0.X - p2.X)) / denom;

            if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
            {
                intersectP = new Point(p0.X + (int)(t * s1.X), p0.Y + (int)(t * s1.Y));
            }

        }

        private void DrawIntersections()
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                Point p1 = points[i].Position;
                Point p2 = points[i + 1].Position;
                for (int j = i + 1; j < points.Count - 1; j++)
                {
                    Point p3 = points[j].Position;
                    Point p4 = points[j + 1].Position;
                    Point? intersect;
                    LineIntersect(p1, p2, p3, p4, out intersect);
                    if (intersect != null && intersect != p1 && intersect != p2 && intersect != p3 && intersect != p4)
                    {
                        g.DrawRectangle(new Pen(Color.Green), intersect.Value.X - 5, intersect.Value.Y - 5, 10, 10);
                    }
                }
            }
        }

        public Knot GetKnot()
        {
            throw new NotImplementedException();
        }

        public void Step()
        {
            throw new NotImplementedException();
        }

        public void Solve()
        {
            throw new NotImplementedException();
        }

        public void CloseKnot()
        {
            if (points.Count >= 2 && !closed)
            {
                g.Clear(Color.White);
                g.DrawClosedCurve(new Pen(Color.Black), points.Select(p => p.Position).ToArray());
                canvas_pic.Image = drawArea;
                closed = true;
            }
            
        }

        public void Clear()
        {
            points.Clear();
            g.Clear(Color.White);
            canvas_pic.Controls.Clear();
            closed = false;
            canvas_pic.Image = drawArea;
        }

        public Image GetImage()
        {
            return canvas_pic.Image;
        }

    }
}
