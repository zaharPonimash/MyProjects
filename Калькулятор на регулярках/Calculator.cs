/*
 * Создано в SharpDevelop.
 * Пользователь: admin
 * Дата: 03.01.2019
 * Время: 20:39
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Regul
{
	/// <summary>
	/// Калькулятор
	/// </summary>
	public class Calculator
	{
		NumberFormatInfo provider; 
		#region Паттерны
		const string patternMult = @"[\-\d\,]+\*[\-\d\,]+";
		const string patternPow = @"[\-\d\,]+\^[\-\d\,]+";
		const string patternDiv = @"\/[\-\d\,]+";
		const string patternDif = @"\-[\d\,]+";
		const string patternSumm = @"[\-\d\,]+\+[\-\d\,]+";
		const string patternParentheses = @"\([\w\,\-\^\*\+\/]+\)";
		
		// Функции
		const string patternAbs = @"abs[\-\d\,]+";
		const string patternSin = @"sin[\-\d\,]+";
		#endregion
		
		/// <summary>
		/// Калькулятор
		/// </summary>
		public Calculator()
		{
			provider = new NumberFormatInfo();
			provider.NumberDecimalSeparator = ",";
			provider.NumberGroupSeparator = ".";
			provider.NumberGroupSizes = new int[] { 3 };
		}
		
		
		/// <summary>
		/// Вычисление строки
		/// </summary>
		/// <param name="strExpression">Выражение</param>
		/// <returns>Ответ</returns>
		public double Calculate(string strExpression)
		{
				string str = strExpression.Replace(".",",").Replace(" ","");
				while(Regex.IsMatch(str, patternParentheses)) 
				{
					str = Regex.Replace (str, patternParentheses, CalcSimpleExpr);
					str = Functions(str);
				}
				str = CalcSimpleExpr(str);
				return Convert.ToDouble(str, provider);
		}
		
		#region Вычисления
		string CalcSimpleExpr(string str)
		{
				while(Regex.IsMatch(str, patternPow)) str = Regex.Replace (str, patternPow, Pow);
				// Div2Mult
				str = Regex.Replace( str, patternDiv, Div2M);
				//Mult		
				while(Regex.IsMatch(str, patternMult)) str = Regex.Replace (str, patternMult, Multi);
				// Dif2Sum
				str = Regex.Replace(str, patternDif, match => ("+"+match.Value));
				// Sum
				while(Regex.IsMatch(str, patternSumm)) str = Regex.Replace (str, patternSumm, Summ);
								
				str = str.Replace("+-","-");
			
				return str;
			
		}
		string CalcSimpleExpr(Match match)
		{
			string str = match.Value;
			char[] ch = new char[str.Length-2];
			str.CopyTo(1, ch, 0, str.Length-2);
			
			return CalcSimpleExpr(new string(ch));
		}
		string Functions(string str)
		{
			str = Regex.Replace (str, patternAbs, Abs);
			str = Regex.Replace (str,  patternSin, Sin);
			return str;
		}
		#endregion
		
		#region Вспом. методы
		string Multi(Match match)
		{
			string str = match.Value;
			string[] strs = str.Split('*');
			
			double a = double.Parse(strs[0], provider);
			double b = double.Parse(strs[1], provider);
			
			return ""+a*b;
		}
		string Pow(Match match)
		{
			string str = match.Value;
			string[] strs = str.Split('^');
			
			double a = double.Parse(strs[0], provider);
			double b = double.Parse(strs[1], provider);
			
			return ""+Math.Pow(a,b);
		}
		string Summ(Match match)
		{
			string str = match.Value;
			string[] strs = str.Split('+');
			
			double a = double.Parse(strs[0], provider);
			double b = double.Parse(strs[1], provider);
			
			return ""+(a+b);
		}	
		string Div2M(Match match)
		{
			string str = match.Value;
			string[] strs = str.Split('/');
			
			double a = double.Parse(strs[1], provider);
			
			return "*"+1.0/a;
		}
		#endregion
		
		#region Функции
		string Abs(Match match)
		{
			string str = match.Value;
			char[] ch = new char[str.Length-3];
			str.CopyTo(3, ch, 0, ch.Length);
			str = new string(ch);
			return Math.Abs(double.Parse(str, provider))+"";
		}
		string Sin(Match match)
		{
			string str = match.Value;
			char[] ch = new char[str.Length-3];
			str.CopyTo(3, ch, 0, ch.Length);
			str = new string(ch);
			return Math.Sin(double.Parse(str, provider))+"";
		}
		#endregion
	}
}
