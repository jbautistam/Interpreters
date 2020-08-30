using System;

namespace Bau.Libraries.LibInterpreter.Interpreter.Context
{
	/// <summary>
	///		Clase con los datos de contexto
	/// </summary>
	public class ContextModel
	{
		public ContextModel(ContextModel parent)
		{
			// Guarda el contexto padre y el índice
			Parent = parent;
			if (Parent == null)
				ScopeIndex = 0;
			else
				ScopeIndex = Parent.ScopeIndex + 1;
			// Crea las tablas de variables y funciones
			VariablesTable = new Variables.TableVariableModel(this);
			FunctionsTable = new Functions.TableFunctionsModel(this);
		}

		/// <summary>
		///		Obtiene recursivamente la tabla de variables del contexto
		/// </summary>
		public Variables.TableVariableModel GetVariablesRecursive()
		{
			Variables.TableVariableModel table = new Variables.TableVariableModel(this);

				// Añade las variables del padre
				if (Parent != null)
					table = Parent.GetVariablesRecursive();
				// Añade / sustituye las variables propias
				foreach (System.Collections.Generic.KeyValuePair<string, Variables.VariableModel> item in VariablesTable.GetAll())
					table.Add(item.Value);
				// Devuelve la colección de tablas
				return table;
		}

		/// <summary>
		///		Contexto padre
		/// </summary>
		public ContextModel Parent { get; }

		/// <summary>
		///		Tabla de variables
		/// </summary>
		public Variables.TableVariableModel VariablesTable { get; }

		/// <summary>
		///		Tabla de funciones
		/// </summary>
		public Functions.TableFunctionsModel FunctionsTable { get; }

		/// <summary>
		///		Indice del ámbito
		/// </summary>
		public int ScopeIndex { get; }

		/// <summary>
		///		Nombre de la variable que debe devolver el resultado de la función que está activa
		/// </summary>
		public string ScopeFuntionResultVariable { get; set; }
	}
}
