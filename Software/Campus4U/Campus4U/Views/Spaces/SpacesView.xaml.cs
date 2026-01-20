using Client.Data.Spaces;
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
    /// Interaction logic for SpacesView.xaml
    /// </summary>
    /// 
    //Marko Mišić
    public partial class SpacesView : Window
    {
        private readonly SpaceRepository prostorRepo;
        public SpacesView()
        {
            InitializeComponent();
            prostorRepo = new SpaceRepository();
        }
    }
}
