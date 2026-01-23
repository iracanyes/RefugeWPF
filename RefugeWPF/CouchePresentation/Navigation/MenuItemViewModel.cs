/**
 * <summary>
 *  Represent an item or sub-item of the menu
 * </summary>
 */ 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace RefugeWPF.CouchePresentation.Navigation
{
    class MenuItemViewModel
    {
        public MenuItemViewModel() { Header = "Inconnu"; }

        public MenuItemViewModel(string header) { 
            Header = header;
        }

        public string Header { get; set; }
        public string? IconGlyph { get; set; }
        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }

        public ObservableCollection<MenuItemViewModel> Children { get; } = new ObservableCollection<MenuItemViewModel>();
    }
}
