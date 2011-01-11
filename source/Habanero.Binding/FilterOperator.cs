namespace Habanero.Binding
{
    // Enum to hold filter operators. The chars 
    // are converted to their integer values.
    public enum FilterOperator
    {
        EqualTo = '=',
        LessThan = '<',
        GreaterThan = '>',
        None = ' ',
    }

    /*///<summary>
    /// Filter operators
    ///</summary>
    public enum FilterOperator
    {
        Equal,
        Like,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        TopN,
        BottomN,
        TopPercent,
        BottomPercent,
        In,
        Between,
        Unknown	
        // prior to definition or illegal value
    }
    internal class FilterOperatorDef
    {
        static internal FilterOperator GetStyle(string s)
        {
            FilterOperator rs;

            switch (s)
            {
                case "Equal":
                case "=":
                    rs = FilterOperator.Equal;
                    break;
                case "TopN":
                    rs = FilterOperator.TopN;
                    break;
                case "BottomN":
                    rs = FilterOperator.BottomN;
                    break;
                case "TopPercent":
                    rs = FilterOperator.TopPercent;
                    break;
                case "BottomPercent":
                    rs = FilterOperator.BottomPercent;
                    break;
                case "In":
                    rs = FilterOperator.In;
                    break;
                case "LessThanOrEqual":
                case "<=":
                    rs = FilterOperator.LessThanOrEqual;
                    break;
                case "LessThan":
                case "<":
                    rs = FilterOperator.LessThan;
                    break;
                case "GreaterThanOrEqual":
                case ">=":
                    rs = FilterOperator.GreaterThanOrEqual;
                    break;
                case "GreaterThan":
                case ">":
                    rs = FilterOperator.GreaterThan;
                    break;
                case "NotEqual":
                case "!=":
                    rs = FilterOperator.NotEqual;
                    break;
                case "Between":
                    rs = FilterOperator.Between;
                    break;
                case "Like":
                    rs = FilterOperator.Like;
                    break;
                default:		// user error just force to normal TODO
                    rs = FilterOperator.Unknown;
                    break;
            }
            return rs;
        }
    }*/

}
