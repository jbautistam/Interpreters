using System;

namespace Bau.Libraries.LibInterpreter.Lexer.Parser
{
	/// <summary>
	///		Clase para obtener los caracteres de una línea
	/// </summary>
	internal class StringCharSeparator
	{
		internal StringCharSeparator(string source)
		{
			Source = source;
			IndexActualChar = 0;
			Row = 1;
			Column = 1;
		}

		/// <summary>
		///		Obtiene el carácter anterior
		/// </summary>
		internal string GetPreviousChar()
		{
			if (IndexActualChar == 0 || IsEof)
				return string.Empty;
			else
				return Source[IndexActualChar - 1].ToString();
		}

		/// <summary>
		///		Obtiene el siguiente carácter
		/// </summary>
		internal char GetNextChar()
		{
			if (IsEof)
				return ' ';
			else
			{
				char actual = Source[IndexActualChar++];

					// Cuenta una columna
					Column++;
					// Cuenta el salto de línea y los tabuladores
					if (actual == '\r' || actual == '\n')
					{
						if (actual == '\n' && actual != '\r')
							Row++;
						Column = 1;
					}
					// Devuelve el carácter
					return actual;
			}
		}

		/// <summary>
		///		Obtiene una serie de caracteres
		/// </summary>
		internal string GetChars(int count)
		{
			string output = "";

				// Recoge los caracteres
				for (int index = 0; index < count; index++)
					if (!IsEof)
						output += GetNextChar();
				// Devuelve la cadena
				return output;
		}

		/// <summary>
		///		Obtiene los caracteres hasta encontrarse un salto de línea
		/// </summary>
		internal string GetCharsToEndLine()
		{
			string output = "";

				// Recorre la cadena hasta encontrarse un salto de línea
				if (!IsEof)
				{
					string actual = LookAtChar();

						while (!IsEof && actual != "\r" && actual != "\n")
						{
							output += GetNextChar();
							actual = LookAtChar();
						}
				}
				// Devuelve la cadena
				return output;
		}

		/// <summary>
		///		Obtiene los caracteres hasta el primer espacio
		/// </summary>
		internal string GetCharsToSpace()
		{
			string output = "";

				// Obtiene los caracteres
				while (!IsEof && !IsSpace)
					output += GetNextChar();
				// Devuelve la cadena de salida
				return output;
		}

		/// <summary>
		///		Salta los espacios
		/// </summary>
		internal void SkipSpaces()
		{
			while (!IsEof && IsSpace)
				GetNextChar();
		}

		/// <summary>
		///		Comprueba si un carácter es un espacio
		/// </summary>
		internal bool CheckIsSpace(string character)
		{
			return character == " " || character == "\t" || character == "\r" || character == "\n";
		}

		/// <summary>
		///		Obtiene un carácter
		/// </summary>
		internal string LookAtChar(int chars = 1)
		{
			string output = "";

				// Recoge los caracteres
				for (int index = 0; index < chars; index++)
					if (!string.IsNullOrWhiteSpace(Source) && IndexActualChar + index < Source.Length)
						output += Source[IndexActualChar + index];
				// Devuelve la cadena
				return output;
		}

		/// <summary>
		///		Obtiene una serie de caracteres
		/// </summary>
		internal string LookAtChars(int from, int length)
		{
			return Mid(Source, IndexActualChar + from, length);
		}

		/// <summary>
		///		Obtiene la cadena media
		/// </summary>
		private string Mid(string source, int first, int length)
		{
			return Left(From(source, first), length);
		}

		/// <summary>
		///		Obtiene una cadena a partir de un carácter
		/// </summary>
		private string From(string source, int first)
		{
			if (string.IsNullOrWhiteSpace(source) || first >= source.Length)
				return string.Empty;
			else if (first <= 0)
				return source;
			else
				return source.Substring(first);
		}

		/// <summary>
		///		Obtiene la parte izquierda de una cadena
		/// </summary>
		private string Left(string source, int length)
		{
			if (string.IsNullOrWhiteSpace(source) || length <= 0)
				return string.Empty;
			else if (length > source.Length)
				return source;
			else
				return source.Substring(0, length);
		}

		/// <summary>
		///		Comprueba si los primeros caracteres coinciden con el patrón de inicio
		/// </summary>
		internal bool CheckPatternStart(string patternStart)
		{
			string value = LookAtChar(patternStart.Length);
			bool isPattern = false;

				// Comprueba el patrón
				if (value.Length == patternStart.Length)
				{ 
					// Por ahora coincide el patrón
					isPattern = true;
					// Conprueba si todos los caracteres están en el patrón
					for (int index = 0; index < patternStart.Length; index++)
						if (!CheckIsPattern(value[index], patternStart[index]))
							isPattern = false;
				}
				// Devuelve el valor que indica si coincide el patrón
				return isPattern;
		}

		/// <summary>
		///		Obtiene una cadena que coincide con el patrón de inicio y fin
		/// </summary>
		internal string GetCharsPattern(string patternStart, string patternContent)
		{
			string value = GetChars(patternStart.Length);
			bool end = false;

				// Añade los caracteres del patrón de contenido
				while (!end && !IsEof)
				{
					string charSource = LookAtChar(1);

						if (!string.IsNullOrWhiteSpace(charSource) && CheckIsAtPattern(charSource[0], patternContent))
							value += GetChars(1);
						else
							end = true;
				}
				// Devuelve los caracteres
				return value;
		}

		/// <summary>
		///		Comprueba si un carácter está en el patrón
		/// </summary>
		private bool CheckIsAtPattern(char source, string pattern)
		{ 
			// Comprueba si un carácter pertenece al patrón
			foreach (char chrPattern in pattern)
				if (CheckIsPattern(source, chrPattern))
					return true;
			// Devuelve el valor que indica si el carácter está en el patrón
			return false;
		}

		/// <summary>
		///		Comprueba si un carácter coincide con el patrón
		/// </summary>
		private bool CheckIsPattern(char source, char pattern)
		{
			return ((pattern == 'a' || pattern == 'A') && char.IsLetter(source)) ||
					(pattern == '9' && char.IsNumber(source)) ||
					(pattern == source);
		}

		/// <summary>
		///		Obtiene la indentación de un carácter
		/// </summary>
		internal int GetIndentFrom(int start)
		{
			bool end = false;
			int indent = 0;

				// Cuenta el número de tabuladores
				while (!end && --start >= 0)
					if (Source[start] == '\t')
						indent++;
					else
						end = true;
				// Se salta los tabuladores que no comienzan en un salto de línea
				if (start > 0 && Source[start] != '\r' && Source[start] != '\n')
					indent = 0;
				// Devuelve el número de tabuladores
				return indent;
		}

		/// <summary>
		///		Indica si es el final del texto
		/// </summary>
		internal bool IsEof
		{
			get { return string.IsNullOrWhiteSpace(Source) || IndexActualChar >= Source.Length; }
		}

		/// <summary>
		///		Indica si el siguiente carácter es un espacio
		/// </summary>
		public bool IsSpace
		{
			get { return CheckIsSpace(LookAtChar()); }
		}

		/// <summary>
		///		Indice del carácter actual
		/// </summary>
		internal int IndexActualChar { get; private set; }

		/// <summary>
		///		Fila
		/// </summary>
		internal int Row { get; private set; }

		/// <summary>
		///		Columna
		/// </summary>
		internal int Column { get; private set; }

		/// <summary>
		///		Línea original
		/// </summary>
		internal string Source { get; }
	}
}
