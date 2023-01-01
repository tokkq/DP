using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DailyProject_221204
{ 
    public class DPComboBox : ComboBox
    {
        static DPComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DPComboBox), new FrameworkPropertyMetadata(typeof(DPComboBox)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            SelectedChangeCommand.Execute(null);
        }

        public ICommand SelectedChangeCommand
        {
            get { return (ICommand)GetValue(SelectedChangeCommandProperty); }
            set { SetValue(SelectedChangeCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedChangeCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedChangeCommandProperty =
            DependencyProperty.Register("SelectedChangeCommand", typeof(ICommand), typeof(DPComboBox));
    }
}