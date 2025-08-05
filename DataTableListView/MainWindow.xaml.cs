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
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Ribbon;
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
        private ICollectionView _ListViewSource;
        private DataRow _CurrentSelectedItem;
        private int _DisplayRowCount;

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
        #endregion Properties


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);

            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnCloseApplication, "Click", this.OnCloseApplication);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnNewRow, "Click", this.OnNewRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnDeleteRow, "Click", this.OnDeleteRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnEditRow, "Click", this.OnEditRow);
            WeakEventManager<MenuItem, RoutedEventArgs>.AddHandler(this.mnuCurrentRow, "Click", this.OnCurrentListViewItemClick);

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
                    this.ListViewSource = repository.Select();
                    if (this.ListViewSource != null)
                    {
                        this.DisplayRowCount = this.ListViewSource.Cast<DataRow>().Count();
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
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private bool DataDefaultFilter(DataRow rowItem)
        {
            bool found = false;

            if (rowItem == null)
            {
                return false;
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
                dr.SetField<int>("Kapitel", 3);
                dr.SetField<string>("KapitelTitel", "Test-Kapitel 3");
                dr.SetField<string>("Titel", "Titelbeschreibung");
                dr.SetField<string>("Beschreibung", string.Empty);
                dr.SetField<decimal>("AufwandMax", 2.5m);
                dr.SetField<decimal>("AufwandMid", 1.5m);
                dr.SetField<decimal>("AufwandMin", 1.0m);
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

            aufwandMax = 3.5M;
            aufwandMid = 3.0M;
            aufwandMin = 2.5M;

            DataRow editRow = this.CurrentSelectedItem.Clone<DataRow>(this.CurrentSelectedItem.Table);
            if (editRow != null)
            {
                using (DemoDataRepository repository = new DemoDataRepository())
                {
                    editRow.SetField<decimal>("AufwandMax", aufwandMax);
                    editRow.SetField<decimal>("AufwandMid", aufwandMid);
                    editRow.SetField<decimal>("AufwandMin", aufwandMin);
                    repository.Update(editRow);
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
                    }

                    this.LoadDataHandler(true);
                }
            }
        }

        private void OnMouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
        }

        private void OnCurrentListViewItemClick(object sender, RoutedEventArgs e)
        {
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