using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ILNumerics;
using ILNumerics.Drawing;
using ILNumerics.Drawing.Plotting;

namespace NeutronDiffusion
{
    public partial class Form1 : Form
    {
        private double SigmaA { get; set; } = 22030;
        private double SigmaS { get; set; } = 2203;
        private double CosFi { get; set; } = 0.4;
        private readonly Enviroment _enviroment;

        private ILPanel _panel;
        private ILScene _scene;
        private ILPlotCube _plotCube;

        public Form1()
        {
            _enviroment = new Enviroment(SigmaA, SigmaS, CosFi);

            KeyPreview = true;
            KeyPress += Form1_KeyPress;
            InitGrapher();
            InitializeComponent();
        }

        private void InitGrapher()
        {
            _panel = new ILPanel {Dock = DockStyle.Fill};
            _scene = _panel.Scene;
            _plotCube = new ILPlotCube(twoDMode: false)
            {
                Rotation = Matrix4.Rotation(new Vector3(1, 0, 0), Math.PI/2.5)
                           *Matrix4.Rotation(new Vector3(0, 0, 1), -3*Math.PI/3.5),
                AllowRotation = true,
                AllowPan = true
            };
            _scene.Add(_plotCube);
            _scene.Configure();
            Controls.Add(_panel);
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 32)
            {
                CalculateAndGraphNextNeutron();
            }
        }

        private void CalculateAndGraphNextNeutron()
        {
            var points = _enviroment.SimulateOneNeutron();
            ILArray<float> xyz = ILMath.zeros<float>(3, points.Count);
            var x = new List<float>(points.Select(e => (float)e.X));
            var y = new List<float>(points.Select(e => (float)e.Y));
            var z = new List<float>(points.Select(e => (float)e.Z));
            xyz["0;:"] = x.ToArray();
            xyz["1;:"] = y.ToArray();
            xyz["2;:"] = z.ToArray();
            _plotCube.Add(new ILLinePlot(xyz, lineWidth: 2));
            _panel.Refresh();
        }
    }
}
