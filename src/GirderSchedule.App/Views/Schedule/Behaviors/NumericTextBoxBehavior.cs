using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GirderSchedule.App.Behaviors
{
    public static class NumericTextBoxBehavior
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(NumericTextBoxBehavior), new PropertyMetadata(null, OnPropertyNameChanged));
        public static readonly DependencyProperty AllowNegativeProperty = DependencyProperty.RegisterAttached("AllowNegative", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));
        public static readonly DependencyProperty NormalizeOnLostFocusProperty = DependencyProperty.RegisterAttached("NormalizeOnLostFocus", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(true));
        public static readonly DependencyProperty UpdateSourceOnTextChangedProperty = DependencyProperty.RegisterAttached("UpdateSourceOnTextChanged", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));

        private static readonly DependencyProperty SourceNotifierProperty = DependencyProperty.RegisterAttached("SourceNotifier", typeof(INotifyPropertyChanged), typeof(NumericTextBoxBehavior), new PropertyMetadata(null));
        private static readonly DependencyProperty SourceHandlerProperty = DependencyProperty.RegisterAttached("SourceHandler", typeof(PropertyChangedEventHandler), typeof(NumericTextBoxBehavior), new PropertyMetadata(null));

        private static bool _isUpdatingText;

        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }

        public static bool GetAllowNegative(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowNegativeProperty);
        }

        public static void SetAllowNegative(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowNegativeProperty, value);
        }

        public static bool GetNormalizeOnLostFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(NormalizeOnLostFocusProperty);
        }

        public static void SetNormalizeOnLostFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(NormalizeOnLostFocusProperty, value);
        }

        public static bool GetUpdateSourceOnTextChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdateSourceOnTextChangedProperty);
        }

        public static void SetUpdateSourceOnTextChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdateSourceOnTextChangedProperty, value);
        }

        public static void Refresh(DependencyObject root)
        {
            if (root == null)
            {
                return;
            }

            var textBox = root as TextBox;

            if (textBox != null && !string.IsNullOrWhiteSpace(GetPropertyName(textBox)))
            {
                RefreshText(textBox);
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(root);

            for (var i = 0; i < childrenCount; i++)
            {
                Refresh(VisualTreeHelper.GetChild(root, i));
            }
        }

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;

            if (textBox == null)
            {
                return;
            }

            Detach(textBox);

            if (string.IsNullOrWhiteSpace(e.NewValue as string))
            {
                return;
            }

            Attach(textBox);
            SubscribeSource(textBox);
        }

        private static void Attach(TextBox textBox)
        {
            textBox.Loaded += TextBox_Loaded;
            textBox.Unloaded += TextBox_Unloaded;
            textBox.DataContextChanged += TextBox_DataContextChanged;
            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.LostFocus += TextBox_LostFocus;

            DataObject.AddPastingHandler(textBox, TextBox_Pasting);
        }

        private static void Detach(TextBox textBox)
        {
            UnsubscribeSource(textBox);

            textBox.Loaded -= TextBox_Loaded;
            textBox.Unloaded -= TextBox_Unloaded;
            textBox.DataContextChanged -= TextBox_DataContextChanged;
            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.LostFocus -= TextBox_LostFocus;

            DataObject.RemovePastingHandler(textBox, TextBox_Pasting);
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            SubscribeSource(textBox);
            RefreshText(textBox);
        }

        private static void TextBox_Unloaded(object sender, RoutedEventArgs e)
        {
            UnsubscribeSource(sender as TextBox);
        }

        private static void TextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            SubscribeSource(textBox);
            RefreshText(textBox);
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                e.Handled = true;
                return;
            }

            var proposedText = GetProposedText(textBox, e.Text);
            e.Handled = !IsAllowedText(proposedText, GetAllowNegative(textBox));
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null || !e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            var pasteText = e.DataObject.GetData(typeof(string)) as string;
            var proposedText = GetProposedText(textBox, pasteText);

            if (!IsAllowedText(proposedText, GetAllowNegative(textBox)))
            {
                e.CancelCommand();
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isUpdatingText)
            {
                return;
            }

            var textBox = sender as TextBox;

            if (textBox == null || !GetUpdateSourceOnTextChanged(textBox))
            {
                return;
            }

            ApplyTextToSource(textBox, false);
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            ApplyTextToSource(textBox, true);

            if (GetNormalizeOnLostFocus(textBox))
            {
                NormalizeText(textBox);
            }
        }

        private static void SubscribeSource(TextBox textBox)
        {
            if (textBox == null)
            {
                return;
            }

            UnsubscribeSource(textBox);

            var notifier = textBox.DataContext as INotifyPropertyChanged;

            if (notifier == null)
            {
                ClearSourceSubscription(textBox);
                return;
            }

            PropertyChangedEventHandler handler = delegate (object sender, PropertyChangedEventArgs e)
            {
                Source_PropertyChanged(textBox, e);
            };

            notifier.PropertyChanged += handler;

            textBox.SetValue(SourceNotifierProperty, notifier);
            textBox.SetValue(SourceHandlerProperty, handler);
        }

        private static void UnsubscribeSource(TextBox textBox)
        {
            if (textBox == null)
            {
                return;
            }

            var notifier = textBox.GetValue(SourceNotifierProperty) as INotifyPropertyChanged;
            var handler = textBox.GetValue(SourceHandlerProperty) as PropertyChangedEventHandler;

            if (notifier != null && handler != null)
            {
                notifier.PropertyChanged -= handler;
            }

            ClearSourceSubscription(textBox);
        }

        private static void ClearSourceSubscription(TextBox textBox)
        {
            textBox.SetValue(SourceNotifierProperty, null);
            textBox.SetValue(SourceHandlerProperty, null);
        }

        private static void Source_PropertyChanged(TextBox textBox, PropertyChangedEventArgs e)
        {
            if (textBox == null || textBox.IsKeyboardFocusWithin)
            {
                return;
            }

            var propertyName = GetPropertyName(textBox);

            if (!string.IsNullOrWhiteSpace(e.PropertyName) && e.PropertyName != propertyName)
            {
                return;
            }

            textBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    RefreshText(textBox);
                }
            }));
        }

        private static string GetProposedText(TextBox textBox, string input)
        {
            var text = textBox.Text ?? string.Empty;
            var selectionStart = textBox.SelectionStart;
            var selectionLength = textBox.SelectionLength;

            if (selectionLength > 0)
            {
                text = text.Remove(selectionStart, selectionLength);
            }

            return text.Insert(selectionStart, input ?? string.Empty);
        }

        private static bool IsAllowedText(string text, bool allowNegative)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            var dotCount = 0;
            var minusCount = 0;

            for (var i = 0; i < text.Length; i++)
            {
                var c = text[i];

                if (char.IsDigit(c))
                {
                    continue;
                }

                if (c == '.')
                {
                    dotCount++;

                    if (dotCount > 1)
                    {
                        return false;
                    }

                    continue;
                }

                if (c == '-' && allowNegative && i == 0)
                {
                    minusCount++;

                    if (minusCount > 1)
                    {
                        return false;
                    }

                    continue;
                }

                return false;
            }

            return true;
        }

        private static void ApplyTextToSource(TextBox textBox, bool force)
        {
            if (textBox == null)
            {
                return;
            }

            var source = textBox.DataContext;
            var property = GetSourceProperty(textBox, source, true);

            if (source == null || property == null)
            {
                return;
            }

            var text = textBox.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(text))
            {
                if (force)
                {
                    SetEmptyValue(source, property);
                }

                return;
            }

            double number;

            if (!TryGetNumber(text, out number))
            {
                return;
            }

            SetValue(source, property, number);
        }

        private static void RefreshText(TextBox textBox)
        {
            if (textBox == null)
            {
                return;
            }

            var source = textBox.DataContext;
            var property = GetSourceProperty(textBox, source, false);

            if (source == null || property == null)
            {
                return;
            }

            var value = property.GetValue(source, null);
            SetText(textBox, FormatValue(value));
        }

        private static PropertyInfo GetSourceProperty(TextBox textBox, object source, bool requireWrite)
        {
            if (source == null)
            {
                return null;
            }

            var propertyName = GetPropertyName(textBox);

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }

            var property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (property == null)
            {
                return null;
            }

            if (requireWrite && !property.CanWrite)
            {
                return null;
            }

            if (!requireWrite && !property.CanRead)
            {
                return null;
            }

            return property;
        }

        private static bool TryGetNumber(string text, out double number)
        {
            number = 0.0;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            text = NormalizeNumberText(text.Trim());

            if (string.IsNullOrWhiteSpace(text))
            {
                number = 0.0;
                return true;
            }

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number))
            {
                return true;
            }

            return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out number);
        }

        private static string NormalizeNumberText(string text)
        {
            if (text == "-" || text == "." || text == "-.")
            {
                return string.Empty;
            }

            if (text.StartsWith("."))
            {
                text = "0" + text;
            }
            else if (text.StartsWith("-."))
            {
                text = "-0" + text.Substring(1);
            }

            if (text.EndsWith("."))
            {
                text = text.Substring(0, text.Length - 1);
            }

            if (text == "-")
            {
                return string.Empty;
            }

            return text;
        }

        private static void SetValue(object source, PropertyInfo property, double number)
        {
            var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
            var type = nullableType ?? property.PropertyType;

            if (type == typeof(string))
            {
                property.SetValue(source, FormatNumber(number), null);
                return;
            }

            if (type == typeof(double))
            {
                property.SetValue(source, number, null);
                return;
            }

            if (type == typeof(int))
            {
                property.SetValue(source, (int)number, null);
            }
        }

        private static void SetEmptyValue(object source, PropertyInfo property)
        {
            var nullableType = Nullable.GetUnderlyingType(property.PropertyType);
            var type = nullableType ?? property.PropertyType;

            if (property.PropertyType == typeof(string))
            {
                property.SetValue(source, string.Empty, null);
                return;
            }

            if (nullableType != null)
            {
                property.SetValue(source, null, null);
                return;
            }

            if (type == typeof(double))
            {
                property.SetValue(source, 0.0, null);
                return;
            }

            if (type == typeof(int))
            {
                property.SetValue(source, 0, null);
            }
        }

        private static void NormalizeText(TextBox textBox)
        {
            double number;

            if (!TryGetNumber(textBox.Text, out number))
            {
                RefreshText(textBox);
                return;
            }

            SetText(textBox, FormatNumber(number));
        }

        private static void SetText(TextBox textBox, string text)
        {
            _isUpdatingText = true;

            textBox.Text = text ?? string.Empty;
            textBox.CaretIndex = textBox.Text.Length;

            _isUpdatingText = false;
        }

        private static string FormatValue(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is double)
            {
                return FormatNumber((double)value);
            }

            return value.ToString();
        }

        private static string FormatNumber(double value)
        {
            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}