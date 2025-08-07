//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="Lifeprojects.de">
//     Class: MainWindow
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>04.08.2025 20:11:52</date>
//
// <summary>
// MainWindow mit Minimalfunktionen
// </summary>
//-----------------------------------------------------------------------

namespace DataTableListView
{
    using System.ComponentModel;
    using System.Data;
    using System.Globalization;
    using System.Net.NetworkInformation;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Ribbon;
    using System.Windows.Data;
    using System.Windows.Input;

    using DataTableListView.Repository;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _WindowTitel;
        private string _FilterDefaultSearch;
        private string _FilterColumnSearch;
        private string _SelectedColumnSearch;
        private ICollectionView _ListViewSource;
        private ICollectionView _AktionSource;
        private DataRow _CurrentSelectedItem;
        private int _DisplayRowCount;
        private string _NotifyMessage;
        private IEnumerable<string> _ColumnsSource;
        private string _SelectedColumnGroup;

        public MainWindow()
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.WindowTitel = "DataTable ListView Demo";
        }

        #region Properties

        public string WindowTitel
        {
            get { return _WindowTitel; }
            set
            {
                if (this._WindowTitel != value)
                {
                    this._WindowTitel = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string NotifyMessage
        {
            get { return _NotifyMessage; }
            set
            {
                if (this._NotifyMessage != value)
                {
                    this._NotifyMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string FilterDefaultSearch
        {
            get { return _FilterDefaultSearch; }
            set
            {
                if (this._FilterDefaultSearch != value)
                {
                    this._FilterDefaultSearch = value;
                    this.OnPropertyChanged();
                    this.RefreshDefaultFilter();
                }
            }
        }

        public string FilterColumnSearch
        {
            get { return _FilterColumnSearch; }
            set
            {
                if (this._FilterColumnSearch != value)
                {
                    this._FilterColumnSearch = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICollectionView ListViewSource
        {
            get { return _ListViewSource; }
            set
            {
                if (this._ListViewSource != value)
                {
                    this._ListViewSource = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public DataRow CurrentSelectedItem
        {
            get { return _CurrentSelectedItem; }
            set
            {
                if (this._CurrentSelectedItem != value)
                {
                    this._CurrentSelectedItem = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int DisplayRowCount
        {
            get { return _DisplayRowCount; }
            set
            {
                if (this._DisplayRowCount != value)
                {
                    this._DisplayRowCount = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ICollectionView AktionSource
        {
            get { return _AktionSource; }
            set
            {
                if (this._AktionSource != value)
                {
                    this._AktionSource = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public IEnumerable<string> ColumnsSource
        {
            get { return this._ColumnsSource; }
            set
            {
                if (this._ColumnsSource != value)
                {
                    this._ColumnsSource = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string SelectedColumnGroup
        {
            get { return _SelectedColumnGroup; }
            set
            {
                if (this._SelectedColumnGroup != value)
                {
                    this._SelectedColumnGroup = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string SelectedColumnSearch
        {
            get { return _SelectedColumnSearch; }
            set
            {
                if (this._SelectedColumnSearch != value)
                {
                    this._SelectedColumnSearch = value;
                    this.OnPropertyChanged();
                }
            }
        }

        #endregion Properties


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);

            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnCloseApplication, "Click", this.OnCloseApplication);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnNewRow, "Click", this.OnNewRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnDeleteRow, "Click", this.OnDeleteRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnEditRow, "Click", this.OnEditRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnSaveRow, "Click", this.OnSaveRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnUndoRow, "Click", this.OnUndoRow);
            WeakEventManager<MenuItem, RoutedEventArgs>.AddHandler(this.mnuCurrentRow, "Click", this.OnEditRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnCreateGroup, "Click", this.OnCreateGroup);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnClearGroup, "Click", this.OnCreateGroup);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnCreateSearch, "Click", this.OnCreateSearch);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnClearSearch, "Click", this.OnCreateSearch);

            this.DataContext = this;

            this.LoadDataHandler();
        }


        private void OnCloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            MessageBoxResult msgYN = MessageBox.Show("Wollen Sie die Anwendung beenden?", "Beenden", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgYN == MessageBoxResult.Yes)
            {
                App.ApplicationExit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void LoadDataHandler(bool isRefresh = false, int currentPos = 0)
        {
            try
            {
                using (DemoDataRepository repository = new DemoDataRepository())
                {
                    this.DisplayRowCount = repository.Count();

                    this.AktionSource = repository.SelectAktion();

                    this.ListViewSource = repository.Select();
                    if (this.ListViewSource != null)
                    {
                        this.ColumnsSource = this.ListViewSource.Cast<DataRow>().First().Table.Columns.Cast<DataColumn>().Select(s => s.ColumnName).ToList();
                        this.DisplayRowCount = this.ListViewSource.Cast<DataRow>().Count();
                        if (this.DisplayRowCount > 0)
                        {
                            this.ListViewSource.Filter = rowItem => this.DataDefaultFilter(rowItem as DataRow);
                            if (isRefresh == false)
                            {
                                this.ListViewSource.MoveCurrentToPosition(currentPos);
                                this.CurrentSelectedItem = (DataRow)this.ListViewSource.CurrentItem;
                            }
                            else
                            {
                                this.ListViewSource.MoveCurrentToFirst();
                                this.CurrentSelectedItem = this.ListViewSource.Cast<DataRow>().First();
                            }

                            WeakEventManager<DataTable, DataRowChangeEventArgs>.RemoveHandler(this.CurrentSelectedItem.Table, "RowChanged", this.OnRowChanged);
                            WeakEventManager<DataTable, DataRowChangeEventArgs>.AddHandler(this.CurrentSelectedItem.Table, "RowChanged", this.OnRowChanged);
                            this.CurrentSelectedItem.Table.AcceptChanges();
                        }

                        if (this.DisplayRowCount == 0)
                        {
                            this.NotifyMessage = $"Bereit: Kein Datensatz";
                        }
                        else if (this.DisplayRowCount == 1)
                        {
                            this.NotifyMessage = $"Bereit: {this.DisplayRowCount} Datensatz";
                        }
                        else if (this.DisplayRowCount > 1)
                        {
                            this.NotifyMessage = $"Bereit: {this.DisplayRowCount} Datensätze";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void OnListViewHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader currentHeader = e.OriginalSource as GridViewColumnHeader;
            if (currentHeader != null && currentHeader.Role != GridViewColumnHeaderRole.Padding)
            {
                using (this.ListViewSource.DeferRefresh())
                {
                    string headerName = ((Binding)currentHeader.Column.DisplayMemberBinding).Path.Path;
                    Func<SortDescription, bool> lamda = item => item.PropertyName.Equals(headerName, StringComparison.Ordinal);
                    if (this.ListViewSource.SortDescriptions.Any(lamda) == true)
                    {
                        SortDescription currentSortDescription = this.ListViewSource.SortDescriptions.First(lamda);
                        ListSortDirection sortDescription = currentSortDescription.Direction == ListSortDirection.Ascending
                            ? ListSortDirection.Descending : ListSortDirection.Ascending;

                        currentHeader.Column.HeaderTemplate = currentSortDescription.Direction == ListSortDirection.Ascending ?
                            this.Resources["HeaderTemplateArrowDown"] as DataTemplate : this.Resources["HeaderTemplateArrowUp"] as DataTemplate;

                        this.ListViewSource.SortDescriptions.Remove(currentSortDescription);
                        this.ListViewSource.SortDescriptions.Insert(0, new SortDescription(headerName, sortDescription));
                    }
                    else
                    {
                        this.ListViewSource.SortDescriptions.Add(new SortDescription(headerName, ListSortDirection.Ascending));
                    }
                }
            }
        }

        private void OnRowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row.Table.GetChanges(DataRowState.Modified) == null)
            {
                return;
            }

            int modifiedCount = e.Row.Table.GetChanges(DataRowState.Modified).Rows.Count;
            if (modifiedCount == 0)
            {
                this.NotifyMessage = $"Bereit: Kein Datensatz geändert";
            }
            else if (modifiedCount == 1)
            {
                this.NotifyMessage = $"Bereit: {modifiedCount} Datensatz geändert";
            }
            else if (modifiedCount > 1)
            {
                this.NotifyMessage = $"Bereit: {modifiedCount} Datensätze geändert";
            }
        }

        private bool DataDefaultFilter(DataRow rowItem)
        {
            bool found = false;

            if (rowItem == null)
            {
                return false;
            }

            string textFilterString = (this.FilterDefaultSearch ?? string.Empty).ToUpper(CultureInfo.CurrentCulture);
            if (string.IsNullOrEmpty(textFilterString) == false)
            {
                string fullRow = rowItem.ToString("KapitelTitel,Titel,Beschreibung");
                if (string.IsNullOrEmpty(fullRow) == true)
                {
                    return true;
                }

                string[] words = textFilterString.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words.AsParallel<string>())
                {
                    found = fullRow.Contains(word, StringComparison.CurrentCultureIgnoreCase);

                    if (found == false)
                    {
                        return false;
                    }
                }
            }
            else
            {
                found = true;
            }

            return found;
        }

        private void RefreshDefaultFilter()
        {
            if (this.ListViewSource != null)
            {
                this.ListViewSource.Refresh();
                this.DisplayRowCount = this.ListViewSource.Cast<DataRow>().Count();
                this.ListViewSource.MoveCurrentToFirst();
            }
        }

        private void OnNewRow(object sender, RoutedEventArgs e)
        {
            using (DemoDataRepository repository = new DemoDataRepository())
            {
                DataRow dr = repository.NewDataRow();
                dr.SetField<Guid>("Id", Guid.NewGuid());
                dr.SetField<int>("Kapitel", 4);
                dr.SetField<string>("KapitelTitel", "Test-Kapitel 4");
                dr.SetField<string>("Titel", "Titelbeschreibung-4");
                dr.SetField<string>("Beschreibung", string.Empty);
                dr.SetField<decimal>("AufwandMax", 4.5m);
                dr.SetField<decimal>("AufwandMid", 4.5m);
                dr.SetField<decimal>("AufwandMin", 4.0m);
                dr.SetField<bool>("Aktiv", true);
                repository.Add(dr);
                MessageBox.Show("Neuer Datensatz in Tabelle übernommen.", "Speichern", MessageBoxButton.OK, MessageBoxImage.Information);

                this.LoadDataHandler(true);
            }
        }

        private void OnEditRow(object sender, RoutedEventArgs e)
        {
            Guid id = this.CurrentSelectedItem.GetField<Guid>("Id");
            int kapitel = this.CurrentSelectedItem.GetField<int>("Kapitel");
            string kapitelTitel = this.CurrentSelectedItem.GetField<string>("KapitelTitel");
            string titel = this.CurrentSelectedItem.GetField<string>("Titel");
            string beschreibung = this.CurrentSelectedItem.GetField<string>("Beschreibung");
            decimal aufwandMax = this.CurrentSelectedItem.GetField<decimal>("AufwandMax");
            decimal aufwandMid = this.CurrentSelectedItem.GetField<decimal>("AufwandMid");
            decimal aufwandMin = this.CurrentSelectedItem.GetField<decimal>("AufwandMin");
            bool aktiv = this.CurrentSelectedItem.GetField<bool>("Aktiv");

            aufwandMax = 3.5M;
            aufwandMid = 3.0M;
            aufwandMin = 2.5M;

            DataRow editRow = this.CurrentSelectedItem.Clone<DataRow>(this.CurrentSelectedItem.Table);
            if (editRow != null)
            {
                using (DemoDataRepository repository = new DemoDataRepository())
                {
                    editRow.BeginEdit();
                    editRow.SetField<decimal>("AufwandMax", aufwandMax);
                    editRow.SetField<decimal>("AufwandMid", aufwandMid);
                    editRow.SetField<decimal>("AufwandMin", aufwandMin);
                    repository.Update(editRow);
                    editRow.EndEdit();
                    editRow.AcceptChanges();
                }
            }

            int rowPos = this.ListViewSource.CurrentPosition;
            this.LoadDataHandler(false, rowPos);
        }

        private void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            if (this.CurrentSelectedItem != null)
            {
                MessageBoxResult msgYN = MessageBox.Show("Wollen Sie den gewählten Datensatz löschen?", "Löschen Eintrag", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgYN == MessageBoxResult.Yes)
                {
                    using (DemoDataRepository repository = new DemoDataRepository())
                    {
                        repository.Delete(this.CurrentSelectedItem);
                        this.CurrentSelectedItem.Table.AcceptChanges();
                    }

                    this.LoadDataHandler(true);
                }
            }
        }

        private void OnMouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Aufrufen eines Detail Dialog zur Bearbeitung","Eintrag bearbeiten",MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnCurrentListViewItemClick(object sender, RoutedEventArgs e)
        {
        }

        private void OnSaveRow(object sender, RoutedEventArgs e)
        {
            Guid id = this.CurrentSelectedItem.GetField<Guid>("Id");
            int kapitel = this.CurrentSelectedItem.GetField<int>("Kapitel");
            string kapitelTitel = this.CurrentSelectedItem.GetField<string>("KapitelTitel");
            string titel = this.CurrentSelectedItem.GetField<string>("Titel");
            string beschreibung = this.CurrentSelectedItem.GetField<string>("Beschreibung");
            decimal aufwandMax = this.CurrentSelectedItem.GetField<decimal>("AufwandMax");
            decimal aufwandMid = this.CurrentSelectedItem.GetField<decimal>("AufwandMid");
            decimal aufwandMin = this.CurrentSelectedItem.GetField<decimal>("AufwandMin");
            bool aktiv = this.CurrentSelectedItem.GetField<bool>("Aktiv");
            int aktionId = this.CurrentSelectedItem.GetField<int>("AktionId");

            DataTable modifiedTables = this.CurrentSelectedItem.Table.GetChanges(DataRowState.Modified);
            if (modifiedTables != null)
            {
                string msgText = $"Anzahl geänderte Datensätze: {modifiedTables.Rows.Count}";
                MessageBox.Show(msgText, "Speichern", MessageBoxButton.OK, MessageBoxImage.Information);



                this.LoadDataHandler(true);
            }
        }

        private void OnUndoRow(object sender, RoutedEventArgs e)
        {
            if (this.CurrentSelectedItem != null)
            {
                this.CurrentSelectedItem.Table.RejectChanges();
                this.LoadDataHandler(true);
            }
        }

        private void OnCreateGroup(object sender, RoutedEventArgs e)
        {
            if ((Button)sender != null && ((Button)sender).Name == "BtnCreateGroup")
            {
                this.ListViewSource.GroupDescriptions.Clear();
                if (string.IsNullOrEmpty(this.SelectedColumnGroup) == false)
                {
                    this.ListViewSource.GroupDescriptions.Add(new PropertyGroupDescription($"[{this.SelectedColumnGroup}]"));
                    this.ListViewSource.Refresh();
                }
            }
            else if ((Button)sender != null && ((Button)sender).Name == "BtnClearGroup")
            {
                this.SelectedColumnGroup = string.Empty;
                this.ListViewSource.GroupDescriptions.Clear();
                this.ListViewSource.Refresh();
            }
        }

        private void OnCreateSearch(object sender, RoutedEventArgs e)
        {
            if ((Button)sender != null && ((Button)sender).Name == "BtnCreateSearch")
            {
                this.ListViewSource.Filter = rowItem =>
                {
                    DataRow row = rowItem as DataRow;
                    if (row == null)
                    {
                        return false;
                    }

                    string rowContent = row[this.SelectedColumnSearch].ToString().ToLower(CultureInfo.CurrentCulture);

                    return rowContent.Contains(this.FilterColumnSearch.ToLower(CultureInfo.CurrentCulture),StringComparison.CurrentCultureIgnoreCase) == true;
                };

                this.ListViewSource.Refresh();
                this.DisplayRowCount = this.ListViewSource.Cast<DataRow>().Count();

                if (this.DisplayRowCount == 0)
                {
                    this.NotifyMessage = $"Bereit: Kein Datensatz";
                }
                else if (this.DisplayRowCount == 1)
                {
                    this.NotifyMessage = $"Bereit: {this.DisplayRowCount} Datensatz";
                }
                else if (this.DisplayRowCount > 1)
                {
                    this.NotifyMessage = $"Bereit: {this.DisplayRowCount} Datensätze";
                }
            }
            else if ((Button)sender != null && ((Button)sender).Name == "BtnClearSearch")
            {
                this.SelectedColumnSearch = string.Empty;
                this.FilterColumnSearch = string.Empty;
                this.LoadDataHandler(true);
            }
        }

        #region INotifyPropertyChanged implementierung
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }
        #endregion INotifyPropertyChanged implementierung
    }
}