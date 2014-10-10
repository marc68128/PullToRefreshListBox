using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Phone.Controls;
using PullToRefreshListBox;
using ReactiveUI;

namespace PullToRefreshListBoxDemo
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageViewModel();
            SetBindings();
            ViewModel.LoadData.Execute(null);
        }

        private void SetBindings()
        {
            ViewModel.WhenAnyValue(vm => vm.IsLoading)
                .Subscribe(x => Dispatcher.BeginInvoke(() => PullToRefreshListBox.IsLoading = x)); 
        }

        private MainPageViewModel ViewModel
        {
            get { return DataContext as MainPageViewModel;  }
        }

        private void PullToRefresh(object sender, EventArgs eventArgs)
        {
            ViewModel.LoadData.Execute(null);
        }
      
    }


}