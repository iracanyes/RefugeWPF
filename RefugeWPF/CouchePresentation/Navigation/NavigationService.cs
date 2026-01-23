using System;
using System.Collections.Generic;
using System.Text;

namespace RefugeWPF.CouchePresentation.Navigation
{
    class NavigationService
    {
        private readonly Action<object> _setCurrent;

        public NavigationService(Action<object> current)
        {
            _setCurrent = current; 
        }

        public void Navigate(object viewModel)
        {
            _setCurrent(viewModel);
        }
    }
}
