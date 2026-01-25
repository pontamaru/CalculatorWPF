using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalculatorWpf.Models
{
	public class CalculatorModel
	{
		// 最終結果
		public string Result { set; get; } = string.Empty;

		// 計算式
		public string Formula { set; get; } = string.Empty;

		public enum Operator
		{
			Addition,
			Subtraction,
			Multiplication,
			Division
		}

		private string? _valueFirst = null;
		private string? _valueSecond = null;
		private Operator? _operator = null;

		public bool CanCulculate =>
			!string.IsNullOrEmpty(_valueFirst) && !string.IsNullOrEmpty(_valueSecond) && _operator.HasValue;

		private bool isCulculated = false;

		public CalculatorModel()
		{
		}

		/// <summary>
		/// 計算結果を算出してMainViewにセットする
		/// </summary>
		public void Culculate()
		{
			if (!(CanCulculate))
				return;

			decimal value1 = decimal.Parse(_valueFirst);
			decimal value2 = decimal.Parse(_valueSecond);


			decimal result = 0;
			bool isFailure = false;
			string errorMessage = string.Empty;

			try
			{
				// オーバーフロー検出付きで計算
				checked
				{
					switch (_operator)
					{
						case Operator.Addition:
							result = value1 + value2;
							break;
						case Operator.Subtraction:
							result = value1 - value2;
							break;
						case Operator.Multiplication:
							result = value1 * value2;
							break;
						case Operator.Division:
							if (value2 == 0)
							{
								isFailure = true;
								errorMessage = "0で割ることはできません";
							}
							else
							{
								result = value1 / value2;
							}
							break;
					}
				}
			}
			catch (OverflowException ex)
			{
				Console.Write(ex.Message);
				errorMessage = "計算結果が大きすぎます";
				isFailure = true;
			}

			Formula += " " + _valueSecond + " =";
			Result = isFailure ? errorMessage : Math.Round(result, 9, MidpointRounding.AwayFromZero).ToString("#,0.#########");
			isFailure = false;

			_valueFirst = isFailure ? null : result.ToString();
			_valueSecond = null;
			_operator = null;
			isCulculated = true;
		}

		/// <summary>
		/// UIからの入力値をセットする
		/// </summary>
		/// <param name="numStr"></param>
		public void SetValue(string numStr)
		{
			// 計算直後の場合は値をクリアしてから入力を受け付ける
			if (isCulculated)
			{
				AllClearValue();
				isCulculated = false;
			}

			// operatorがセットされていれば2つ目の値が対象
			string? targetStr = _operator.HasValue ? _valueSecond : _valueFirst;
			targetStr ??= string.Empty;

			// 16桁以上の入力は無視
			if (targetStr.Replace(".", "").Length >= 16)
				return;

			if (numStr == ".")
			{
				// すでにカンマが含まれていれば無視
				if (targetStr.Contains('.'))
					return;

				// 値が空の場合は"0."とする
				if (string.IsNullOrEmpty(targetStr))
				{
					numStr = "0.";
				}
			}

			targetStr += numStr;
			if (_operator.HasValue)
			{
				_valueSecond = targetStr;
			}
			else
			{
				_valueFirst = targetStr;
			}
			Result = targetStr;
		}

		/// <summary>
		/// 正負の値を反転する
		/// </summary>
		public void InvertValue()
		{
			// operatorがセットされていれば2つ目の値が対象
			string? targetStr = _operator.HasValue ? _valueSecond : _valueFirst;
			targetStr ??= string.Empty;

			if (string.IsNullOrEmpty(targetStr))
				return;


			if (targetStr.StartsWith("-"))
			{
				targetStr = targetStr.Substring(1);
			}
			else
			{
				targetStr = "-" + targetStr;
			}

			if (_operator.HasValue)
			{
				_valueSecond = targetStr;
			}
			else
			{
				_valueFirst = targetStr;
			}
			Result = targetStr;
		}

		/// <summary>
		/// 一文字削除する
		/// </summary>
		public void DeleteValue()
		{
			// operatorがセットされていれば2つ目の値が対象
			string? targetStr = _operator.HasValue ? _valueSecond : _valueFirst;
			targetStr ??= string.Empty;

			// 値が空でなければ最後の一文字を削除
			if (!string.IsNullOrEmpty(targetStr))
			{
				targetStr = targetStr.Remove(targetStr.Length - 1, 1);
			}

			if (_operator.HasValue)
			{
				_valueSecond = targetStr;
			}
			else
			{
				_valueFirst = targetStr;
			}
			Result = targetStr;
		}

		/// <summary>
		/// 現在入力中の値をクリアする
		/// </summary>
		public void ClearValue()
		{
			// operatorがセットされていれば2つ目の値が対象
			if (_operator.HasValue)
			{
				_valueSecond = null;
			}
			else
			{
				_valueFirst = null;
			}
			Result = string.Empty;
		}

		/// <summary>
		/// すべての値をクリアする
		/// </summary>
		public void AllClearValue()
		{
			_valueFirst = null;
			_valueSecond = null;
			_operator = null;
			Result = string.Empty;
			Formula = string.Empty;
		}

		/// <summary>
		/// 四則演算子をセットする
		/// </summary>
		/// <param name="operatorStr"></param>
		public void SetOperator(string operatorStr)
		{
			isCulculated = false;

			// すでに二つ目の値が入力されている場合は無視
			if (!string.IsNullOrEmpty(_valueSecond))
				return;

			// 一つ目の値が未入力の場合スルー
			// ただし、-が入力された場合は処理を継続
			if (string.IsNullOrEmpty(_valueFirst) && operatorStr != "-")
				return;

			// 一つ目の値が-のみの場合は0に変換
			if (_valueFirst == "-")
			{
				_valueFirst = "0";
			}


			bool isOperator = true;
			string targetOpeStr = string.Empty;
			switch (operatorStr)
			{
				case "+":
					_operator = Operator.Addition;
					targetOpeStr = "+";
					break;
				case "-":
					// 一つ目の値が空であれば負にする
					if (string.IsNullOrEmpty(_valueFirst))
					{
						isOperator = false;
						_valueFirst = "-";
						Result = _valueFirst;
					}
					// 乗算、除算がすでに設定されている場合は、二つ目の値を負にする
					else if (_operator == Operator.Multiplication || _operator == Operator.Division)
					{
						isOperator = false;
						_valueSecond = "-";
						Result = _valueSecond;
					}
					else
					{
						_operator = Operator.Subtraction;
						targetOpeStr = "-";
					}
					break;
				case "*":
					_operator = Operator.Multiplication;
					targetOpeStr = "×";
					break;
				case "/":
					_operator = Operator.Division;
					targetOpeStr = "÷";
					break;
			}

			if (isOperator)
			{
				Formula = _valueFirst+ " " + targetOpeStr;
				Result = string.Empty;
			}
		}
	}
}