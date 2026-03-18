using System.Windows.Input;

namespace IronConvert;

/// <summary>
/// A command class that can be executed, typically used in MVVM patterns to bind actions to UI elements.
/// </summary>
public class RelayCommand : ICommand
{
    private readonly Action<object?> execute;
    private readonly Predicate<object?> canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<object?> execute)
    {
        this.execute = execute;
        canExecute = Always;
    }

    public RelayCommand(Action<object?> execute, Predicate<object?> canExecute)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object? parameter = null) => canExecute(parameter);
    public void Execute(object? parameter = null) => execute(parameter);

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public static Predicate<object?> Always => _ => true;
    public static Predicate<object?> WhenNotNull => _ => _ is not null;

}