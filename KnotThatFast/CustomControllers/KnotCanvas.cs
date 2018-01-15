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
using KnotThatFast.Utilities;
using System.Drawing.Drawing2D;

namespace KnotThatFast.CustomControllers
{
    public partial class KnotCanvas : UserControl
    {
        private Graphics g = null;
        private Bitmap drawArea = null;
        private List<MovablePoint> points = new List<MovablePoint>();
        public bool KnotIsClosed = false;
        public bool Closable { get { return points.Count >= 3 && !KnotIsClosed; } }
        private Knot Knot;
        private List<IntersectionPoint> OrderedIntersectionPoints = new List<IntersectionPoint>();

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
            if (!KnotIsClosed)
            {
                switch (e.Button)
                {
                    case MouseButtons.Left:
                        {
                            //under cross
                            MovablePoint movPoint = new MovablePoint(new Point(e.X, e.Y));
                            //movPoint.MouseMove += MovPoint_MouseMove;
                            points.Add(movPoint);
                            canvas_pic.Controls.Add(movPoint);

                            if (points.Count >= 2)
                            {
                                MovablePoint last = points.Last();
                                MovablePoint beforeLast = points.Skip(points.Count - 2).First();
                                List<Point> interPoints = GetIntersections(last.Position, beforeLast.Position);
                                if (interPoints.Count > 0)
                                    gaussCode_txt.Text += new string('U', interPoints.Count);
                                List<IntersectionPoint> intersectionPoints = interPoints
                                    .Select(p => new IntersectionPoint(p, CrossingType.Under))
                                    .OrderBy(p => p.Distance(beforeLast.Position))
                                    .ToList();
                                OrderedIntersectionPoints.AddRange(intersectionPoints);

                            }
                            break;
                        }
                    case MouseButtons.Right:
                        {
                            //over cross
                            MovablePoint movPoint = new MovablePoint(new Point(e.X, e.Y));
                            //movPoint.MouseMove += MovPoint_MouseMove;
                            points.Add(movPoint);
                            canvas_pic.Controls.Add(movPoint);
                            

                            if (points.Count >= 2)
                            {
                                MovablePoint last = points.Last();
                                MovablePoint beforeLast = points.Skip(points.Count - 2).First();
                                List<Point> interPoints = GetIntersections(last.Position, beforeLast.Position);
                                if (interPoints.Count > 0)
                                    gaussCode_txt.Text += new string('O', interPoints.Count);
                                List<IntersectionPoint> intersectionPoints = interPoints
                                    .Select(p => new IntersectionPoint(p, CrossingType.Over))
                                    .OrderBy(p => p.Distance(beforeLast.Position))
                                    .ToList();
                                OrderedIntersectionPoints.AddRange(intersectionPoints);

                            }
                            break;
                        }

                }

                if (points.Count >= 2)
                {
                    g.Clear(Color.White);
                    g.DrawLines(new Pen(Color.Blue), points.Select(p => p.Position).ToArray());
                }

                canvas_pic.Image = drawArea;
            }
        }

        private void MovPoint_MouseMove(object sender, MouseEventArgs e)
        {
            if (points.Count >= 2)
            {
                g.Clear(Color.White);
                g.DrawLines(new Pen(Color.Blue), points.Select(p => p.Position).ToArray());
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

        private List<Point> GetIntersections(Point p0, Point p1)
        {
            List<Point> intersections = new List<Point>();
            for (int j = 0; j < points.Count - 1; j++)
            {
                Point p2 = points[j].Position;
                Point p3 = points[j + 1].Position;
                Point? intersect;
                LineIntersect(p0, p1, p2, p3, out intersect);
                if (intersect != null && intersect != p0 && intersect != p1 && intersect != p2 && intersect != p3)
                {
                    intersections.Add(intersect.Value);
                }
            }

            return intersections;
        }

        private List<Point> Intersections()
        {
            List<Point> intersections = new List<Point>();
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
                        intersections.Add(intersect.Value);
                    }
                }
            }
            return intersections;
        }

        private Point[] CreateOpenBezier(Point[] _points)
        {
            Point[] firstC, secondC;
            OpenBezier.GetCurveControlPoints(_points, out firstC, out secondC);
            Point[] final = new Point[_points.Count() + firstC.Count() + secondC.Count()];
            int ind_p = 0, ind_f = 0, ind_s = 0;
            for (int i = 0; i < final.Count(); i++)
            {
                switch (i % 3)
                {
                    case 0:
                        final[i] = _points[ind_p++];
                        break;
                    case 1:
                        final[i] = firstC[ind_f++];
                        break;
                    case 2:
                        final[i] = secondC[ind_s++];
                        break;
                }
            }
            return final;
        }

        private Point[] CreateClosedBezier(Point[] _points)
        {
            Point[] firstC, secondC;
            ClosedBezier.GetCurveControlPoints(_points, out firstC, out secondC);
            Point[] final = new Point[_points.Count() + firstC.Count() + secondC.Count()];
            int ind_p = 0, ind_f = 0, ind_s = 0;
            for (int i = 0; i < final.Count(); i++)
            {
                switch (i % 3)
                {
                    case 0:
                        final[i] = _points[ind_p++];
                        break;
                    case 1:
                        final[i] = firstC[ind_f++];
                        break;
                    case 2:
                        final[i] = secondC[ind_s++];
                        break;
                }
            }
            return final;
        }

        public Knot GetKnot()
        {
            if (Knot == null)
            {
                throw new Exception("Knot has to be closed before getting it.");
            }

            return Knot;
        }

        public void Step()
        {
            throw new NotImplementedException();
        }

        public void Solve()
        {
            throw new NotImplementedException();
        }

        private void CalculateGaussCode()
        {
            gaussCode_txt.Text = "";
            KnotIsClosed = true;

            List<int> gaussCode = new List<int>();

            /* starting from the start A, get all the intersections K in AB
             * order all intersection i in K by distance with A
             * map each i with the corrisponding intersection OrderedIntersectionPoints
             * if the i intersction in under, set over and viceversa
             */
            int crossIndex = 1;
            for (int i = 0; i < points.Count - 1; i++)
            {
                List<Point> pinter = GetIntersections(points[i].Position, points[i + 1].Position);
                List<IntersectionPoint> inters = new List<IntersectionPoint>();
                foreach (Point p in pinter)
                {
                    inters.Add(OrderedIntersectionPoints.Single(k => k.Distance(p) < 5));
                }
                inters = inters.OrderBy(p => p.Distance(points[i].Position)).ToList();
                foreach (IntersectionPoint p in inters)
                {
                    if (p.gaussCross == 0)
                    {
                        p.gaussCross = crossIndex++;
                    }
                    if (!gaussCode.Contains(p.gaussCross) && !gaussCode.Contains(-p.gaussCross))
                    {
                        if (p.CrossingType == CrossingType.Over)
                        {
                            gaussCode.Add(-p.gaussCross);
                        }
                        else
                        {
                            gaussCode.Add(p.gaussCross);
                        } 
                    }
                    else
                    {
                        if (gaussCode.Contains(p.gaussCross) && !gaussCode.Contains(-p.gaussCross))
                        {
                            gaussCode.Add(-p.gaussCross);
                        }
                        else
                        {
                            gaussCode.Add(p.gaussCross);  
                        }
                    }

                }
            }
            foreach (int cross in gaussCode)
            {
                gaussCode_txt.Text += cross + ";";
            }

            Knot = new Knot(gaussCode);
        }

        public void CloseKnot()
        {
            if (Closable)
            {
                points.ForEach(p => canvas_pic.Controls.Remove(p));
                g.Clear(Color.White);
                points.Add(points[0]);
                g.DrawLines(new Pen(Color.Blue), points.Select(p => p.Position).ToArray());

                canvas_pic.Image = drawArea;

                CalculateGaussCode();

            }

        }

        public void Clear()
        {
            points.Clear();
            g.Clear(Color.White);
            canvas_pic.Controls.Clear();
            KnotIsClosed = false;
            canvas_pic.Image = drawArea;
            gaussCode_txt.Text = "";
            OrderedIntersectionPoints.Clear();
        }

        public Image GetImage()
        {
            return canvas_pic.Image;
        }

        private class IntersectionPoint
        {
            public Point Point;
            public CrossingType CrossingType;
            public int gaussCross;

            public IntersectionPoint(Point p, CrossingType cross)
            {
                Point = p;
                CrossingType = cross;
            }

            public double Distance(Point p)
            {
                return Math.Sqrt(Math.Pow((p.X - Point.X), 2) + Math.Pow((p.Y - Point.Y), 2));
            }

        }

    }
}
