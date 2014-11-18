using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc
{
    /// <summary>
    /// SearchCriteriaElement serialization identifies
    /// </summary>
    public static class SceIdentifies
    {
        public const string Field = "Fld";
        public const string Function = "Func";
        public const string Constant = "Const";
        public const string ArrayTypeConstant = "ArrConst";
        public const string ComplexTypeConstant = "ComplexConst";

        public const string JObjTypeProp = "_t";

        public const string Field_NameProp = "Name";

        public const string Constant_ValProp = "Val";

        public const string ArrayTypeConstant_ValProp = "Val";
        public const string ArrayTypeConstant_Item = "Item";

        public const string ComplexTypeConstant_ValProp = "Val";
        public const string ComplexTypeConstant_Type = "Type";
        public const string ComplexTypeConstant_QualifiedName = "QualifiedName";

        public const string Function_Type = "Type";
        public const string Function_QualifiedName = "QualifiedName";
    }

    /// <summary>
    /// FilterCriteria serialization identifies
    /// </summary>
    public static class FcIdentifies
    {
        public const string BinaryOpFilterCriteria = "BinOp";
        public const string CompositeFilterCriteria_Not = "Not";
        public const string CompositeFilterCriteria_And = "And";
        public const string CompositeFilterCriteria_Or = "Or";
        public const string InOpFilterCriteria = "InOp";
        public const string RefOpFilterCriteria = "RefOp";
        public const string TripleOpFilterCriteria = "TriOp";

        public const string OpProp = "Op";
        public const string FofProp = "FOF";
        public const string Fof1Prop = "FOF1";
        public const string Fof2Prop = "FOF2";
        public const string ArgProp = "Arg";
        public const string Arg1Prop = "Arg1";
        public const string Arg2Prop = "Arg2";

        public const string JObjTypeProp = "_t";
    }

    public static class PcIdentifies
    {
        public const string JObjTypeProp = "_t";

        public const string PagingCriteria = "Paging";

        public const string CurrentPage = "P";
        public const string PageSize = "Ps";
        public const string TotalSize = "Ts";
        public const string TotalPages = "Tps";
    }

    public static class OcIdentifies
    {
        public const string JObjTypeProp = "_t";

        public const string OrderByCriteria = "OrderBy";

        public const string Field = "Fld";
        public const string Order = "Sort";
        public const string ThenBy = "ThenBy";
    }
}
