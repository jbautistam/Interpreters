using System;

namespace Compilers.Testing.Models
{
	/// <summary>
	///		Mensaje de salida del script en ejecución
	/// </summary>
	public class ResultMessageModel : BaseResultModel
	{
		public ResultMessageModel(string message)
		{
			Message = message;
		}

		/// <summary>
		///		Mensaje de salida
		/// </summary>
		public string Message { get; }
	}
}
