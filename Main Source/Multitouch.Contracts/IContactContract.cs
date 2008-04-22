using System;
using System.AddIn.Contract;
using PipelineHints;

namespace Multitouch.Contracts.Contracts
{
	public interface IContactContract : IContract
	{
		[Comment(
		"/// <summary>"+
		"/// Contact Id."+
		"/// </summary>"
		)]
		int Id { get; }
		[Comment(
		"/// <summary>"+
		"/// X coordinate of contact center."+
		"/// </summary>"
		)]
		double X { get; }
		[Comment(
		"/// <summary>"+
		"/// Y coordinate of contact center."+
		"/// </summary>"
		)]
		double Y { get; }
		[Comment(
		"/// <summary>"+
		"/// Width of contact."+
		"/// </summary>"
		)]
		double Width { get; }
		[Comment(
		"/// <summary>"+
		"/// Height of contact."+
		"/// </summary>"
		)]
		double Height { get; }
		[Comment(
		"/// <summary>"+
		"/// State of contact. See <see cref=\"ContactState\"/>"+
		"/// </summary>"
		)]
		ContactState State { get; }
	}
}