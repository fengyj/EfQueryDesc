using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.Utils
{
    public static class ExpressionUtils
    {
        private static object lockObj = new object();
        private static ConcurrentDictionary<Type, ConcurrentDictionary<string, LambdaExpression>> propertyExps
            = new ConcurrentDictionary<Type, ConcurrentDictionary<string, LambdaExpression>>();

        /// <summary>
        /// return a string represent the path of a property chain
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="propertyExp"></param>
        /// <returns></returns>
        /// <remarks>not supports indexed property</remarks>
        /// <example>
        /// <![CDATA[
        /// GetPropertyPath<Post, string>(post => post.Blog.Author.Name) 
        /// >> Blog.Author.Name
        /// ]]>
        /// </example>
        public static string GetPropertyPath<TInput, TResult>(Expression<Func<TInput, TResult>> propertyExp)
        {
            Expression exp = propertyExp.Body;
            List<string> propNames = new List<string>();
            while (exp is MemberExpression)
            {
                var memberExp = exp as MemberExpression;
                propNames.Add(memberExp.Member.Name);
                exp = memberExp.Expression;
            }
            return string.Join(".", propNames.Reverse<string>().ToArray());
        }

        /// <summary>
        /// return a lambda expression base on the obj type and property path
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        /// <remarks>not supports indexed property</remarks>
        /// <example>
        /// <![CDATA[
        /// GetPropExp<Post>("Blog.Author.Name")
        /// >> post => post.Blog.Author.Name
        /// ]]>
        /// </example>
        public static LambdaExpression GetPropertyExp<TObj>(string propertyPath)
        {
            return GetPropertyExp(typeof(TObj), propertyPath);
        }  
        
        /// <summary>
        /// return a lambda expression base on the obj type and property path
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        /// <remarks>not supports indexed property</remarks>
        /// <example>
        /// <![CDATA[
        /// GetPropExp<Post>("Blog.Author.Name")
        /// >> post => post.Blog.Author.Name
        /// ]]>
        /// </example>
        public static LambdaExpression GetPropertyExp(Type type, string propertyPath)
        {
            LambdaExpression lambdaExp = null;

            if (propertyExps.ContainsKey(type) && propertyExps[type].ContainsKey(propertyPath))
            {
                lambdaExp = propertyExps[type][propertyPath];
            }
            else
            {
                lock (lockObj)
                {
                    ConcurrentDictionary<string, LambdaExpression> propDict = null;
                    if (propertyExps.ContainsKey(type))
                        propDict = propertyExps[type];
                    else
                        propDict = new ConcurrentDictionary<string, LambdaExpression>();

                    if (!propDict.ContainsKey(propertyPath))
                    {
                        var paramExp = Expression.Parameter(type);
                        var props = propertyPath.Split('.');

                        Expression propExp = paramExp;
                        var objType = type;
                        foreach (var p in props)
                        {
                            var propInfo = objType.GetProperty(p);
                            propExp = Expression.Property(propExp, propInfo);
                            objType = propInfo.PropertyType;
                        }
                        lambdaExp = Expression.Lambda(
                            propExp,
                            paramExp);

                        propDict.TryAdd(propertyPath, lambdaExp);
                    }
                    else
                    {
                        lambdaExp = propDict[propertyPath];
                    }

                    if (!propertyExps.ContainsKey(type))
                        propertyExps.TryAdd(type, propDict);
                }
            }
            // for outside, we don't know how the expression will be used, 
            // so for safety, return a brand new object
            return ClonePropertyExp(lambdaExp);
        }

        private static LambdaExpression ClonePropertyExp(LambdaExpression lambdaExp)
        {
            Stack<PropertyInfo> stack = new Stack<PropertyInfo>();
            Expression exp = lambdaExp.Body;
            while (exp is MemberExpression)
            {
                var mExp = exp as MemberExpression;
                exp = mExp.Expression;
                stack.Push(mExp.Member as PropertyInfo);
            }
            var paramExp = Expression.Parameter(lambdaExp.Parameters[0].Type);
            exp = paramExp;
            while (stack.Count > 0)
            {
                var memberInfo = stack.Pop();
                exp = Expression.Property(exp, memberInfo);
            }
            return Expression.Lambda(exp, paramExp);
        }

        /// <summary>
        /// return a lambda expression base on the obj type and property path
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        /// <remarks>not supports indexed property</remarks>
        /// <example>
        /// <![CDATA[
        /// GetPropExp<Post>("Blog.Author.Name")
        /// >> post => post.Blog.Author.Name
        /// ]]>
        /// </example>
        public static Expression<Func<TObj, TProperty>> GetPropertyExp<TObj, TProperty>(string propertyPath)
        {
            return GetPropertyExp(typeof(TObj), propertyPath) as Expression<Func<TObj, TProperty>>;
        }

        /// <summary>
        /// convert a string to a ConstantExpression by specified type
        /// </summary>
        /// <param name="val"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ConstantExpression GetConstantExpFromString(string val, Type type)
        {
            return ConstantExpHelper.GetConstantExp(val, type);
        }

        /// <summary>
        /// it provides the converters from a string to object for common struct types, 
        /// and you can call AddParser function to add converts for other types
        /// </summary>
        public static class ConstantExpHelper
        {
            private static object lockObj = new object();
            private static Dictionary<Type, Func<string, ConstantExpression>> dict;
            private static Dictionary<Type, Func<string[], ConstantExpression>> arrDict;

            static ConstantExpHelper()
            {
                dict = new Dictionary<Type, Func<string, ConstantExpression>>();
                arrDict = new Dictionary<Type, Func<string[], ConstantExpression>>();

                dict.Add(typeof(bool), GetExp(bool.Parse));
                dict.Add(typeof(bool?), GetExpForNullable(bool.Parse));
                dict.Add(typeof(byte), GetExp(byte.Parse));
                dict.Add(typeof(byte?), GetExpForNullable(byte.Parse));
                dict.Add(typeof(DateTime), GetExp(DateTime.Parse));
                dict.Add(typeof(DateTime?), GetExpForNullable(DateTime.Parse));
                dict.Add(typeof(DateTimeOffset), GetExp(DateTimeOffset.Parse));
                dict.Add(typeof(DateTimeOffset?), GetExpForNullable(DateTimeOffset.Parse));
                dict.Add(typeof(decimal), GetExp(decimal.Parse));
                dict.Add(typeof(decimal?), GetExpForNullable(decimal.Parse));
                dict.Add(typeof(double), GetExp(double.Parse));
                dict.Add(typeof(double?), GetExpForNullable(double.Parse));
                dict.Add(typeof(Guid), GetExp(Guid.Parse));
                dict.Add(typeof(Guid?), GetExpForNullable(Guid.Parse));
                dict.Add(typeof(short), GetExp(short.Parse));
                dict.Add(typeof(short?), GetExpForNullable(short.Parse));
                dict.Add(typeof(int), GetExp(int.Parse));
                dict.Add(typeof(int?), GetExpForNullable(int.Parse));
                dict.Add(typeof(long), GetExp(long.Parse));
                dict.Add(typeof(long?), GetExpForNullable(long.Parse));
                dict.Add(typeof(sbyte), GetExp(sbyte.Parse));
                dict.Add(typeof(sbyte?), GetExpForNullable(sbyte.Parse));
                dict.Add(typeof(float), GetExp(float.Parse));
                dict.Add(typeof(float?), GetExpForNullable(float.Parse));
                dict.Add(typeof(TimeSpan), GetExp(TimeSpan.Parse));
                dict.Add(typeof(TimeSpan?), GetExpForNullable(TimeSpan.Parse));
                dict.Add(typeof(string), GetExpForStringType());
                dict.Add(typeof(char[]), GetExpForArrayOfCharType());

                arrDict.Add(typeof(bool), GetArrayExp(bool.Parse));
                arrDict.Add(typeof(byte), GetArrayExp(byte.Parse));
                arrDict.Add(typeof(DateTime), GetArrayExp(DateTime.Parse));
                arrDict.Add(typeof(DateTimeOffset), GetArrayExp(DateTimeOffset.Parse));
                arrDict.Add(typeof(decimal), GetArrayExp(decimal.Parse));
                arrDict.Add(typeof(double), GetArrayExp(double.Parse));
                arrDict.Add(typeof(Guid), GetArrayExp(Guid.Parse));
                arrDict.Add(typeof(short), GetArrayExp(short.Parse));
                arrDict.Add(typeof(int), GetArrayExp(int.Parse));
                arrDict.Add(typeof(long), GetArrayExp(long.Parse));
                arrDict.Add(typeof(sbyte), GetArrayExp(sbyte.Parse));
                arrDict.Add(typeof(float), GetArrayExp(float.Parse));
                arrDict.Add(typeof(TimeSpan), GetArrayExp(TimeSpan.Parse));
                arrDict.Add(typeof(string), GetArrayExpForStringType());
            }

            internal static ConstantExpression GetConstantExp(string val, Type type)
            {
                if (type.IsEnum)
                    return GetExpForEnumType(type, val);
                else
                    return dict[type](val);
            }

            internal static ConstantExpression GetConstantExp(string[] val, Type type)
            {
                if (type.IsEnum)
                    return GetExpForEnumType(type, val);
                else
                    return arrDict[type](val);
            }

            /// <summary>
            /// add a parser function for specified type to convert the string to the object
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="parser"></param>
            public static void AddParser<T>(Func<string, T> parser)
            {
                var type = typeof(T);
                lock (lockObj)
                {
                    if (dict.ContainsKey(type)) return;
                    dict.Add(type, val => Expression.Constant(parser(val), type));
                    arrDict.Add(type, val => Expression.Constant(
                        val.Select(item => parser(item)).ToArray(), type.MakeArrayType()));
                }
            }

            private static ConstantExpression GetExpForEnumType(Type enumType, string val)
            {
                var obj = Enum.Parse(enumType, val, true);
                return Expression.Constant(obj, enumType);
            }

            private static ConstantExpression GetExpForEnumType(Type enumType, string[] vals)
            {
                var objs = vals.Select(v => Enum.Parse(enumType, v, true)).ToArray();
                return Expression.Constant(objs, enumType.MakeArrayType());
            }

            private static Func<string, ConstantExpression> GetExp<T>(Func<string, T> func) where T : struct
            {
                return val => Expression.Constant(func(val), typeof(T));
            }

            private static Func<string, ConstantExpression> GetExpForNullable<T>(Func<string, T> func) where T : struct
            {
                return val =>
                {
                    if (string.IsNullOrEmpty(val)) return Expression.Constant(null, typeof(Nullable<T>));
                    else return Expression.Constant(func(val), typeof(Nullable<T>));
                };
            }

            private static Func<string, ConstantExpression> GetExpForStringType()
            {
                return val => Expression.Constant(val, typeof(string));
            }

            private static Func<string, ConstantExpression> GetExpForArrayOfCharType()
            {
                return val => Expression.Constant(val == null ? (char[])null : val.ToCharArray(), typeof(char[]));
            }

            private static Func<string[], ConstantExpression> GetArrayExp<T>(Func<string, T> func) where T : struct
            {
                return val => Expression.Constant(val.Select(item => func(item)).ToArray(), typeof(T[]));
            }

            private static Func<string[], ConstantExpression> GetArrayExpForStringType()
            {
                return val => Expression.Constant(val, typeof(string[]));
            }
        }

        public class ParameterUpdateVisitor : ExpressionVisitor
        {
            private ParameterExpression _oldParameter;
            private ParameterExpression _newParameter;

            public ParameterUpdateVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (object.ReferenceEquals(node, _oldParameter))
                    return _newParameter;

                return base.VisitParameter(node);
            }
        }

        public class ExpressionReplaceVisitor : ExpressionVisitor
        {
            private Expression _oldExp;
            private Expression _newExp;

            public ExpressionReplaceVisitor(Expression oldExp, Expression newExp)
            {
                this._oldExp = oldExp;
                this._newExp = newExp;
            }

            public override Expression Visit(Expression node)
            {
                if(object.ReferenceEquals(node, _oldExp))
                    return _newExp;
                return base.Visit(node);
            }
        }
    }
}
