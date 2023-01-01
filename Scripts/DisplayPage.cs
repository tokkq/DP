using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;

namespace DailyProject_221204
{
    class PageSwitcher
    {
        readonly Dictionary<int, Page> _pageMap = null!;
        readonly Frame _frame;

        public PageSwitcher(Frame frame, Dictionary<int, Page> pageMap)
        {
            _frame = frame;
            _pageMap = pageMap;
        }

        public void Switch(int pageIndex)
        {
            if (_pageMap.TryGetValue(pageIndex, out var page))
            {
                Switch(page);
            }
            else
            {
                throw new InvalidEnumArgumentException($"[pageIndex: {pageIndex}]");
            }
        }
        public void Switch(Page page)
        {
            _frame.Navigate(page);
        }
    }
}
