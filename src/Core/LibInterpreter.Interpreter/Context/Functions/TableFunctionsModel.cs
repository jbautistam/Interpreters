using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Functions
{
	/// <summary>
	///		Tabla de funciones
	/// </summary>
	public class TableFunctionsModel
	{
		public TableFunctionsModel(ContextModel context)
		{
			Context = context;
		}

		/// <summary>
		///		Añade una función definida por el usuario
		/// </summary>
		public void Add(Models.Sentences.SentenceFunction function)
		{
			UserDefinedFunctionModel udf = new UserDefinedFunctionModel(function.Definition, function.Arguments);

				// Asigna los datos
				udf.Sentences.AddRange(function.Sentences);
				// Añade la función a la tabla
				Add(udf);
		}

		/// <summary>
		///		Añade una función
		/// </summary>
		public void Add(BaseFunctionModel function)
		{
			string name = Normalize(function.Definition.Name);

				// Añade / modifica el valor
				if (Functions.ContainsKey(name))
					Functions[name] = function;
				else
					Functions.Add(name, function);
		}

		/// <summary>
		///		Añade una lista de funciones
		/// </summary>
		public void AddRange<TypeData>(List<TypeData> functions) where TypeData : BaseFunctionModel
		{
			foreach (TypeData function in functions)
				Add(function);
		}

		/// <summary>
		///		Obtiene una función
		/// </summary>
		public BaseFunctionModel GetIfExists(string name)
		{
			// Normaliza el nombre
			name = Normalize(name);
			// Obtiene el valor
			if (Functions.ContainsKey(name))
				return Functions[name];
			else if (Context.Parent != null)
				return Context.Parent.FunctionsTable.GetIfExists(name);
			else
				return null;
		}

		/// <summary>
		///		Normaliza el nombre de la variable
		/// </summary>
		private string Normalize(string name)
		{
			return name.ToUpper();
		}

		/// <summary>
		///		Indizador
		/// </summary>
		public BaseFunctionModel this[string name]
		{
			get { return GetIfExists(name); }
			set { Add(value); }
		}

		/// <summary>
		///		Contexto
		/// </summary>
		private ContextModel Context { get; }

		/// <summary>
		///		Diccionario de funciones
		/// </summary>
		private Dictionary<string, BaseFunctionModel> Functions { get; } = new Dictionary<string, BaseFunctionModel>();
	}
}
