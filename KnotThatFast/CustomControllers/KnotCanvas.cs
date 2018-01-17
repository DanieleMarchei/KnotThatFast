﻿using System;
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
        //private List<IntersectionPoint> OrderedIntersectionPoints = new List<IntersectionPoint>();
        private List<Line> lines = new List<Line>();
        int indexCross = 1;

        public KnotCanvas()
        {
            InitializeComponent();
        }

        private void KnotCanvas_Load(object sender, EventArgs e)
        {
            drawArea = new Bitmap(canvas_pic.Size.Width, canvas_pic.Size.Height);
            g = Graphics.FromImage(drawArea);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //TEST
            //Knot knot = new Knot(new List<int>() { -1, 2, -3, 4, -4, 5, -2, 1, -5, 3 });
            //Knot.Solve(knot);
        }

        private void canvas_pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (!KnotIsClosed)
            {
                Point newP = new Point(e.X, e.Y);
                newP = new Point(e.X, e.Y);
                MovablePoint movPoint = new MovablePoint(newP);
                //movPoint.MouseMove += MovPoint_MouseMove;

                if(e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    CrossingType crossType = e.Button == MouseButtons.Left ? CrossingType.Under : CrossingType.Over;
                    char charType = e.Button == MouseButtons.Left ? 'U' : 'O';
                    points.Add(movPoint);
                    canvas_pic.Controls.Add(movPoint);

                    if (points.Count >= 2)
                    {
                        MovablePoint beforeLast = points.Skip(points.Count - 2).First();
                        Line line = new Line(beforeLast, movPoint);
                        MovablePoint last = points.Last();
                        List<Point> interPoints = new List<Point>();
                        for (int i = 0; i < lines.Count; i++)
                        {
                            Point? p = line.Intersect(lines[i]);
                            if (p.HasValue)
                            {
                                if(!lines[i].Intersections.Any(ip => ip.Distance(p.Value) <= 5))
                                {
                                    interPoints.Add(p.Value);

                                    IntersectionPoint p1 = new IntersectionPoint(p.Value, crossType);
                                    IntersectionPoint p2 = new IntersectionPoint(p.Value, crossType == CrossingType.Over ? CrossingType.Under : CrossingType.Over);
                                    p1.gaussCross = crossType == CrossingType.Over ? indexCross : -indexCross;
                                    p2.gaussCross = -p1.gaussCross;
                                    indexCross++;
                                    line.Intersections.Add(p1);
                                    lines[i].Intersections.Add(p2);
                                    lines[i].OrderPoints();
                                }
                            }
                        }

                        line.OrderPoints();
                        lines.Add(line);

                        if (interPoints.Count > 0)
                            gaussCode_txt.Text += new string(charType, interPoints.Count);
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
            if (Knot != null)
                Knot = Knot.Step(Knot);
        }

        public void Solve()
        {
            if (Knot != null)
                Knot = Knot.Solve(Knot);
        }

        private void CalculateGaussCode()
        {
            gaussCode_txt.Text = "";
            KnotIsClosed = true;

            List<int> gaussCode = new List<int>();

            foreach (Line l in lines)
            {
                foreach(IntersectionPoint p in l.Intersections)
                {
                    gaussCode.Add(p.gaussCross);
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
                Line line = new Line(points.Last(), points.First());
                lines.Add(line);

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
            lines.Clear();
            indexCross = 1;
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

        private class Line
        {
            public MovablePoint Start, Finish;
            public List<IntersectionPoint> Intersections;

            public Line(MovablePoint start, MovablePoint finish)
            {
                Start = start;
                Finish = finish;
                Intersections = new List<IntersectionPoint>();
            }

            public Point? Intersect(Line l)
            {
                if (this == l)
                    return null;

                if (this.Start == l.Start || this.Start == l.Finish)
                    return null;

                Point p0 = this.Start.Position;
                Point p1 = this.Finish.Position;
                Point p2 = l.Start.Position;
                Point p3 = l.Finish.Position;

                Point s1 = new Point(p1.X - p0.X, p1.Y - p0.Y);
                Point s2 = new Point(p3.X - p2.X, p3.Y - p2.Y);

                float denom = -s2.X * s1.Y + s1.X * s2.Y;
                float s = (-s1.Y * (p0.X - p2.X) + s1.X * (p0.Y - p2.Y)) / denom;
                float t = (s2.X * (p0.Y - p2.Y) - s2.Y * (p0.X - p2.X)) / denom;

                if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
                {
                    return new Point(p0.X + (int)(t * s1.X), p0.Y + (int)(t * s1.Y));
                }

                return null;
            }

            public void OrderPoints()
            {
                Intersections = Intersections.OrderBy(p => p.Distance(Start.Position)).ToList();
            }

            public override bool Equals(object obj)
            {
                if(obj is Line)
                {
                    Line other = (Line)obj;

                    bool same = this.Start == other.Start && this.Finish == other.Finish;
                    bool inverse = this.Start == other.Finish && this.Finish == other.Start;
                    return same || inverse;
                }
                return false;
            }

            public override int GetHashCode()
            {
                var hashCode = -71426702;
                hashCode = hashCode * -1521134295 + EqualityComparer<MovablePoint>.Default.GetHashCode(Start);
                hashCode = hashCode * -1521134295 + EqualityComparer<MovablePoint>.Default.GetHashCode(Finish);
                hashCode = hashCode * -1521134295 + EqualityComparer<List<IntersectionPoint>>.Default.GetHashCode(Intersections);
                return hashCode;
            }

            public static bool operator ==(Line line1, Line line2)
            {
                return EqualityComparer<Line>.Default.Equals(line1, line2);
            }

            public static bool operator !=(Line line1, Line line2)
            {
                return !(line1 == line2);
            }
        }

    }
}
