

module QueryDesc {

    export enum SortTypes { Asc = 1, Desc = 2 };

    export class Paging {
        P: number; /* Current Page */
        Ps: number;  /* Page Size */
        Ts: number;  /* Total Size */
        Tps: number;  /* Total Pages */

        constructor(p: number, ps: number, ts: number, tps: number) {
            this.P = p;
            this.Ps = ps;
            this.Ts = ts;
            this.Tps = tps;
        }
    };

    export class OrderBy {
        Fld: string;  /* Field */
        Sort: SortTypes;  /* Asc or Desc */
        ThenBy: OrderBy;

        constructor(fld: string, orderby: SortTypes, thenby: OrderBy) {
            this.Fld = fld;
            this.Sort = orderby;
            this.ThenBy = thenby;
        }
    };

    export interface ISearchCriteriaElement {
        _t: string;
    };

    export interface IFieldOrFunction extends ISearchCriteriaElement {
    };

    export interface IConstantOrFunction extends ISearchCriteriaElement {
    };

    export class Field implements IFieldOrFunction {
        _t: string;
        Name: string;
        constructor(name: string) {
            this._t = "Fld";
            this.Name = name;
        }
    };

    export class Constant implements IConstantOrFunction {
        _t: string;
        Val: string;
        constructor(val: string) {
            this._t = "Const";
            this.Val = val;
        }
    };

    export class ArrayConstant implements IConstantOrFunction {
        _t: string;
        Val: Array<string>;
        constructor(val: Array<string>) {
            this._t = "ArrConst";
            this.Val = val;
        }
    };

    export interface IFunction extends IFieldOrFunction, IConstantOrFunction {
    };

    export interface IFilterCriteria extends IFunction {
    };

    export class BinaryOpFilter implements IFilterCriteria {
        _t: string;
        FOF: IFieldOrFunction;
        Arg: IConstantOrFunction;
        Op: Operators;
        constructor(fof: IFieldOrFunction, arg: IConstantOrFunction, op: Operators) {
            this._t = "BinOp";
            this.FOF = fof;
            this.Arg = arg;
            this.Op = op;
        }
    };

    export class RefOpFilter implements IFilterCriteria {
        _t: string;
        FOF1: IFieldOrFunction;
        FOF2: IFieldOrFunction;
        Op: Operators;
        constructor(fof1: IFieldOrFunction, fof2: IConstantOrFunction, op: Operators) {
            this._t = "RefOp";
            this.FOF1 = fof1;
            this.FOF2 = fof2;
            this.Op = op;
        }
    };

    export class InOpFilter implements IFilterCriteria {
        _t: string;
        FOF: IFieldOrFunction;
        Arg: ArrayConstant;
        constructor(fof: IFieldOrFunction, arg: ArrayConstant) {
            this._t = "InOp";
            this.FOF = fof;
            this.Arg = arg;
        }
    };

    export class TriOpFilter implements IFilterCriteria {
        _t: string;
        FOF: IFieldOrFunction;
        Arg1: IConstantOrFunction;
        Arg2: IConstantOrFunction;
        constructor(fof: IFieldOrFunction, arg1: IConstantOrFunction, arg2: IConstantOrFunction) {
            this._t = "TriOp";
            this.FOF = fof;
            this.Arg1 = arg1;
            this.Arg2 = arg2;
        }
    };

    export class AndOpFilter implements IFilterCriteria {
        _t: string;
        Arg1: IFilterCriteria;
        Arg2: IFilterCriteria;
        constructor(arg1: IFilterCriteria, arg2: IFilterCriteria) {
            this._t = "And";
            this.Arg1 = arg1;
            this.Arg2 = arg2;
        }
    };

    export class OrOpFilter implements IFilterCriteria {
        _t: string;
        Arg1: IFilterCriteria;
        Arg2: IFilterCriteria;
        constructor(arg1: IFilterCriteria, arg2: IFilterCriteria) {
            this._t = "Or";
            this.Arg1 = arg1;
            this.Arg2 = arg2;
        }
    };

    export class NotOpFilter implements IFilterCriteria {
        _t: string;
        Arg: IFilterCriteria;
        constructor(arg: IFilterCriteria) {
            this._t = "Not";
            this.Arg = arg;
        }
    };

    export enum Operators {
        Equal = 1,
        NotEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        LessThan = 5,
        LessThanOrEqual = 6,
        StartsWith = 16,
        EndsWith = 17,
        Contains = 18
    };

    export interface IQueryDesc {
        Filter?: IFilterCriteria;
        OrderBy?: OrderBy;
        Paging?: Paging;
    };

    export function buildQuery(queryDesc: IQueryDesc) {
        var desc = { Filter: null, OrderBy: null, Paging: null };
        if (queryDesc.Filter)
            desc.Filter = queryDesc.Filter;
        if (queryDesc.OrderBy)
            desc.OrderBy = queryDesc.OrderBy;
        if (queryDesc.Paging)
            desc.Paging = queryDesc.Paging;
        return desc;
    };

    export function and(arg1: IFilterCriteria, arg2: IFieldOrFunction) {
        return new AndOpFilter(arg1, arg2);
    };

    export function or(arg1: IFilterCriteria, arg2: IFieldOrFunction) {
        return new OrOpFilter(arg1, arg2);
    };

    export function not(arg: IFilterCriteria) {
        return new NotOpFilter(arg);
    };

    export function binFilter(field: string, op: Operators, val: string) {
        return new BinaryOpFilter(
            new Field(field),
            new Constant(val),
            op);
    };

    export function refFilter(field1: string, op: Operators, field2: string) {
        return new RefOpFilter(
            new Field(field1),
            new Field(field2),
            op);
    };

    export function inFilter(field: string, val: Array<string>) {
        return new InOpFilter(new Field(field), new ArrayConstant(val));
    };

    export function betweenFilter(field: string, val1: string, val2: string) {
        return new TriOpFilter(new Field(field), new Constant(val1), new Constant(val2));
    };

    export function orderBy(field: string, asc: boolean, thenby: OrderBy) {
        if (asc === undefined || asc === null || asc)
            return new OrderBy(field, SortTypes.Asc, thenby);
        else
            return new OrderBy(field, SortTypes.Desc, thenby);
    };

    export function paging(page: number, pageSize: number) {
        return new Paging(page, pageSize, 0, 0);
    };
};

//QueryDesc.buildQuery({
//    Filter: new QueryDesc.AndOpFilter(
//        new QueryDesc.BinaryOpFilter(
//            new QueryDesc.Field("Name"),
//            new QueryDesc.Constant("Eric"),
//            QueryDesc.Operators.StartsWith),
//        new QueryDesc.InOpFilter(
//            new QueryDesc.Field("Type"),
//            new QueryDesc.ArrayConstant(["1", "2"])))
//});

//QueryDesc.buildQuery({
//    Filter: QueryDesc.and(
//        QueryDesc.binFilter("Name", QueryDesc.Operators.StartsWith, "Eric"),
//        QueryDesc.inFilter("Type", ["1", "2"])),
//    Paging: QueryDesc.paging(1, 10)
//});