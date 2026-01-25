using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CalculatorWpf.Commands;
using CalculatorWpf.Models;

namespace CalculatorWpf
{
	public class CalculatorViewModel : INotifyPropertyChanged
	{
		// 入力表示部分(データバインディング)
		private string _mainView = string.Empty;
		public string MainView
		{
			get => _mainView;
			set
			{
				if (_mainView != value)
				{
					_mainView = value;
					OnPropertyChanged();
				}
			}
		}

		// 途中式表示部分(データバインディング)
		private string _subView = string.Empty;
		public string SubView
		{
			get => _subView;
			set
			{
				if (_subView != value)
				{
					_subView = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand PushEqualCommand => _pushEqualCommand;
		public ICommand PushNumCommand => _pushNumCommand;
		public ICommand PushOperatorCommand => _pushOperatorCommand;
		public ICommand PushInvertCommand => _pushInvertCommand;
		public ICommand PushDeleteCommand => _pushDeleteCommand;
		public ICommand PushClearCommand => _pushClearCommand;
		public ICommand PushAllClearCommand => _pushAllClearCommand;

		private readonly CalculatorModel _model;
		private readonly SimpleCommand _pushEqualCommand;
		private readonly SimpleCommand _pushNumCommand;
		private readonly SimpleCommand _pushOperatorCommand;
		private readonly SimpleCommand _pushInvertCommand;
		private readonly SimpleCommand _pushDeleteCommand;
		private readonly SimpleCommand _pushClearCommand;
		private readonly SimpleCommand _pushAllClearCommand;

		public CalculatorViewModel()
		{
			_model = new CalculatorModel();

			_pushEqualCommand = new SimpleCommand(_ =>
			{
				_model.Culculate();
				UpdateViews();
			}, _ => _model.CanCulculate);
			_pushNumCommand = new SimpleCommand(param =>
			{
				if (param != null)
				{
					_model.SetValue((string)param);
					UpdateViews();
				}
			});
			_pushOperatorCommand = new SimpleCommand(param =>
			{
				if (param != null)
				{
					_model.SetOperator((string)param);
					UpdateViews();
				}
			});
			_pushInvertCommand = new SimpleCommand(_ =>
			{ 
				_model.InvertValue();
				UpdateViews();
			});
			_pushDeleteCommand = new SimpleCommand(_ =>
			{
				_model.DeleteValue();
				UpdateViews();
			});
			_pushClearCommand = new SimpleCommand(_ =>
			{
				_model.ClearValue();
				UpdateViews();
			});
			_pushAllClearCommand = new SimpleCommand(_ =>
			{
				_model.AllClearValue();
				UpdateViews();
			});
		}

		private void UpdateViews()
		{
			MainView = _model.Result;
			SubView = _model.Formula;
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			// CallerMemberName属性を使用して、呼び出し元のプロパティ名を自動的に取得
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			if (propertyName == nameof(MainView))
			{
				_pushEqualCommand.RaiseCanExecuteChanged();
			}
		}
	}
}