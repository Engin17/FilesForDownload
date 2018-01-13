using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XUpload
{
    /// <summary>
    /// Custom TextBox with Select text property
    /// </summary>
    public class SmartTextBox : TextBox
    {
        public static readonly DependencyProperty IsSelectedTextProperty = DependencyProperty.RegisterAttached("IsSelectedText",
        typeof(bool), typeof(SmartTextBox), new FrameworkPropertyMetadata(false, OnIsSelectedChanged));

        public bool IsSelectedText
        {
            get { return (bool)GetValue(IsSelectedTextProperty); }
            set { SetValue(IsSelectedTextProperty, value); }
        }

        private static void OnIsSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SmartTextBox textbox = sender as SmartTextBox;
            if ((bool)e.NewValue)
            {
                textbox.Focus();
                textbox.SelectAll();
            }
        }

    }
}
