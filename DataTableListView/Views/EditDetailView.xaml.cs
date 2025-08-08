namespace DataTableListView.Views
{
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using DataTableListView.Repository;

    /// <summary>
    /// Interaktionslogik für EditDetailView.xaml
    /// </summary>
    public partial class EditDetailView : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _WindowTitel;
        private DataRow _CurrentRow;
        private ICollectionView _AktionSource;

        public EditDetailView()
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.WindowTitel = "Neuer Eintrag erstellen";
            this.CurrentRow = null;
        }

        public EditDetailView(DataRow currentRow)
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.WindowTitel = "Gewählter Eintrag bearbeiten";
            this.CurrentRow = currentRow;
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

        public DataRow CurrentRow
        {
            get { return _CurrentRow; }
            set
            {
                if (this._CurrentRow != value)
                {
                    this._CurrentRow = value;
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

        private bool? DialogCloseResult { get; set; }
        private bool? RowIsNew { get; set; }

        #endregion Properties

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnCloseDialog, "Click", this.OnCloseDialog);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnSaveRow, "Click", this.OnSaveRow);

            this.DataContext = this;

            this.LoadDataHandler();
        }

        private void LoadDataHandler()
        {
            try
            {
                using (DemoDataRepository repository = new DemoDataRepository())
                {
                    this.AktionSource = repository.SelectAktion();

                    if (this.CurrentRow == null)
                    {
                        this.CurrentRow = repository.NewDataRow();
                        this.CurrentRow.SetField<Guid>("Id", Guid.NewGuid());
                        this.RowIsNew = true;
                    }
                    else
                    {
                        this.RowIsNew = false;
                    }
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void OnCloseDialog(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            DialogResult = this.DialogCloseResult;

            /* prüfen ob alle Kriterien zum verlassen des Dialog erfüllt sind */
            /* wenn nein, e.Cancel = true; */

            /*
            MessageBoxResult msgYN = MessageBox.Show("Es sind nicht alle Kriterien zum verlassen des Dialog erfüllt. Trotzdem ohne speichern beenden?", "Dialog schließen", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgYN == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            */
        }

        private void OnSaveRow(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DemoDataRepository repository = new DemoDataRepository())
                {
                    if (this.RowIsNew == false)
                    {
                        repository.Update(this.CurrentRow);
                    }
                    else
                    {
                        repository.Add(this.CurrentRow);
                    }
                }

                this.DialogCloseResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
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
