using Client.Domain.Spaces;
using Client.Domain.Templates;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Client.Presentation.Views.Templates
{
    /// <summary>
    /// Interaction logic for SpaceCard.xaml
    /// </summary>
    /// Nikola Kihas
    public partial class SpaceCard : UserControl
    {

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(SpaceCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty CapacityProperty =
            DependencyProperty.Register(nameof(Capacity), typeof(string), typeof(SpaceCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(SpaceCard), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register(nameof(ImagePath), typeof(string), typeof(SpaceCard), new PropertyMetadata(string.Empty));

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

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public event EventHandler<SpaceCardReserveEventArgs>? ReserveRequested;

        private void BtnRezervirajProstoriju_Click(object sender, RoutedEventArgs e)
        {
            var item = DataContext as FavoriteSpaceItem;
            if (item?.Space == null) return;

            ReserveRequested?.Invoke(this, new SpaceCardReserveEventArgs(item.Space, item.KorisnikId));
        }
    }

    public sealed class SpaceCardReserveEventArgs : EventArgs
    {
        public SpaceCardReserveEventArgs(Space? prostor, int? korisnikId)
        {
            Prostor = prostor;
            KorisnikId = korisnikId;
        }

        public Space? Prostor { get; }
        public int? KorisnikId { get; }
    }
}
