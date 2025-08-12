# DataTable, DataRow als Binding-Element für ListView und Dialoge verwenden

![NET](https://img.shields.io/badge/NET-8.0-green.svg)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![VS2022](https://img.shields.io/badge/Visual%20Studio-2022-white.svg)
![Version](https://img.shields.io/badge/Version-1.0.2025.0-yellow.svg)]

In dieser Demo-Programmierung wird der experimentelle Ansatz verfolgt, soweit wie möglich alle Eingabe- und Anzeige Controls über einem DataTable bzw. DataRow mit den notwendigen Daten zu versorgen. Als Datenbank dient eine [SQLite Core Database](https://system.data.sqlite.org/home/doc/trunk/www/index.wiki).</br>
Es wird daher ohne den klassischen Ansatz mit dem MVVM-Pattern gearbeitet. Trotzdem implementieren die einzelnen Windows und UserControls das *INotifyPropertyChanged*-Interface um die Vorteile des *Binding*, z.B. für Asyncrone Operationen oder Live-Aktionen (Suchen, Gruppieren, Sortieren usw.) einfacherer einsetzten zu können.  

Das Beispiel verwendet keine weiteren Pakete, somit ist alles direkt im C# Source vorhanden. 

# Architektur
Der Experimentelle Ansatz verfolgt das Ziel, einfache Desktop Anwendungen ohne zusätzliche Frameworks schnell und einfach erstellen zu können.
Hauptbestandteil der Anwendung sind die Klassen zum Lesen und Schreiben der Daten.
1. Datenbank</br>
    - **SQLiteDBContext**</br>
      Stellt im einfachsten Fall ein Connection-Object zur Verfügung
    - **SqlBuilderContext, SqlBuilderResult**</br>
      Generiert einfache Insert, Update, Delete Anweisungen
    - **DemoDataRepository**</br>
      Fachliche Klasse zum Lesen und Schreiben von Daten
      Pro Tabelle bzw. Dialog wird jeweils ein Daten Repository verwendet.
    - Extension Klasse [**SQLRecordSetExtension**](https://github.com/GerhardAhrens/Console.SQLiteHelper) und **DataRowExtensions**</br>
      Sind ebenfalls zwei allgemeine Klassen um das Arbeiten mit *DataRow* und SQL Anzweisungen zu unterstützen.</b> Eine Beschreibung zur **SQLRecordSetExtension** finden Sie unter diesem [**Link**](https://github.com/GerhardAhrens/Console.SQLiteHelper).

      Während die Klassen *SQLiteDBContext, SqlBuilderContext, SqlBuilderResult* allgeine Klassen sind, werden in der Klasse *DemoDataRepository* die fachlichen Aspekte abgebildet.

2. Darstellung und Eingabe von Daten</br>
   Zum Darstellen von Daten (ListView) bzw. Eingabe (TextBox, ComboBox, usw.) werden WPF Standard-Controls verwendet.

3. Validierung von Eingaben</br>
   Zur Prüfung der Eingaben steht die Klasse *InputValidation* zur Verfügung.

# Datenzugriff
Verwendung des *SQLiteDBContext* für eine Repository-Klasse
```csharp
public class DemoDataRepository : SQLiteDBContext
{
    private const string TABLENAME = "TAB_Aufwand";
    /// <summary>
    /// Initializes a new instance of the <see cref="DemoDataRepository"/> class.
    /// </summary>
    public DemoDataRepository()
    {
        this.DBConnection = base.Connection;
        this.Tablename = TABLENAME;
    }

    public SQLiteConnection DBConnection { get; private set; }

    private string Tablename { get; set; }
}
```

Ermitteln der Anzahl von Datensätze
```csharp
public int Count()
{
    int result = 0;

    try
    {
        result = base.Connection.RecordSet<int>($"select count(*) from {this.Tablename}").Get().Result;
    }
    catch (Exception ex)
    {
        string errorText = ex.Message;
        throw;
    }

    return result;
}
```


Lesen von Daten in den Typ *ICollectionView* zum späteren binden an ein ListView-Control.
```csharp
public ICollectionView Select()
{
    ICollectionView result = null;

    try
    {
        result = base.Connection.RecordSet<ICollectionView>($"SELECT * FROM {this.Tablename}").Get().Result;
    }
    catch (Exception ex)
    {
        string errorText = ex.Message;
        throw;
    }

    return result;
}
```

# Anzeigen im ListView


# Anzeigen im Dialog

