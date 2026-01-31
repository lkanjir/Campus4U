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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client.Presentation.Views.Templates
{
    /// <summary>
    /// Interaction logic for SpaceCard.xaml
    /// </summary>
    /// Nikola Kihas
    public partial class SpaceCard : UserControl
    {
        private static readonly ImageSource DefaultImage =
          new BitmapImage(new Uri("pack://application:,,,/Images/no-img.png", UriKind.Absolute));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(SpaceCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CapacityProperty =
            DependencyProperty.Register(nameof(Capacity), typeof(string), typeof(SpaceCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(SpaceCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(nameof(Image), typeof(ImageSource), typeof(SpaceCard), new PropertyMetadata(DefaultImage));
        public SpaceCard()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Capacity
        {
            get => (string)GetValue(CapacityProperty);
            set => SetValue(CapacityProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }
    }
}
