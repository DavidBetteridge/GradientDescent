using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GradientDescent
{
    public partial class Form1 : Form
    {
        private readonly Panel _panel;
        private const int border = 50;
        private readonly List<PointF> _points = new List<PointF>();
        public Form1()
        {
            InitializeComponent();

            _panel = new Panel
            {
                BackColor = Color.DarkGray,
                Size = new Size(500, 500),
                Location = new Point(100, 100)
            };

            Controls.Add(_panel);

            this.WindowState = FormWindowState.Maximized;
            this.Paint += Form1_Paint;

            this._panel.MouseClick += _panel_MouseClick;
        }

        private void CalculateBestFit()
        {
            var t0 = 0.0;
            var t1 = 0.0;
            var alpha = .05;
            double m = _points.Count;

            var temp_t0 = t0;
            var temp_t1 = t1;

            var i = 0;

            do
            {
                temp_t0 = t0;
                temp_t1 = t1;

                var sum0 = _points.Sum(pt =>
                {
                    var h = temp_t0 + (temp_t1 * pt.X);
                    var diff = h - pt.Y;
                    return diff;
                });

                t0 = temp_t0 - (alpha * (1 / m) * sum0);

                var sum1 = _points.Sum(pt =>
                {
                    var h = temp_t0 + (temp_t1 * pt.X);
                    var diff = h - pt.Y;
                    return diff * pt.X;
                });

                t1 = temp_t1 - (alpha * (1 / m) * sum1);

                i++;
            } while (i < 100 && (t0 != temp_t0 || t1 != temp_t1));


            DrawLine(t0, t1);
        }

        private void DrawLine(double t0, double t1)
        {
            using var g = _panel.CreateGraphics();

            var start = new Point(0, (int)t0);
            var end = new Point(10, (int)(t0 + (t1 * 10)));

            g.DrawLine(Pens.Green,
                       new Point(XtoCoord(start.X), YtoCoord(start.Y)),
                       new Point(XtoCoord(end.X), YtoCoord(end.Y)));
        }

        private void _panel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X < border || e.X > _panel.Width - border) return;
            if (e.Y < border || e.Y > _panel.Height - border) return;

            var x = (e.X - border) / ((_panel.Width - border * 2) / 10.0);
            var y = 10 - ((e.Y - border) / ((_panel.Height - border * 2) / 10.0));

            _points.Add(new PointF((float)x, (float)y));

            DrawAll();
        }

        private void DrawAll()
        {
            _panel.Refresh();

            using var g = _panel.CreateGraphics();

            g.DrawLine(Pens.Black, new Point(border, border), new Point(border, _panel.Height - border));
            g.DrawLine(Pens.Black, new Point(border, _panel.Height - border), new Point(_panel.Width - border, _panel.Height - border));

            g.DrawString("x", this.Font, Brushes.Black, new PointF(_panel.Width - border, _panel.Height - border));
            g.DrawString("y", this.Font, Brushes.Black, new PointF(border / 2, border));

            foreach (var point in _points)
            {
                var x = XtoCoord(point.X);
                var y = YtoCoord(point.Y);

                g.DrawLine(Pens.Red, new Point(x - 5, y - 5), new Point(x + 5, y + 5));
                g.DrawLine(Pens.Red, new Point(x + 5, y - 5), new Point(x - 5, y + 5));
            }

            if (_points.Count > 5)
            {
                CalculateBestFit();
            }
        }

        private int XtoCoord(float x) => (int)(border + (x * ((_panel.Width - border * 2) / 10.0)));
        private int YtoCoord(float y) => (int)(border + ((10 - y) * ((_panel.Height - border * 2) / 10.0)));

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DrawAll();
        }
    }
}
