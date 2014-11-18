using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc
{
    public static class FilterOperations
    {
        internal enum FullFilterOperations
        {
            Equal = 1,
            NotEqual = 2,
            GreaterThan = 3,
            GreaterThanOrEqual = 4,
            LessThan = 5,
            LessThanOrEqual = 6,
            Between = 7,
            In = 8,

            StartsWith = 16,
            EndsWith = 17,
            Contains = 18,

            And = 253,
            Or = 254,
            Not = 255
        }

        public enum BinaryFilterOperations
        {
            Equal = FullFilterOperations.Equal,
            NotEqual = FullFilterOperations.NotEqual,
            GreaterThan = FullFilterOperations.GreaterThan,
            GreaterThanOrEqual = FullFilterOperations.GreaterThanOrEqual,
            LessThan = FullFilterOperations.LessThan,
            LessThanOrEqual = FullFilterOperations.LessThanOrEqual,

            /// <summary>
            /// only support string type data
            /// </summary>
            StartsWith = FullFilterOperations.StartsWith,
            /// <summary>
            /// only support string type data
            /// </summary>
            EndsWith = FullFilterOperations.EndsWith,
            /// <summary>
            /// only support string type data
            /// </summary>
            Contains = FullFilterOperations.Contains
        }

        public enum InFilterOperations
        {
            In = FullFilterOperations.In
        }

        public enum ThreeOperations
        {
            Between = FullFilterOperations.Between
        }

        public enum ComposeFilterOperations
        {
            And = FullFilterOperations.And,
            Or = FullFilterOperations.Or,
            Not = FullFilterOperations.Not
        }
    }

}
