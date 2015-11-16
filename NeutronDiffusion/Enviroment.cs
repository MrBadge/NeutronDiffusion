using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeutronDiffusion
{
	class Enviroment
	{
		public double SigmaS { get; }
		public double SigmaA { get; }
		public double CosFi { get; }
		public int NeutronNums { get; set; }

		private List<Neutron> neutrons = new List<Neutron>();

		public Enviroment(double SigmaS, double SigmaA, double CosFi)
		{
			this.SigmaS = SigmaS;
			this.SigmaA = SigmaA;
			this.CosFi = CosFi;
		}

		public static void Main()
		{
			Enviroment env = new Enviroment(2, 2, 0.1);
			env.NeutronNums = 2;
			env.StartSimulation();
		}

		public void StartSimulation()
		{
			for (int i = 0; i < NeutronNums; i++)
				neutrons.Add(new Neutron(new CustomPoint3D(), SigmaA, SigmaS, CosFi));
			neutrons.ForEach(neutron => neutron.Move());
			Console.WriteLine("HERE");
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
