namespace DataTableListView.Views
{
    using System.ComponentModel;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using DataTableListView.Comparer;
    using DataTableListView.Core;
    using DataTableListView.Repository;

    /// <summary>
    /// Interaktionslogik für EditDetailView.xaml
    /// </summary>
    public partial class EditDetailView : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<string, Func<Result<string>>> ValidationRules;
        private Dictionary<string, string> _ValidationErrors;
        private string _WindowTitel;
        private int _ErrorCount;
        private DataRow _OriginalRow;
        private DataRow _CurrentRow;
        private ICollectionView _AktionSource;

        public EditDetailView(RowNextAction rowAction = RowNextAction.AddRow)
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.ValidationErrors = new Dictionary<string, string>();
            this.ValidationRules = new Dictionary<string, Func<Result<string>>>();
            this.RowAction = rowAction;
            this.WindowTitel = "Neuer Eintrag erstellen";
            this.CurrentRow = null;
        }

        public EditDetailView(DataRow currentRow, RowNextAction rowAction = RowNextAction.UpdateRow)
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.ValidationErrors = new Dictionary<string, string>();
            this.ValidationRules = new Dictionary<string, Func<Result<string>>>();
            this.RowAction = rowAction;
            if (currentRow != null && rowAction == RowNextAction.UpdateRow)
            {
                this.WindowTitel = "Gewählter Eintrag bearbeiten";
                this.OriginalRow = currentRow;
                this.CurrentRow = currentRow;
            }
            else if (currentRow != null && rowAction == RowNextAction.CopyRow)
            {
                this.WindowTitel = "Gewählter Eintrag kopieren";
                this.OriginalRow = currentRow;
                this.CurrentRow = currentRow;
            }
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

        public DataRow OriginalRow
        {
            get { return _OriginalRow; }
            set
            {
                if (this._OriginalRow != value)
                {
                    this._OriginalRow = value;
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

        public Dictionary<string, string> ValidationErrors
        {
            get { return _ValidationErrors; }
            set
            {
                if (this._ValidationErrors != value)
                {
                    this._ValidationErrors = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int ErrorCount
        {
            get { return _ErrorCount; }
            set
            {
                if (this._ErrorCount != value)
                {
                    this._ErrorCount = value;
                    this.OnPropertyChanged();
                }
            }
        }

        private bool? DialogCloseResult { get; set; }
        private RowNextAction RowAction { get; set; }
        private bool IsColumnModified { get; set; }

        #endregion Properties

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            Keyboard.Focus(this);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnCloseDialog, "Click", this.OnCloseDialog);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnSaveRow, "Click", this.OnSaveRow);
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnShowErrors, "Click", this.OnShowErrors);

            this.RegisterValidations();
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

                    if (this.CurrentRow == null && this.RowAction == RowNextAction.AddRow)
                    {
                        this.CurrentRow = repository.NewDataRow();
                        this.CurrentRow.SetField<Guid>("Id", Guid.NewGuid());
                    }
                    else if (this.CurrentRow != null && this.RowAction == RowNextAction.UpdateRow)
                    {
                        this.CurrentRow = this.OriginalRow.Clone<DataRow>(this.OriginalRow.Table);
                    }
                    else if (this.CurrentRow != null && this.RowAction == RowNextAction.CopyRow)
                    {
                        this.CurrentRow = this.OriginalRow.Clone<DataRow>(this.OriginalRow.Table);
                        this.CurrentRow.SetField("Id", Guid.NewGuid());
                    }

                    WeakEventManager<DataTable, DataColumnChangeEventArgs>.AddHandler(this.CurrentRow.Table, "ColumnChanged", this.OnColumnChanged);
                    this.StatusLineA.Text = "Bereit";
                    this.IsColumnModified = false;
                    this.CheckInputControls("Kapitel", "KapitelTitel");
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private void OnColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            this.StatusLineA.Text = "Geändert";
            this.IsColumnModified = true;

            string fieldName = e.Column.ColumnName;

            this.CheckInputControls(fieldName);
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

            if (this.IsColumnModified == true)
            {
                MessageBoxResult msgYN = MessageBox.Show("Es sind noch ungespeicherte Änderungen vorhanden. Soll der Dialog ohne speichern beenden werden?", "Dialog schließen", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgYN == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void OnSaveRow(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.CheckInputControls("Kapitel", "KapitelTitel") == true)
                {
                    return;
                }


                using (DemoDataRepository repository = new DemoDataRepository())
                {
                    if (this.RowAction == RowNextAction.UpdateRow)
                    {
                        if (CompareDataRow.IsEquals(this.CurrentRow, this.OriginalRow) == false)
                        {
                            if (repository.CheckContentWith(this.CurrentRow,1, "Kapitel", "KapitelTitel") == false)
                            {
                                repository.Update(this.CurrentRow);
                                this.IsColumnModified = false;
                                this.DialogCloseResult = true;
                                this.Close();
                            }
                            else
                            {
                                MessageBox.Show($"Die Werte für Kapitel und Thema sind bereits vorhanden, wählen Sie daher andere Werte!", "Eingabeprüfung", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else if (this.RowAction == RowNextAction.CopyRow)
                    {
                        if (repository.CheckContentWith(this.CurrentRow,0, "Kapitel", "KapitelTitel") == false)
                        {
                            repository.Add(this.CurrentRow);
                            this.IsColumnModified = false;
                            this.DialogCloseResult = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Die Werte für Kapitel und Thema sind bereits vorhanden, wählen Sie daher andere Werte!", "Eingabeprüfung", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if (this.RowAction == RowNextAction.AddRow)
                    {
                        if (repository.CheckContentWith(this.CurrentRow, 0, "Kapitel", "KapitelTitel") == false)
                        {
                            repository.Add(this.CurrentRow);
                            this.IsColumnModified = false;
                            this.DialogCloseResult = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Die Werte für Kapitel und Thema sind bereits vorhanden, wählen Sie daher andere Werte!", "Eingabeprüfung", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private bool CheckInputControls(params string[] fieldNames)
        {
            bool result = false;

            try
            {
                foreach (string fieldName in fieldNames)
                {
                    Func<Result<string>> function = null;
                    if (this.ValidationRules.TryGetValue(fieldName, out function) == true)
                    {
                        Result<string> ruleText = this.DoValidation(function, fieldName);
                        if (string.IsNullOrEmpty(ruleText.Value) == false)
                        {
                            if (this.ValidationErrors.ContainsKey(fieldName) == false)
                            {
                                this.ValidationErrors.Add(fieldName, ruleText.Value);
                            }
                        }
                        else
                        {
                            if (this.ValidationErrors.ContainsKey(fieldName) == true)
                            {
                                this.ValidationErrors.Remove(fieldName);
                            }
                        }
                    }
                }

                if (this.ValidationErrors != null && this.ValidationErrors.Count > 0)
                {
                    this.BtnSaveRow.IsEnabled = false;
                    this.BtnShowErrors.Visibility = Visibility.Visible;
                    this.ErrorCount = this.ValidationErrors.Count;
                    result = true;
                }
                else if (this.ValidationErrors != null && this.ValidationErrors.Count == 0)
                {
                    this.BtnSaveRow.IsEnabled = true;
                    this.BtnShowErrors.Visibility = Visibility.Hidden;
                    this.ErrorCount = this.ValidationErrors.Count;
                    result = false;
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }

            return result;
        }

        #region Register Validations
        private void RegisterValidations()
        {
            this.ValidationRules.Add("Kapitel", () =>
            {
                return InputValidation<DataRow>.This(this.CurrentRow).GreaterThanZero("Kapitel","KapitelNr");
            });

            this.ValidationRules.Add("KapitelTitel", () =>
            {
                return InputValidation<DataRow>.This(this.CurrentRow).NotEmpty("KapitelTitel","Thema");
            });
        }

        private Result<string> DoValidation(Func<Result<string>> validationFunc, string propName)
        {
            Result<string> result = validationFunc.Invoke();

            return result;
        }

        private void OnShowErrors(object sender, RoutedEventArgs e)
        {
        }
        #endregion Register Validations

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
