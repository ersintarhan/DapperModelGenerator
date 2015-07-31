#region

using System.Windows;
using System.Windows.Controls;
using AurelienRibon.Ui.SyntaxHighlightBox;
using DataMapper.Schema;

#endregion

namespace DataMapper
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            txtConnectionString.Text =
                "Server=host;Database=database;User Id=username;Password=password;";
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var cstr = txtConnectionString.Text;
            var h = ((ComboBoxItem)cmbType.SelectedValue).Content;
            string tip = h.ToString();
            var includeViews = chkViews.IsChecked.GetValueOrDefault();
            var upper = chckBoxUpper.IsChecked;
            Shbox.CurrentHighlighter = HighlighterManager.Instance.Highlighters["CSharp"];
            Shbox.Text = SchemaGenerator.ReadSchema(cstr, includeViews,tip,txtProcedureName.Text,upper.Value);
        }
    }
}