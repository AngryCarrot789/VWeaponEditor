using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using VWeaponEditor.Highlighting;
using TextRange = VWeaponEditor.Highlighting.TextRange;

namespace VWeaponEditor.Processes {
    public class SearchResultInlinesConverter : IMultiValueConverter {
        public Style NormalRunStyle { get; set; }
        public Style HighlightedRunStyle { get; set; }

        public Run CreateNormalRun(string text = null) {
            return this.NormalRunStyle != null ? new Run(text) { Style = this.NormalRunStyle } : new Run(text);
        }

        public Run CreateHighlightedRun(string text = null) {
            return this.HighlightedRunStyle != null ? new Run(text) { Style = this.HighlightedRunStyle } : new Run(text) {
                Background = new SolidColorBrush(Colors.Goldenrod),
                Foreground = new SolidColorBrush(Colors.Black)
            };
        }

        public IEnumerable<Run> CreateString(string text, IEnumerable<TextRange> ranges) {
            return InlineHelper.CreateHighlight(text, ranges, this.CreateNormalRun, this.CreateHighlightedRun);
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values == null || values.Length != 2) {
                throw new Exception("Expected 2 values, got " + (values == null ? "null" : values.Length.ToString()));
            }

            if (!(values[0] is string text)) throw new Exception("Expected values[0] to be a string, got " + values[0]);
            if (!(values[1] is IEnumerable<TextRange> ranges)) {
                if (values[1] == null) // allow null enumerable
                    return Enumerable.Empty<Run>();
                throw new Exception("Expected values[1] to be IEnumerable<TextRange>, got " + values[1]);
            }

            return this.CreateString(text, ranges);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}