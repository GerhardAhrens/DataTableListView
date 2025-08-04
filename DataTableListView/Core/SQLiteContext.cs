/*
 * <copyright file="SQLiteContext.cs" company="Lifeprojects.de">
 *     Class: SQLiteContext
 *     Copyright © Lifeprojects.de 2025
 * </copyright>
 *
 * <author>Gerhard Ahrens - Lifeprojects.de</author>
 * <email>gerhard.ahrens@lifeprojects.de</email>
 * <date>04.08.2025 20:37:09</date>
 * <Project>CurrentProject</Project>
 *
 * <summary>
 * Beschreibung zur Klasse
 * </summary>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by the Free Software Foundation, 
 * either version 3 of the License, or (at your option) any later version.
 * This program is distributed in the hope that it will be useful,but WITHOUT ANY WARRANTY; 
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.You should have received a copy of the GNU General Public License along with this program. 
 * If not, see <http://www.gnu.org/licenses/>.
*/

namespace DataTableListView.Core
{
    using System;

    using System.Data.SQLite;
    using System.IO;

    public class SQLiteContext : IDisposable
    {
        private bool classIsDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteContext"/> class.
        /// </summary>
        public SQLiteContext(string databaseName)
        {
            if (File.Exists(databaseName) == true)
            {
                this.ConnectString = ConnectStringToText(databaseName);
                CreateConnection(ConnectString);
            }
            else
            {
                throw new FileNotFoundException($"Die Datenbankdatei '{databaseName}' wurde nicht gefunden!");
            }
        }

        public string ConnectString { get; private set; }

        public static SQLiteConnection Connection { get; private set; }

        private static void CreateConnection(string connectionString)
        {
            try
            {
                SQLiteConnection conn = new SQLiteConnection(connectionString);
                if (conn != null && conn.State != System.Data.ConnectionState.Open)
                {
                    Connection = conn;
                }
            }
            catch (SQLiteException ex)
            {
                string errorText = ex.Message;
                throw;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        private static string ConnectStringToText(string databasePath)
        {
            SQLiteConnectionStringBuilder conString = new SQLiteConnectionStringBuilder();
            conString.DataSource = databasePath;
            conString.DefaultTimeout = 30;
            conString.SyncMode = SynchronizationModes.Off;
            conString.JournalMode = SQLiteJournalModeEnum.Memory;
            conString.PageSize = 65536;
            conString.CacheSize = 16777216;
            conString.FailIfMissing = false;
            conString.ReadOnly = false;
            conString.Version = 3;
            conString.UseUTF16Encoding = true;

            return conString.ToString();
        }

        #region Implement Dispose

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool classDisposing = false)
        {
            if (this.classIsDisposed == false)
            {
                if (classDisposing == true)
                {
                    this.Connection = null;
                }
            }

            this.classIsDisposed = true;
        }

        #endregion Implement Dispose
    }
}
