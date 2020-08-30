using System;
using System.Collections.Generic;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context.Variables
{
	/// <summary>
	///		Tabla de variables
	/// </summary>
	public class TableVariableModel
	{
		public TableVariableModel(ContextModel context)
		{
			Context = context;
		}

		/// <summary>
		///		Añade una colección de variables
		/// </summary>
		public void AddRange(List<VariableModel> variables)
		{
			if (variables != null)
				foreach (VariableModel variable in variables)
					Add(variable);
		}

		/// <summary>
		///		Añade una variable
		/// </summary>
		public void Add(VariableModel variable)
		{
			Add(variable.Name, variable.Type, variable.Value);
		}

		/// <summary>
		///		Añade una variable de un tipo con el valor por defecto
		/// </summary>
		public void Add(string name, VariableModel.VariableType type)
		{
			switch (type)
			{
				case VariableModel.VariableType.Boolean:
						Add(name, type, false);
					break;
				case VariableModel.VariableType.Date:
						Add(name, type, null);
					break;
				case VariableModel.VariableType.Numeric:
						Add(name, type, 0);
					break;
				case VariableModel.VariableType.String:
						Add(name, type, string.Empty);
					break;
				default:
					throw new ArgumentException("Type unknown");
			}
		}

		/// <summary>
		///		Añade una variable
		/// </summary>
		public void Add(string name, VariableModel.VariableType type, object value)
		{
			// Normaliza el nombre
			name = Normalize(name);
			// Añade / modifica el valor
			if (Variables.ContainsKey(name))
				Variables[name].Value = value;
			else
			{
				VariableModel variable = new VariableModel(name, type);

					// Asigna el valor
					variable.Value = value;
					// Añade la variable a la tabla
					Variables.Add(name, variable);
			}
		}

		/// <summary>
		///		Clona todas las variables de este contexto
		/// </summary>
		public Dictionary<string, VariableModel> GetAll()
		{
			Dictionary<string, VariableModel> variables = new Dictionary<string, VariableModel>();

				// Convierte las variables
				foreach (KeyValuePair<string, VariableModel> item in Variables)
					variables.Add(item.Key, item.Value);
				// Devuelve la colección de variables
				return variables;
		}

		/// <summary>
		///		Obtiene una variable (si existe, no la crea si no existe en la tabla de variables)
		/// </summary>
		//TODO --> FALTA buscar por el índice
		public VariableModel Get(string name, int? index = null)
		{
			// Normaliza el nombre
			name = Normalize(name);
			// Obtiene el valor
			if (Variables.ContainsKey(name))
				return Variables[name];
			else if (Context.Parent != null)
				return Context.Parent.VariablesTable.Get(name);
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
		public VariableModel this[string name]
		{
			get { return Get(name); }
			set { Add(value); }
		}

		/// <summary>
		///		Contexto
		/// </summary>
		private ContextModel Context { get; }

		/// <summary>
		///		Diccionario de variables
		/// </summary>
		private Dictionary<string, VariableModel> Variables { get; } = new Dictionary<string, VariableModel>();
	}
}