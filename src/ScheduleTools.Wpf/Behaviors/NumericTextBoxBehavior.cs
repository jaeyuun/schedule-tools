using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ScheduleTools.Wpf.Behaviors
{
    public static class NumericTextBoxBehavior
    {
        private static readonly DependencyProperty IsInternalUpdateProperty = DependencyProperty.RegisterAttached("IsInternalUpdate", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));
        private static readonly DependencyProperty OriginalValueProperty = DependencyProperty.RegisterAttached("OriginalValue", typeof(object), typeof(NumericTextBoxBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(NumericTextBoxBehavior), new PropertyMetadata(string.Empty, OnPropertyNameChanged));
        public static readonly DependencyProperty AllowNegativeProperty = DependencyProperty.RegisterAttached("AllowNegative", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));
        public static readonly DependencyProperty UpdateSourceOnTextChangedProperty = DependencyProperty.RegisterAttached("UpdateSourceOnTextChanged", typeof(bool), typeof(NumericTextBoxBehavior), new PropertyMetadata(false));

        public static string GetPropertyName(DependencyObject obj)
        {
            return obj.GetValue(PropertyNameProperty) as string ?? string.Empty;
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

        public static bool GetUpdateSourceOnTextChanged(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdateSourceOnTextChangedProperty);
        }

        public static void SetUpdateSourceOnTextChanged(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdateSourceOnTextChangedProperty, value);
        }

        private static bool GetIsInternalUpdate(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsInternalUpdateProperty);
        }

        private static void SetIsInternalUpdate(DependencyObject obj, bool value)
        {
            obj.SetValue(IsInternalUpdateProperty, value);
        }

        private static object? GetOriginalValue(DependencyObject obj)
        {
            return obj.GetValue(OriginalValueProperty);
        }

        private static void SetOriginalValue(DependencyObject obj, object? value)
        {
            obj.SetValue(OriginalValueProperty, value);
        }

        private static void OnPropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as TextBox;

            if (textBox == null)
            {
                return;
            }

            DetachEvents(textBox);

            if (string.IsNullOrWhiteSpace(e.NewValue as string))
            {
                return;
            }

            AttachEvents(textBox);
            RefreshTextBox(textBox);
        }

        private static void AttachEvents(TextBox textBox)
        {
            textBox.Loaded += TextBox_Loaded;
            textBox.DataContextChanged += TextBox_DataContextChanged;
            textBox.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            textBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.TextChanged += TextBox_TextChanged;
            DataObject.AddPastingHandler(textBox, TextBox_Pasting);
        }

        private static void DetachEvents(TextBox textBox)
        {
            textBox.Loaded -= TextBox_Loaded;
            textBox.DataContextChanged -= TextBox_DataContextChanged;
            textBox.GotKeyboardFocus -= TextBox_GotKeyboardFocus;
            textBox.LostKeyboardFocus -= TextBox_LostKeyboardFocus;
            textBox.PreviewTextInput -= TextBox_PreviewTextInput;
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.TextChanged -= TextBox_TextChanged;
            DataObject.RemovePastingHandler(textBox, TextBox_Pasting);
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            RefreshTextBox(textBox);
        }

        private static void TextBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            RefreshTextBox(textBox);
        }

        private static void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            SetOriginalValue(textBox, GetSourceValue(textBox));
        }

        private static void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            CommitValue(textBox);
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            var candidateText = GetCandidateText(textBox, e.Text);

            if (!IsAllowedIntermediateText(textBox, candidateText))
            {
                e.Handled = true;
            }
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            if (e.Key == Key.Enter)
            {
                CommitValue(textBox);
                MoveFocusNext(textBox);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Escape)
            {
                RestoreOriginalValue(textBox);
                MoveFocusNext(textBox);
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null || GetIsInternalUpdate(textBox))
            {
                return;
            }

            if (!GetUpdateSourceOnTextChanged(textBox))
            {
                return;
            }

            TryUpdateSourceWhileTyping(textBox);
        }

        private static void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            var pastedText = e.DataObject.GetData(DataFormats.Text) as string ?? string.Empty;
            var candidateText = GetCandidateText(textBox, pastedText);

            if (!IsAllowedIntermediateText(textBox, candidateText))
            {
                e.CancelCommand();
            }
        }

        private static void TryUpdateSourceWhileTyping(TextBox textBox)
        {
            var text = NormalizeInputText(textBox.Text);

            if (IsIntermediateText(text))
            {
                return;
            }

            var property = GetTargetProperty(textBox);

            if (property == null)
            {
                return;
            }


            if (!TryConvertValue(property.PropertyType, text, out object? convertedValue))
            {
                return;
            }

            SetSourceValue(textBox, property, convertedValue);
        }

        private static void CommitValue(TextBox textBox)
        {
            var property = GetTargetProperty(textBox);

            if (property == null)
            {
                return;
            }

            var text = NormalizeCommitText(textBox.Text, property.PropertyType);


            if (!TryConvertValue(property.PropertyType, text, out object? convertedValue))
            {
                RestoreOriginalValue(textBox);
                return;
            }

            SetSourceValue(textBox, property, convertedValue);
            SetOriginalValue(textBox, convertedValue);
            SetTextWithoutNotification(textBox, FormatValue(convertedValue));
        }

        private static string NormalizeCommitText(string text, Type propertyType)
        {
            text = NormalizeInputText(text);

            if (string.IsNullOrWhiteSpace(text) || text == "-" || text == "." || text == "-.")
            {
                return "0";
            }

            if (IsIntegerType(propertyType))
            {
                return text;
            }

            if (text.StartsWith(".", StringComparison.Ordinal))
            {
                text = "0" + text;
            }
            else if (text.StartsWith("-.", StringComparison.Ordinal))
            {
                text = "-0" + text.Substring(1);
            }

            if (text.EndsWith(".", StringComparison.Ordinal))
            {
                text = text.TrimEnd('.');
            }

            if (string.IsNullOrWhiteSpace(text) || text == "-")
            {
                return "0";
            }

            return text;
        }

        private static string NormalizeInputText(string text)
        {
            return (text ?? string.Empty)
                .Trim()
                .Replace(',', '.');
        }

        private static bool IsAllowedIntermediateText(TextBox textBox, string text)
        {
            text = NormalizeInputText(text);

            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            var property = GetTargetProperty(textBox);

            if (property == null)
            {
                return false;
            }

            var isInteger = IsIntegerType(property.PropertyType);
            var allowNegative = GetAllowNegative(textBox);

            if (text == "-")
            {
                return allowNegative;
            }

            if (!isInteger && text == ".")
            {
                return true;
            }

            if (!isInteger && text == "-.")
            {
                return allowNegative;
            }

            if (!allowNegative && text.Contains("-"))
            {
                return false;
            }

            if (text.IndexOf('-', 1) >= 0)
            {
                return false;
            }

            if (isInteger && text.Contains("."))
            {
                return false;
            }

            if (!isInteger && CountCharacter(text, '.') > 1)
            {
                return false;
            }

            if (isInteger)
            {
                int intValue;
                return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue);
            }

            double doubleValue;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out doubleValue);
        }

        private static bool IsIntermediateText(string text)
        {
            return string.IsNullOrWhiteSpace(text) ||
                   text == "-" ||
                   text == "." ||
                   text == "-." ||
                   text.EndsWith(".", StringComparison.Ordinal);
        }

        private static int CountCharacter(string text, char value)
        {
            var count = 0;

            for (var i = 0; i < text.Length; i++)
            {
                if (text[i] == value)
                {
                    count++;
                }
            }

            return count;
        }

        private static string GetCandidateText(TextBox textBox, string inputText)
        {
            var currentText = textBox.Text ?? string.Empty;
            var selectionStart = textBox.SelectionStart;
            var selectionLength = textBox.SelectionLength;

            if (selectionStart < 0)
            {
                selectionStart = 0;
            }

            if (selectionStart > currentText.Length)
            {
                selectionStart = currentText.Length;
            }

            if (selectionLength < 0)
            {
                selectionLength = 0;
            }

            if (selectionStart + selectionLength > currentText.Length)
            {
                selectionLength = currentText.Length - selectionStart;
            }

            return currentText.Remove(selectionStart, selectionLength).Insert(selectionStart, inputText ?? string.Empty);
        }

        private static PropertyInfo? GetTargetProperty(TextBox textBox)
        {
            var propertyName = GetPropertyName(textBox);

            if (string.IsNullOrWhiteSpace(propertyName) || textBox.DataContext == null)
            {
                return null;
            }

            var property = textBox.DataContext.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

            if (property == null || !property.CanRead || !property.CanWrite)
            {
                return null;
            }

            return property;
        }

        private static object? GetSourceValue(TextBox textBox)
        {
            var property = GetTargetProperty(textBox);

            if (property == null || textBox.DataContext == null)
            {
                return null;
            }

            return property.GetValue(textBox.DataContext);
        }

        private static void SetSourceValue(TextBox textBox, PropertyInfo property, object? value)
        {
            if (textBox.DataContext == null || property == null)
            {
                return;
            }

            var currentValue = property.GetValue(textBox.DataContext);

            if (Equals(currentValue, value))
            {
                return;
            }

            property.SetValue(textBox.DataContext, value);
        }

        private static bool TryConvertValue(Type propertyType, string text, out object? convertedValue)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            if (underlyingType == typeof(int))
            {
                int value;

                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    convertedValue = value;
                    return true;
                }
            }
            else if (underlyingType == typeof(double))
            {
                double value;

                if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    convertedValue = value;
                    return true;
                }
            }
            else if (underlyingType == typeof(float))
            {
                float value;

                if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    convertedValue = value;
                    return true;
                }
            }
            else if (underlyingType == typeof(decimal))
            {
                decimal value;

                if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    convertedValue = value;
                    return true;
                }
            }
            else if (underlyingType == typeof(long))
            {
                long value;

                if (long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
                {
                    convertedValue = value;
                    return true;
                }
            }

            convertedValue = null;
            return false;
        }

        private static bool IsIntegerType(Type propertyType)
        {
            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            return underlyingType == typeof(byte) ||
                   underlyingType == typeof(short) ||
                   underlyingType == typeof(int) ||
                   underlyingType == typeof(long) ||
                   underlyingType == typeof(sbyte) ||
                   underlyingType == typeof(ushort) ||
                   underlyingType == typeof(uint) ||
                   underlyingType == typeof(ulong);
        }

        private static string FormatValue(object? value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value is double doubleValue)
            {
                return doubleValue.ToString("0.################", CultureInfo.InvariantCulture);
            }

            if (value is float floatValue)
            {
                return floatValue.ToString("0.################", CultureInfo.InvariantCulture);
            }

            if (value is decimal decimalValue)
            {
                return decimalValue.ToString("0.################", CultureInfo.InvariantCulture);
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static void RestoreOriginalValue(TextBox textBox)
        {
            var originalValue = GetOriginalValue(textBox);

            if (originalValue == null)
            {
                originalValue = GetSourceValue(textBox);
            }

            SetTextWithoutNotification(textBox, FormatValue(originalValue));
        }

        private static void RefreshTextBox(TextBox textBox)
        {
            if (textBox == null || textBox.IsKeyboardFocusWithin)
            {
                return;
            }

            var value = GetSourceValue(textBox);

            SetOriginalValue(textBox, value);
            SetTextWithoutNotification(textBox, FormatValue(value));
        }

        private static void SetTextWithoutNotification(TextBox textBox, string text)
        {
            SetIsInternalUpdate(textBox, true);

            try
            {
                textBox.Text = text ?? string.Empty;
            }
            finally
            {
                SetIsInternalUpdate(textBox, false);
            }
        }

        private static void MoveFocusNext(TextBox textBox)
        {
            var request = new TraversalRequest(FocusNavigationDirection.Next);
            textBox.MoveFocus(request);
        }

        public static void Refresh(DependencyObject root)
        {
            if (root == null)
            {
                return;
            }

            if (root is TextBox textBox && !string.IsNullOrWhiteSpace(GetPropertyName(textBox)))
            {
                RefreshTextBox(textBox);
            }

            var childCount = VisualTreeHelper.GetChildrenCount(root);

            for (var i = 0; i < childCount; i++)
            {
                Refresh(VisualTreeHelper.GetChild(root, i));
            }
        }
    }
}