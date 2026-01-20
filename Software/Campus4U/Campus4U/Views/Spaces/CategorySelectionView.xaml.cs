using Client.Domain.Spaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Presentation.Views.Spaces
{
    /// <summary>
    /// Interaction logic for CategorySelectionView.xaml
    /// </summary>
    public partial class CategorySelectionView : Window
    {
        public CategorySelectionView()
        {
            InitializeComponent();
        }

        private void BtnUcionice_Click(object sender, RoutedEventArgs e)
        {
            SpacesView spacesWindow = new SpacesView(TipProstora.Ucionica);
            spacesWindow.Show();
            this.Close();
        }

        private void BtnTeretane_Click(object sender, RoutedEventArgs e)
        {
            SpacesView spacesWindow = new SpacesView(TipProstora.Teretana);
            spacesWindow.Show();
            this.Close();
        }
    }
}
