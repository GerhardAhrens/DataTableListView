namespace DataTableListView.Core
{
    using System;
    using System.Data;
    using System.Globalization;

    public class InputValidation<TDataRow> where TDataRow : class
    {
        private static InputValidation<DataRow> validation;

        private DataRow ThisObject { get; set; }

        public static InputValidation<DataRow> This(DataRow thisObject)
        {
            validation = new InputValidation<DataRow>();
            validation.ThisObject = thisObject;
            return validation;
        }

        public Result<string> NotEmpty(string fieldName)
        {
            string result = string.Empty;
            bool resultValidError = false;
            string propertyValue = (string)((DataRow)validation.ThisObject).GetField<string>(fieldName);

            if (string.IsNullOrEmpty(propertyValue) == true)
            {
                result = $"Das Feld '{fieldName}' darf nicht leer sein.";
                resultValidError = true;
            }

            return Result<string>.SuccessResult(result, resultValidError);
        }

        public Result<string> InRange(string fieldName, int min, int max)
        {
            string result = string.Empty;
            bool resultValidError = false;
            object propertyValue = (int)((DataRow)validation.ThisObject).GetField<int>(fieldName);

            if (propertyValue == null)
            {
                return Result<string>.SuccessResult(result, resultValidError);
            }

            if (string.IsNullOrEmpty(propertyValue.ToString()) == false)
            {
                if ((Enumerable.Range(min, max).Contains(Convert.ToInt32(propertyValue, CultureInfo.CurrentCulture))) == false)
                {
                    result = $"Das Feld '{fieldName}' muß zwischen {min} und {max} liegen";
                    resultValidError = true;
                }
            }
            else
            {
                result = $"Das Feld '{fieldName}' darf nicht leer sein.";
                resultValidError = true;
            }

            return Result<string>.SuccessResult(result, resultValidError);
        }

        public Result<string> GreaterThanZero(string fieldName)
        {
            string result = string.Empty;
            bool resultValidError = false;
            object propertyValue = (int)((DataRow)validation.ThisObject).GetField<int>(fieldName);

            if (propertyValue == null)
            {
                return Result<string>.SuccessResult(result, resultValidError);
            }

            if (string.IsNullOrEmpty(propertyValue.ToString()) == false)
            {
                double testDouble;
                if (double.TryParse(propertyValue.ToString(), out testDouble) == true)
                {
                    if (testDouble <= 0)
                    {
                        result = $"Der Feld '{fieldName}' muß größer 0 sein";
                        resultValidError = true;
                    }
                }
                else
                {
                    result = $"Das Feld '{fieldName}' nicht leer sein.";
                    resultValidError = true;
                }
            }
            else
            {
                result = $"Das Feld nicht leer sein.";
                resultValidError = true;
            }

            return Result<string>.SuccessResult(result, resultValidError);
        }

        public Result<string> GreaterOrZero(string fieldName)
        {
            string result = string.Empty;
            bool resultValidError = false;
            object propertyValue = (int)((DataRow)validation.ThisObject).GetField<int>(fieldName);

            if (propertyValue == null)
            {
                return Result<string>.SuccessResult(result, resultValidError);
            }

            if (string.IsNullOrEmpty(propertyValue.ToString()) == false)
            {
                double testDouble;
                if (double.TryParse(propertyValue.ToString(), out testDouble) == true)
                {
                    if (testDouble <= -1)
                    {
                        result = $"Der Feld '{fieldName}' muß größer oder gleich 0 sein";
                        resultValidError = true;
                    }
                }
                else
                {
                    result = $"Das Feld '{fieldName}' nicht leer sein.";
                    resultValidError = true;
                }
            }
            else
            {
                result = $"Das Feld nicht leer sein.";
                resultValidError = true;
            }

            return Result<string>.SuccessResult(result, resultValidError);
        }
    }
}