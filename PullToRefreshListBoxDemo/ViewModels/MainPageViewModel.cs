using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using ReactiveUI;

namespace PullToRefreshListBoxDemo
{
    public class MainPageViewModel : ReactiveObject
    {
        readonly Random _random = new Random((int)DateTime.Now.Ticks);

        public ReactiveList<ListItem> List { get; set; }
        public ReactiveCommand LoadData;

        public MainPageViewModel()
        {
            LoadData = new ReactiveCommand();
            List = new ReactiveList<ListItem>();

            SetupLoadingSequence();
        }

        private void SetupLoadingSequence()
        {
            var dataLoaded = LoadData.RegisterAsync(_ => Observable.Start(() => GenerateList()));

            dataLoaded.Subscribe(l =>
            {
                using (List.SuppressChangeNotifications())
                {
                    List.Clear();
                    l.ForEach(List.Add);
                }
                IsLoading = false;
            });
        }

        #region ListGeneration

        private List<ListItem> GenerateList()
        {
            IsLoading = true;
            var ret = new List<ListItem>();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                ret.Add(new ListItem
                {
                    Title = RandomString(_random.Next(1, 10)),
                    ImagePath = "/Images/" + _random.Next(1, 5) + ".jpg"
                });
            }

            return ret;

        }

        private string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26*_random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        #endregion

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { this.RaiseAndSetIfChanged(ref _isLoading, value); }
        }

    }
}
