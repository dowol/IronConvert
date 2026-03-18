using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IronConvert;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainDataContext Context => (MainDataContext)DataContext;

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(Context.DestinationTypeIndex < 0)
        {
            Context.DestinationTypeIndex = 0;
        }

        if(Context.SourceFile is not null)
        {
            string? destext = Context.DestinationType?.Extensions.FirstOrDefault();
            Context.DestinationFile = new(new(Context.SourceFile!.FullNameWithoutExtension + (string.IsNullOrWhiteSpace(destext) ? Context.SourceFile!.Extensions : destext)));
        }
    }
}