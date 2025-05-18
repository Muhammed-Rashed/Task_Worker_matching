using Avalonia.Controls;

namespace Task_worker_matching.Controllers
{
    public class Navigator
    {
        private static Navigator? _instance;
        public static Navigator Instance => _instance ??= new Navigator();

        private ContentControl? _mainContentControl;

        public void SetContentControl(ContentControl contentControl)
        {
            _mainContentControl = contentControl;
        }

        public void Navigate(UserControl page)
        {
            if (_mainContentControl != null)
            {
                _mainContentControl.Content = page;
            }
        }
    }
}