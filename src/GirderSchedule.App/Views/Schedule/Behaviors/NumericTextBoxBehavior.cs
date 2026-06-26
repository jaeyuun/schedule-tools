using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GirderSchedule.App.Behaviors
{
    public static class NumericTextBoxBehavior
    {
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(NumericTextBoxBehavior), new PropertyMetadata(null, OnPropertyNameChanged));
        public static readonly DependencyProperty AllowNegativeProperty = DependencyProperty.RegisterAttached("AllowNegative", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));
        public static readonly DependencyProperty NormalizeOnLostFocusProperty = DependencyProperty.RegisterAttached("NormalizeOnLostFocus", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(true));

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

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;

            if (textBox == null)
            {
                return;
            }

            textBox.Loaded -= TextBox_Loaded;
            textBox.DataContextChanged -= TextBox_DataContextChanged;
            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.LostFocus -= TextBox_LostFocus;
            DataObject.RemovePastingHandler(textBox, TextBox_Pasting);

            if (string.IsNullOrWhiteSpace(e.NewValue as string))
            {
                return;
            }

            textBox.Loaded += TextBox_Loaded;
            textBox.DataContextChanged += TextBox_DataContextChanged;
            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.LostFocus += TextBox_LostFocus;
            DataObject.AddPastingHandler(textBox, TextBox_Pasting);
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshText(sender as TextBox);
        }

        private static void TextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RefreshText(sender as TextBox);
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                e.Handled = true;
                return;
            }

            var text = GetProposedText(textBox, e.Text);
            e.Handled = !IsAllowedText(text, GetAllowNegative(textBox));
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

            if (textBox == null)
            {
                e.CancelCommand();
                return;
            }

            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            var pasteText = e.DataObject.GetData(typeof(string)) as string;
            var text = GetProposedText(textBox, pasteText);

            if (!IsAllowedText(text, GetAllowNegative(textBox)))
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

            ApplyTextToSource(sender as TextBox, false);
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
            var propertyName = GetPropertyName(textBox);

            if (source == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            var property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (property == null || !property.CanWrite)
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

        private static bool TryGetNumber(string text, out double number)
        {
            number = 0.0;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            text = text.Trim();

            if (text == "-" || text == "." || text == "-.")
            {
                number = 0.0;
                return true;
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

                if (string.IsNullOrWhiteSpace(text) || text == "-")
                {
                    number = 0.0;
                    return true;
                }
            }

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number))
            {
                return true;
            }

            return double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out number);
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

            var text = FormatNumber(number);

            _isUpdatingText = true;
            textBox.Text = text;
            textBox.CaretIndex = textBox.Text.Length;
            _isUpdatingText = false;
        }

        private static void RefreshText(TextBox textBox)
        {
            if (textBox == null)
            {
                return;
            }

            var source = textBox.DataContext;
            var propertyName = GetPropertyName(textBox);

            if (source == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return;
            }

            var property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (property == null || !property.CanRead)
            {
                return;
            }

            var value = property.GetValue(source, null);
            var text = FormatValue(value);

            _isUpdatingText = true;
            textBox.Text = text;
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
            return value.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}