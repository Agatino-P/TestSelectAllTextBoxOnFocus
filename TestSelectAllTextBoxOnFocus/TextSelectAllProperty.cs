using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSelectAllTextBoxOnFocus
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public class TextBoxAttachedDP : DependencyObject
    {
        public static readonly DependencyProperty FocusSelectAllProperty = DependencyProperty.RegisterAttached(
            "FocusSelectAll",
            typeof(bool),
            typeof(TextBoxAttachedDP),
            new PropertyMetadata(false, FocusSelectAllPropertyChanged));

        private static void FocusSelectAllPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                TextBox textBox = d as TextBox;
                if ((e.NewValue as bool?).GetValueOrDefault(false))
                {
                    textBox.GotKeyboardFocus += OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                }
                else
                {
                    textBox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                }
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dependencyObject = GetParentFromVisualTree(e.OriginalSource);

            if (dependencyObject == null)
            {
                return;
            }

            var textBox = (TextBox)dependencyObject;
            if (!textBox.IsKeyboardFocusWithin)
            {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static DependencyObject GetParentFromVisualTree(object source)
        {
            DependencyObject parent = source as UIElement;
            while (parent != null && !(parent is TextBox))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetFocusSelectAll(DependencyObject @object)
        {
            return (bool)@object.GetValue(FocusSelectAllProperty);
        }

        public static void SetFocusSelectAll(DependencyObject @object, bool value)
        {
            @object.SetValue(FocusSelectAllProperty, value);
        }
    }
}
