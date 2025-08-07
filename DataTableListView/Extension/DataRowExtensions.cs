//-----------------------------------------------------------------------
// <copyright file="DataRowExtensions.cs" company="Lifeprojects.de">
//     Class: DataRowExtensions
//     Copyright © Lifeprojects.de 2020
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>29.09.2020</date>
//
// <summary>Extension Class für DataRow</summary>
//-----------------------------------------------------------------------

namespace System.Data
{
    using System.Globalization;
    using System.Runtime.Versioning;
    using System.Text;

    [SupportedOSPlatform("windows")]
    public static class DataRowExtensions
    {
        /// <summary>
        /// Gibt eine Column von einem DataRow im gewünschten Typ zurück
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="this">Alltuelle DataRow Zeile</param>
        /// <param name="fieldName">Column</param>
        /// <returns>Ergebnis zur angegebenen Column</returns>
        public static TResult GetField<TResult>(this DataRow @this, string fieldName)
        {
            try
            {
                object result = null;
                if (typeof(TResult).Name == "Guid")
                {
                    result = @this[fieldName] == DBNull.Value ? Guid.Empty : new Guid(@this[fieldName].ToString());
                }
                else if (typeof(TResult).IsEnum == true)
                {
                    if (@this[fieldName].GetType() == typeof(int))
                    {
                        result = (TResult)@this[fieldName];
                    }
                    else if (@this[fieldName].GetType() == typeof(string))
                    {
                        result = (TResult)Enum.Parse(typeof(TResult), @this[fieldName].ToString(), true);
                    }
                    else
                    {
                        result = (TResult)Enum.Parse(typeof(TResult), @this[fieldName].ToString(), true);
                    }
                }
                else
                {
                    if (@this != null)
                    {
                        result = @this[fieldName] == DBNull.Value ? default(TResult) : (TResult)Convert.ChangeType(@this[fieldName], typeof(TResult), CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        return default(TResult);
                    }
                }

                return (TResult)result;
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                return default(TResult);
            }
        }

        /// <summary>
        /// Gibt eine Column von einem DataRow im gewünschten Typ zurück, mit der möglichkeit einen Default-Wert anzugeben
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="this">Alltuelle DataRow Zeile</param>
        /// <param name="fieldName">Column</param>
        /// <param name="defaultValue">Default-Wert</param>
        /// <returns>Ergebnis zur angegebenen Column</returns>
        public static TResult GetField<TResult>(this DataRow @this, string fieldName, TResult defaultValue)
        {
            try
            {
                object result = null;
                if (@this[fieldName] != DBNull.Value)
                {
                    if (typeof(TResult).Name == "Guid")
                    {
                        result = @this[fieldName] == DBNull.Value ? Guid.Empty : new Guid(@this[fieldName].ToString());
                    }
                    else if (typeof(TResult).IsEnum == true)
                    {
                        if (@this[fieldName].GetType() == typeof(int))
                        {
                            result = (TResult)@this[fieldName];
                        }
                        else if (@this[fieldName].GetType() == typeof(string))
                        {
                            result = (TResult)Enum.Parse(typeof(TResult), @this[fieldName].ToString(), true);
                        }
                        else
                        {
                            result = (TResult)Enum.Parse(typeof(TResult), @this[fieldName].ToString(), true);
                        }
                    }
                    else
                    {
                        result = @this[fieldName] == DBNull.Value ? default(TResult) : (TResult)Convert.ChangeType(@this[fieldName], typeof(TResult), CultureInfo.InvariantCulture);
                    }

                    return (TResult)result;
                }
                else
                {
                    return defaultValue;
                }
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                return default(TResult);
            }
        }

        public static T Clone<T>(this DataRow @this, DataTable parentTable) where T : DataRow
        {
            T clonedRow = (T)parentTable.NewRow();
            clonedRow.ItemArray = @this.ItemArray;
            return clonedRow;
        }

        public static bool HasColumn(this DataRow @this, string columnName)
        {
            bool result = false;

            int columnFound = @this.Table.Columns.OfType<DataColumn>().ToList().Count(c => c.ColumnName.ToLower() == columnName.ToLower());
            if (columnFound > 0)
            {
                result = true;
            }

            return result;
        }

        public static string ItemArrayToString(this DataRow @this, char separator = ',')
        {
            return string.Join(separator, @this.ItemArray);
        }

        public static string ToString(this DataRow @this, char separator = ',')
        {
            return string.Join(separator, @this.ItemArray.Select(c => c.ToString()).ToArray());
        }

        public static string ToString(this DataRow @this, string columns, char separator = ',')
        {
            StringBuilder sb = new StringBuilder();

            string[] columnList = columns.Split(',');

            foreach (string column in columnList)
            {
                if (@this.HasColumn(column) == true)
                {
                    sb.Append(@this[column].ToString());
                    sb.Append(separator);
                }
            }

            if (sb.ToString().Trim().Length > 0)
            {
                sb.Remove(sb.ToString().Trim().Length - 1, 1);
            }

            return sb.ToString();
        }


        public static bool Equals(this DataRow @this, DataRow secondDataRow)
        {
            bool result = false;

            if (@this.GetType() != typeof(DataRow) || secondDataRow.GetType() != typeof(DataRow))
            {
                return result;
            }

            if (@this.ItemArray.Length != secondDataRow.ItemArray.Length)
            {
                return result;
            }

            DataRowComparer<DataRow> drc = DataRowComparer.Default;
            result = drc.Equals(@this, secondDataRow);

            return result;
        }
    }
}