using System.Collections.ObjectModel;
using System.Windows.Controls;
using Client.Application.EventFeedBack;

namespace Client.Presentation.Views.EventFeedBack
{
    public partial class EventFeedBackUserControl : UserControl
    {
        public ObservableCollection<EventFeedbackComment> Comments { get; } = new();

        public EventFeedBackUserControl()
        {
            InitializeComponent();
        }
    }
}
