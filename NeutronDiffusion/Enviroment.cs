using System;
using System.Collections.Generic;
using System.Linq;

namespace NeutronDiffusion
{
	class Enviroment
	{
		public double SigmaS { get; set; }
		public double SigmaA { get; set; }
		public double CosFi { get; set; }
        public double SigmaTr { get; set; }
		public int NeutronNums { get; set; }

		private List<Neutron> neutrons = new List<Neutron>();

		public Enviroment(double SigmaS, double SigmaA, double CosFi)
		{
			this.SigmaS = SigmaS;
			this.SigmaA = SigmaA;
			this.CosFi = CosFi;
		    this.SigmaTr = SigmaA + SigmaS*(1 - CosFi);
		}

		public static void Main2()
		{
			Enviroment env = new Enviroment(2, 2, 0.1);
			env.NeutronNums = 20000;
            Console.WriteLine("Launching {0} neutrons...", env.NeutronNums);
			env.StartSimulation();
		}

		public void StartSimulation()
		{
			for (int i = 0; i < NeutronNums; i++)
				neutrons.Add(new Neutron(new CustomPoint3D(), SigmaA, SigmaTr));
            var threads = new NeutronThreadsWrapper(neutrons);
            threads.LaunchCalculations();
            Console.WriteLine("MeanFreePathBeforeAbsorption: {0}", MeanFreePathBeforeAbsorption());
        }

        public List<CustomPoint3D> SimulateOneNeutron()
        {
            Neutron neutron = new Neutron(new CustomPoint3D(), SigmaA, SigmaTr);
            neutron.Move();
            return neutron.CollisionPoint;
        }

        private double MeanFreePathBeforeAbsorption()
		{
			return neutrons.Aggregate(
				0.0,
				(res, neutron) => res + neutron.AverageFreePathLength * neutron.FreePathLength.Count
			) / NeutronNums;
		}

		private double MeanSquareOffsetBeforeAbsorption()
		{
			return 0;
		}
    }
}
