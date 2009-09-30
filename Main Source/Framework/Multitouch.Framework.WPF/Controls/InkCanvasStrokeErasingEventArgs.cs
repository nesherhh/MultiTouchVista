using System;
using System.ComponentModel;
using System.Windows.Ink;

namespace Multitouch.Framework.WPF.Controls
{
	/// <summary>
	/// 
	/// </summary>
	public class InkCanvasStrokeErasingEventArgs : CancelEventArgs
	{
		private readonly Stroke stroke;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="stroke"></param>
		public InkCanvasStrokeErasingEventArgs(Stroke stroke)
		{
			this.stroke = stroke;
		}

		/// <summary>
		/// 
		/// </summary>
		public Stroke Stroke
		{
			get { return stroke; }
		}
	}
}
