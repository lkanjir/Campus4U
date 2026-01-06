using System.Windows.Controls;

namespace Client.Presentation.Views;

public partial class StudentView : UserControl
{
    public int KorisnikId { get; set; }

    public StudentView()
    {
        InitializeComponent();
    }
}