using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace PullToRefreshListBox
{
    public class UCPullToRefreshListBox : ListBox
    {
        #region Private fileds

        private double _verticalCompression;
        private ScrollViewer _scrollViewer;

        #endregion

        #region Events

        public event PullToRefresh PullToRefresh;

        #endregion

        #region DependencyProperties

        private static readonly DependencyProperty IsArrowDownProperty =
            DependencyProperty.Register("IsArrowDown", typeof (bool), typeof (UCPullToRefreshListBox),
                new PropertyMetadata(true));

        private static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof (bool), typeof (UCPullToRefreshListBox),
                new PropertyMetadata(false));

        private static readonly DependencyProperty LoaderOpacityProperty =
            DependencyProperty.Register("LoaderOpacity", typeof(float), typeof(UCPullToRefreshListBox),
                new PropertyMetadata(0f));

        private static readonly DependencyProperty TextOpacityProperty =
            DependencyProperty.Register("TextOpacity", typeof(float), typeof(UCPullToRefreshListBox),
                new PropertyMetadata(1f));

        private static readonly DependencyProperty ReleaseToRefreshTextProperty =
            DependencyProperty.Register("ReleaseToRefreshText", typeof (string), typeof (UCPullToRefreshListBox),
                new PropertyMetadata("release to refresh"));

        private static readonly DependencyProperty PullToRefreshTextProperty =
            DependencyProperty.Register("PullToRefreshText", typeof (string), typeof (UCPullToRefreshListBox),
                new PropertyMetadata("pull to refresh"));

        #endregion

        #region Creation

        public UCPullToRefreshListBox()
        {
            Application.Current.Resources.Source = new Uri(
                "/PullToRefreshListBox;component/Ressources/Ressources.xaml", UriKind.Relative);
            Style = (Style) Application.Current.Resources["PullToRefreshListBoxStyle2"];

            Loaded += OnLoaded;

            #region GestureListenerConfig

            var gl = GestureService.GetGestureListener(this);
            gl.DragDelta += GestureListener_OnDragDelta;
            gl.DragCompleted += GestureListener_OnDragCompleted;

            #endregion
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = FindScrollViewer(this);
        }

        private ScrollViewer FindScrollViewer(DependencyObject parent)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childCount; i++)
            {
                var elt = VisualTreeHelper.GetChild(parent, i);
                if (elt is ScrollViewer) return (ScrollViewer) elt;
                var result = FindScrollViewer(elt);
                if (result != null) return result;
            }
            return null;
        }

        #endregion

        #region Getter & Setter

        private bool IsArrowDown
        {
            get { return (bool) GetValue(IsArrowDownProperty); }
            set { SetValue(IsArrowDownProperty, value); }
        }
        public bool IsLoading
        {
            get { return (bool) GetValue(IsLoadingProperty); }
            set
            {
                SetValue(IsLoadingProperty, value);
                SetValue(LoaderOpacityProperty, value ? 1f : 0f);
                SetValue(TextOpacityProperty, value ? 0f : 1f);
            }
        }
        public string PullToRefreshText
        {
            get { return (string) GetValue(PullToRefreshTextProperty); }
            set { SetValue(PullToRefreshTextProperty, value); }
        }
        public string ReleaseToRefreshText
        {
            get { return (string) GetValue(ReleaseToRefreshTextProperty); }
            set { SetValue(ReleaseToRefreshTextProperty, value); }
        }
        private float LoaderOpacity
        {
            get { return (float)GetValue(LoaderOpacityProperty); }
            set { SetValue(LoaderOpacityProperty, value); }
        }
        private float TextOpacity
        {
            get { return (float)GetValue(TextOpacityProperty); }
            set { SetValue(LoaderOpacityProperty, value); }
        }

        #endregion

        #region GestureListener

        private void GestureListener_OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {         
            _verticalCompression = 0;
            if (!IsArrowDown && PullToRefresh != null && !IsLoading)
            {
                PullToRefresh.Invoke(this, new EventArgs());
                IsArrowDown = true;
            }
        }

        private void GestureListener_OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            if (_scrollViewer.VerticalOffset == 0)
            {
                if (e.VerticalChange > 0)
                {
                    if (_verticalCompression < 130)
                        _verticalCompression += e.VerticalChange;
                }
                else
                    _verticalCompression += e.VerticalChange;

            }
            IsArrowDown = _verticalCompression < 125;
        }

        #endregion

    }

    public delegate void PullToRefresh(object sender, EventArgs e);
}
