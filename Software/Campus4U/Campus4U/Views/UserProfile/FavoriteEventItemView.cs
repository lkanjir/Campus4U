using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client.Presentation.Views.UserProfile
{
    public sealed class FavoriteEventItemView
    {
        public int EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ImageSource? Image { get; set; }
    }
}
