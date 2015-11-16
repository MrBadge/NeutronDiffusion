using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Media3D;

namespace NeutronDiffusion
{
    class Neutron
    {
        readonly Random _r = new Random();
        public List<double> FreePathLength { get; set; }
        private List<Vector3D> GuidedCos { get; set; }
        public List<CustomPoint3D> CollisionPoint { get; set; }
        public double AverageFreePathLength { get; set; }
        private bool isAbsorbed { get; set; }

        private const double TWO_PI = Math.PI*2;
        private double _sigmaA, _sigmaTr;        

        public Neutron(CustomPoint3D startPoint, double sigmaA, double sigmaTr)
        {
            this._sigmaA = sigmaA;
            this._sigmaTr = sigmaTr;
            this.AverageFreePathLength = 0;

            this.isAbsorbed = false;
            this.FreePathLength = new List<double>() { 0 };
            this.GuidedCos = new List<Vector3D>() { new Vector3D() };
            this.CollisionPoint = new List<CustomPoint3D>() { startPoint };
        }

        public void Move()
        {
            if (this.isAbsorbed)
                return;
            var step = CollisionPoint.Count;
            while (!isAbsorbed)
            {
                SubStep(step, Rnd(0, 1), CollisionPoint[step - 1]);
                step += 1;
            }
            AverageFreePathLength = AverageFreePathLength/(FreePathLength.Count - 1);
        }

        private void SubStep(int step, double gamma, CustomPoint3D startPoint)
        {
            FreePathLength.Add(-Math.Log(gamma) / _sigmaTr);
            var cosZ = 1 - 2 * gamma;
            var tmp2 = Math.Sqrt(1 - Math.Pow(cosZ, 2));
            GuidedCos.Add(new Vector3D(tmp2 * Math.Cos(TWO_PI * gamma), tmp2 * Math.Sin(TWO_PI * gamma), cosZ));
            CollisionPoint.Add(new CustomPoint3D(
                startPoint.X + GuidedCos[step].X * FreePathLength[step],
                startPoint.Y + GuidedCos[step].Y * FreePathLength[step],
                startPoint.Z + GuidedCos[step].Z * FreePathLength[step]));
            AverageFreePathLength = CustomPoint3D.DistanceBetween(CollisionPoint[step], CollisionPoint[step - 1]);
            if (Rnd(0, 1) <= _sigmaA/_sigmaTr)
                this.isAbsorbed = true;
        }



        private double Rnd(double a, double b)
        {
            return a + _r.NextDouble() * (b - a);
        }


    }
}
