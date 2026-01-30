using Client.Presentation.Views.Spaces;
using System.Windows.Controls;

namespace Client.Presentation.Views;

public partial class StudentView : UserControl
{
    public int KorisnikId { get; set; }

    public StudentView()
    {
        InitializeComponent();
    }

    private void BtnRezerviraj_Click(object sender, EventArgs e)
    {
        CategorySelectionView reservationView = new CategorySelectionView(KorisnikId);
        reservationView.Show();
    }
}