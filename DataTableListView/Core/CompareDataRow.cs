//-----------------------------------------------------------------------
// <copyright file="CompareDataRow.cs" company="Lifeprojects.de">
//     Class: CompareDataRow
//     Copyright © Gerhard Ahrens, 2019
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>development@lifeprojects.de</email>
// <date>04.11.2019</date>
//
// <summary>Class for CompareDataRow Result</summary>
// <example>
//  List<CompareResult> changes = CompareObject.CompareDifferences(dr1, dr2);
// </example>
//-----------------------------------------------------------------------

namespace DataTableListView.Comparer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Runtime.Versioning;
    using System.Text.Json;

    [SupportedOSPlatform("windows")]
    //[DebuggerStepThrough]
    [Serializable]
    public static class CompareDataRow
    {

        public static bool IsEquals(DataRow sourceRow, DataRow targetRow)
        {
            bool result = true;

            ArgumentNullException.ThrowIfNull(sourceRow, nameof(sourceRow));
            ArgumentNullException.ThrowIfNull(targetRow, nameof(targetRow));

            try
            {
                for (int i = 0; i < (sourceRow.ItemArray.Length - 1); i++)
                {
                    if (sourceRow.ItemArray[i].Equals(targetRow[i]) == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static bool IsEquals(DataRow sourceRow, DataRow targetRow, params string[] columns)
        {
            bool result = true;

            ArgumentNullException.ThrowIfNull(sourceRow, nameof(sourceRow));
            ArgumentNullException.ThrowIfNull(targetRow, nameof(targetRow));
            ArgumentNullException.ThrowIfNull(columns, nameof(columns));

            try
            {
                for (int i = 0; i < columns.Length; i++)
                {
                    int columnIndex = sourceRow.Table.Columns.IndexOf(columns[i]);
                    if (sourceRow.ItemArray[columnIndex].Equals(targetRow[columnIndex]) == false)
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static bool IsEqualsIgnorFields(DataRow sourceRow, DataRow targetRow, params string[] columns)
        {
            bool result = true;

            ArgumentNullException.ThrowIfNull(sourceRow, nameof(sourceRow));
            ArgumentNullException.ThrowIfNull(targetRow, nameof(targetRow));
            ArgumentNullException.ThrowIfNull(columns, nameof(columns));

            try
            {
                for (int i = 0; i < sourceRow.Table.Columns.Count; i++)
                {
                    string columnName = sourceRow.Table.Columns[i].ColumnName;
                    if (columns.Contains(columnName) == false)
                    {
                        int columnIndex = sourceRow.Table.Columns.IndexOf(columns[i]);
                        if (sourceRow.ItemArray[columnIndex].Equals(targetRow[columnIndex]) == false)
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static List<CompareResult> CompareDifferences(DataRow sourceRow, DataRow compareRow)
        {
            ArgumentNullException.ThrowIfNull(sourceRow,nameof(sourceRow));
            ArgumentNullException.ThrowIfNull(compareRow, nameof(compareRow));

            List<CompareResult> differenceInfoList = new List<CompareResult>();

            for (int i = 0; i < sourceRow.ItemArray.Length; i++)
            {
                object firstValue = sourceRow.ItemArray[i];
                object secondValue = compareRow.ItemArray[i];
                string tableName = sourceRow.Table.TableName;
                string fieldName = sourceRow.Table.Columns[i].ColumnName;
                Type type = sourceRow.Table.Columns[i].DataType;

                if (object.Equals(firstValue, secondValue) == false)
                {
                    differenceInfoList.Add(new CompareResult(tableName, fieldName, type.Name, firstValue, secondValue));
                }

            }

            return differenceInfoList;
        }

        public static List<CompareResult> CompareDifferences(DataRow sourceRow, DataRow compareRow, params string[] ignoreColumn)
        {
            ArgumentNullException.ThrowIfNull(sourceRow, nameof(sourceRow));
            ArgumentNullException.ThrowIfNull(compareRow, nameof(compareRow));

            List<string> ignoreList = null;
            if (ignoreColumn != null)
            {
                ignoreList = new List<string>(ignoreColumn);
            }

            List<CompareResult> differenceInfoList = new List<CompareResult>();

            for (int i = 0; i < sourceRow.ItemArray.Length; i++)
            {
                object firstValue = sourceRow.ItemArray[i];
                object secondValue = compareRow.ItemArray[i];
                string tableName = sourceRow.Table.TableName;
                string fieldName = sourceRow.Table.Columns[i].ColumnName;
                Type type = sourceRow.Table.Columns[i].DataType;

                if (ignoreList == null)
                {
                    if (object.Equals(firstValue, secondValue) == false)
                    {
                        differenceInfoList.Add(new CompareResult(tableName, fieldName, type.Name, firstValue, secondValue));
                    }
                }
                else
                {
                    if (ignoreList.Contains(fieldName) == false)
                    {
                        if (object.Equals(firstValue, secondValue) == false)
                        {
                            differenceInfoList.Add(new CompareResult(tableName, fieldName, type.Name, firstValue, secondValue));
                        }
                    }
                }

            }

            return differenceInfoList;
        }

        public static List<CompareResult> CompareDifferences(DataRow sourceRow, DataRow compareRow, List<string> ignoreColumn)
        {
            ArgumentNullException.ThrowIfNull(sourceRow, nameof(sourceRow));
            ArgumentNullException.ThrowIfNull(compareRow, nameof(compareRow));

            List<CompareResult> differenceInfoList = new List<CompareResult>();

            for (int i = 0; i < sourceRow.ItemArray.Length; i++)
            {
                object firstValue = sourceRow.ItemArray[i];
                object secondValue = compareRow.ItemArray[i];
                string tableName = sourceRow.Table.TableName;
                string fieldName = sourceRow.Table.Columns[i].ColumnName;
                Type type = sourceRow.Table.Columns[i].DataType;

                if (ignoreColumn == null)
                {
                    if (object.Equals(firstValue, secondValue) == false)
                    {
                        differenceInfoList.Add(new CompareResult(tableName, fieldName, type.Name, firstValue, secondValue));
                    }
                }
                else
                {
                    if (ignoreColumn.Contains(fieldName) == false)
                    {
                        if (object.Equals(firstValue, secondValue) == false)
                        {
                            differenceInfoList.Add(new CompareResult(tableName, fieldName, type.Name, firstValue, secondValue));
                        }
                    }
                }

            }

            return differenceInfoList;
        }

        private static bool AreEqual<T>(T rowSource, T rowTarget)
        {
            if (rowSource is decimal)
            {
                decimal propVal1 = Convert.ToDecimal(rowSource, CultureInfo.CurrentCulture);
                decimal propVal2 = Convert.ToDecimal(rowTarget, CultureInfo.CurrentCulture);

                if ((int)(((decimal)propVal1 % 1) * 100) == 0 && (int)(((decimal)propVal2 % 1) * 100) == 0)
                {
                    var obj1Serialized = JsonSerializer.Serialize((Convert.ToInt32(rowSource, CultureInfo.CurrentCulture)));
                    var obj2Serialized = JsonSerializer.Serialize(Convert.ToInt32(rowTarget, CultureInfo.CurrentCulture));

                    return obj1Serialized == obj2Serialized;
                }
                else
                {
                    var obj1Serialized = JsonSerializer.Serialize(rowSource);
                    var obj2Serialized = JsonSerializer.Serialize(rowTarget);

                    return obj1Serialized == obj2Serialized;
                }
            }
            else
            {
                var obj1Serialized = JsonSerializer.Serialize(rowSource);
                var obj2Serialized = JsonSerializer.Serialize(rowTarget);

                return obj1Serialized == obj2Serialized;
            }
        }

    }
}
