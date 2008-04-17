using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Physics2DDotNet;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Shapes;

namespace Danilins.Multitouch.PhysicsLibrary
{
	public class CustomBody : Body
	{
		internal List<Joint> joints;

		public CustomBody(PhysicsState state, Shape shape, double mass, Coefficients coefficients, Lifespan lifetime)
			: base(state, shape, mass, coefficients, lifetime)
		{
			joints = new List<Joint>();
		}

		public CustomBody(PhysicsState state, Shape shape, MassInfo massInfo, Coefficients coefficients, Lifespan lifetime)
			: base(state, shape, massInfo, coefficients, lifetime)
		{
			joints = new List<Joint>();
		}

		public ReadOnlyCollection<Joint> Joints
		{
			get { return new ReadOnlyCollection<Joint>(joints); }
		}
	}
}