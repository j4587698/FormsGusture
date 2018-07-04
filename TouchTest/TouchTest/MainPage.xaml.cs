using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Plugin.FormsGesture.Shared;
using TouchTest.Annotations;
using Xamarin.Forms;

namespace TouchTest
{
    public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		    BindingContext = new MainPageViewModel();
            //Plugin.FormsGesture.Shared.Gesture.SetTapCommand(lbMain, new Command(args => { }));
        }
	}

    public class MainPageViewModel : INotifyPropertyChanged
    {

        public int TapCount { get; set; }

        public string CurrentPosition { get; set; }

        public string StartPosition { get; set; }

        private string status;

        public string Status {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public string TotalMove { get; set; }

        public ICommand TapCommand1 => new Command(args =>
        {
            TapCount++;
            OnPropertyChanged(nameof(TapCount));
        });

        public ICommand PanCommand1 => new Command(args =>
        {
            if (args is Gesture.PanEventArgs panEvent)
            {
                Debug.WriteLine($"Command running-GestureStatus:{panEvent.StatusType}");
                CurrentPosition = panEvent.CurrentPosition.ToString();
                StartPosition = panEvent.StartPosition.ToString();
                Status = panEvent.StatusType.ToString();
                TotalMove = panEvent.TotalMove.ToString();
                Debug.WriteLine($"{CurrentPosition} {StartPosition} {Status}");
            }
            OnPropertyChanged(nameof(CurrentPosition));
            OnPropertyChanged(nameof(StartPosition));
            OnPropertyChanged(nameof(TotalMove));
            //OnPropertyChanged(nameof(Status));
            Debug.WriteLine($"{nameof(CurrentPosition)} {nameof(StartPosition)} {nameof(Status)}");
            Debug.WriteLine("OnPropertyChanged complate");
        });

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.WriteLine($"PropertyChanged Enter, PropertyName:{propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
