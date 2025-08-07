/*
 * <copyright file="DemoDataRepository.cs" company="Lifeprojects.de">
 *     Class: DemoDataRepository
 *     Copyright © Lifeprojects.de 2025
 * </copyright>
 *
 * <author>Gerhard Ahrens - Lifeprojects.de</author>
 * <email>gerhard.ahrens@lifeprojects.de</email>
 * <date>05.08.2025 07:49:41</date>
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

namespace DataTableListView.Repository
{
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;

    using DataTableListView.Core;
    using DataTableListView.Generator;

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

        public ICollectionView SelectAktion()
        {
            ICollectionView result = null;

            try
            {
                result = base.Connection.RecordSet<ICollectionView>($"SELECT * FROM TAB_Aktion").Get().Result;
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }

            return result;
        }

        public DataRow NewDataRow()
        {
            DataRow result = null;
            string sqlText = string.Empty;

            try
            {
                result = base.Connection.RecordSet<DataRow>(this.Tablename).New().Result;

            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }

            return result;
        }

        public void Add(DataRow entity)
        {
            try
            {
                using (SqlBuilderContext ctx = new SqlBuilderContext(entity))
                {
                    (string, SQLiteParameter[]) sql = ctx.GetInsert();

                    this.Connection.RecordSet<int>(sql.Item1, sql.Item2).Execute();
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }

        public int Delete(DataRow entity)
        {
            int result = 0;

            try
            {
                using (SqlBuilderContext ctx = new SqlBuilderContext(entity))
                {
                    ctx.CurrentUser = Environment.UserName;
                    (string, SQLiteParameter[]) sql = ctx.GetDelete();

                    result = this.Connection.RecordSet<int>(sql.Item1, sql.Item2).Execute().Result;
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }

            return result;
        }

        public void Update(DataRow entity)
        {
            try
            {
                using (SqlBuilderContext ctx = new SqlBuilderContext(entity))
                {
                    ctx.CurrentUser = Environment.UserName;
                    (string, SQLiteParameter[]) sql = ctx.GetUpdate();

                    this.Connection.RecordSet<int>(sql.Item1, sql.Item2).Execute();
                }
            }
            catch (Exception ex)
            {
                string errorText = ex.Message;
                throw;
            }
        }
    }
}
