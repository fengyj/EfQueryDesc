using me.fengyj.QueryDesc.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc
{
    internal static class FilterExpressionHelper
    {
        static FilterExpressionHelper()
        {
            typesCanConvertTo.Add(
                typeof(byte), new Dictionary<Type, Type> { 
                        { typeof(sbyte), typeof(int) },
                        { typeof(short), typeof(int) },
                        { typeof(decimal), typeof(decimal) },
                        { typeof(double), typeof(double) },
                        { typeof(int), typeof(int) },
                        { typeof(long), typeof(long) },
                        { typeof(float), typeof(float) }
                    });
            typesCanConvertTo.Add(
                typeof(decimal), new Dictionary<Type, Type> { 
                        { typeof(short), typeof(decimal) },
                        { typeof(int), typeof(decimal) },
                        { typeof(long), typeof(decimal) },
                        { typeof(sbyte), typeof(decimal) }
                    });
            typesCanConvertTo.Add(
                typeof(double), new Dictionary<Type, Type> { 
                        { typeof(short), typeof(double) },
                        { typeof(int), typeof(double) },
                        { typeof(long), typeof(double) },
                        { typeof(sbyte), typeof(double) },
                        { typeof(float), typeof(double) }
                    });
            typesCanConvertTo.Add(
                typeof(short), new Dictionary<Type, Type> { 
                        { typeof(int), typeof(int) },
                        { typeof(long), typeof(long) },
                        { typeof(sbyte), typeof(int) },
                        { typeof(float), typeof(float) }
                    });
            typesCanConvertTo.Add(
                typeof(int), new Dictionary<Type, Type> { 
                        { typeof(long), typeof(long) },
                        { typeof(sbyte), typeof(int) },
                        { typeof(float), typeof(float) }
                    });
            typesCanConvertTo.Add(
                typeof(long), new Dictionary<Type, Type> { 
                        { typeof(sbyte), typeof(long) },
                        { typeof(float), typeof(float) }
                    });
            typesCanConvertTo.Add(
                typeof(sbyte), new Dictionary<Type, Type> {
                        { typeof(float), typeof(float) }
                    });
        }

        #region private functions
        private static Dictionary<Type, Dictionary<Type, Type>> typesCanConvertTo 
            = new Dictionary<Type, Dictionary<Type, Type>>();

        private static Expression ReplaceArgsWhenLambda(Expression exp, LambdaExpression lambdaExp)
        {
            if (exp is LambdaExpression)
            {
                var lambdaExp2 = exp as LambdaExpression;
                for (var i = 0; i < lambdaExp2.Parameters.Count; i++)
                {
                    var vistor = new ExpressionUtils.ParameterUpdateVisitor(lambdaExp2.Parameters[i], lambdaExp.Parameters[i]);
                    lambdaExp2 = vistor.Visit(lambdaExp2) as LambdaExpression;
                }
                return lambdaExp2.Body;
            }
            return exp;
        }

        private static void ConvertToSameType(ref LambdaExpression exp1, ref Expression exp2)
        {
            var type1 = exp1.ReturnType;
            var type2 = exp2.Type;
            if (exp2 is LambdaExpression) type2 = (exp2 as LambdaExpression).ReturnType;

            if (type1 == type2) return;

            var isType1Nullable = type1.IsGenericType && type1.GetGenericTypeDefinition() == typeof(Nullable<>);
            var isType2Nullable = type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(Nullable<>);

            var t1 = isType1Nullable ? type1.GenericTypeArguments[0] : type1;
            var t2 = isType2Nullable ? type2.GenericTypeArguments[0] : type2;

            if (t1 == t2)
            {
                if (isType1Nullable)
                {
                    if (exp2 is LambdaExpression)
                    {
                        var lambdaExp2 = exp2 as LambdaExpression;
                        exp2 = Expression.Lambda(Expression.Convert(lambdaExp2.Body, type1), lambdaExp2.Parameters);
                    }
                    else
                    {
                        exp2 = Expression.Convert(exp2, type1);
                    }
                }
                if(isType2Nullable)
                {
                    exp1 = Expression.Lambda(Expression.Convert(exp1.Body, type2), exp1.Parameters);
                }
            }
            else
            {
                // get common type of t1 and t2
                Type commonType = null;
                if (typesCanConvertTo.ContainsKey(t1) && typesCanConvertTo[t1].ContainsKey(t2))
                    commonType = typesCanConvertTo[t1][t2];
                else if (typesCanConvertTo.ContainsKey(t2) && typesCanConvertTo[t2].ContainsKey(t1))
                    commonType = typesCanConvertTo[t2][t1];
                else return;

                if (isType1Nullable || isType2Nullable)
                    commonType = typeof(Nullable<>).MakeGenericType(commonType);

                exp1 = Expression.Lambda(Expression.Convert(exp1.Body, commonType), exp1.Parameters);
                if (exp2 is LambdaExpression)
                {
                    var lambdaExp2 = exp2 as LambdaExpression;
                    exp2 = Expression.Lambda(Expression.Convert(lambdaExp2.Body, commonType), lambdaExp2.Parameters);
                }
                else
                {
                    exp2 = Expression.Convert(exp2, commonType);
                }
            }

        }
        #endregion

        #region common functions
        public static LambdaExpression Equal(
            LambdaExpression lambdaExp,
            Expression exp)
        {
            ConvertToSameType(ref lambdaExp, ref exp);
            return Expression.Lambda(
                Expression.Equal(lambdaExp.Body, ReplaceArgsWhenLambda(exp, lambdaExp)),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression NotEqual(
            LambdaExpression lambdaExp,
            Expression exp)
        {
            ConvertToSameType(ref lambdaExp, ref exp);
            return Expression.Lambda(
                Expression.NotEqual(lambdaExp.Body, exp),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression GreaterThan(
            LambdaExpression lambdaExp,
            Expression exp)
        {
            ConvertToSameType(ref lambdaExp, ref exp);
            return Expression.Lambda(
                Expression.GreaterThan(lambdaExp.Body, ReplaceArgsWhenLambda(exp, lambdaExp)),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression GreaterThanOrEqual(
            LambdaExpression lambdaExp,
            Expression exp)
        {
            ConvertToSameType(ref lambdaExp, ref exp);
            return Expression.Lambda(
                Expression.GreaterThanOrEqual(lambdaExp.Body, ReplaceArgsWhenLambda(exp, lambdaExp)),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression LessThan(
            LambdaExpression lambdaExp,
            Expression exp)
        {
            ConvertToSameType(ref lambdaExp, ref exp);
            return Expression.Lambda(
                Expression.LessThan(lambdaExp.Body, ReplaceArgsWhenLambda(exp, lambdaExp)),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression LessThanOrEqual(
            LambdaExpression lambdaExp,
            Expression exp)
        {
            ConvertToSameType(ref lambdaExp, ref exp);
            return Expression.Lambda(
                Expression.LessThanOrEqual(lambdaExp.Body, ReplaceArgsWhenLambda(exp, lambdaExp)),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression In(
            LambdaExpression lambdaExp,
            ConstantExpression exp)
        {
            return Expression.Lambda(
                Expression.Call(
                    typeof(Enumerable),
                    "Contains",
                    new Type[] { lambdaExp.ReturnType },
                    exp,
                    lambdaExp.Body),
                lambdaExp.Parameters[0]);
        }
        #endregion

        #region string functions
        private static MethodInfo stringMethod_StartsWith =
            typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static MethodInfo stringMethod_EndsWith =
            typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
        private static MethodInfo stringMethod_Contains =
            typeof(string).GetMethod("Contains", new Type[] { typeof(string) });

        public static LambdaExpression StartsWith(
            LambdaExpression lambdaExp,
            ConstantExpression prefix)
        {
            return Expression.Lambda(
                Expression.Call(lambdaExp.Body, stringMethod_StartsWith, prefix),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression EndsWith(
            LambdaExpression lambdaExp,
            ConstantExpression postfix)
        {
            return Expression.Lambda(
                Expression.Call(lambdaExp.Body, stringMethod_EndsWith, postfix),
                lambdaExp.Parameters[0]);
        }

        public static LambdaExpression Contains(
            LambdaExpression lambdaExp,
            ConstantExpression val)
        {
            return Expression.Lambda(
                Expression.Call(lambdaExp.Body, stringMethod_Contains, val),
                lambdaExp.Parameters[0]);
        }
        #endregion

        #region logic calc
        public static LambdaExpression And(
            LambdaExpression exp1,
            LambdaExpression exp2)
        {
            var vistor = new ExpressionUtils.ParameterUpdateVisitor(exp2.Parameters[0], exp1.Parameters[0]);
            var newExp2 = vistor.Visit(exp2) as LambdaExpression;
            return Expression.Lambda(Expression.And(exp1.Body, newExp2.Body), exp1.Parameters[0]);
        }

        public static LambdaExpression Or(
            LambdaExpression exp1,
            LambdaExpression exp2)
        {
            var vistor = new ExpressionUtils.ParameterUpdateVisitor(exp2.Parameters[0], exp1.Parameters[0]);
            var newExp2 = vistor.Visit(exp2) as LambdaExpression;
            return Expression.Lambda(Expression.Or(exp1.Body, newExp2.Body), exp1.Parameters[0]);
        }

        public static LambdaExpression Not(LambdaExpression exp)
        {
            return Expression.Lambda(Expression.Not(exp.Body), exp.Parameters[0]);
        }
        #endregion
    }
}
