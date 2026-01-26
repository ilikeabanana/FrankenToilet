using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;

namespace FrankenToilet.Core.Extensions;
[PublicAPI]
public static class HarmonyExtensions
{

    extension(CodeMatcher matcher)
    {
        /// <summary>
        /// Removes instructions matched by the provided <see cref="CodeMatch">CodeMatches</see>.
        /// </summary>
        /// <param name="matches">Some code matches</param>
        /// <returns>The same <see cref="CodeMatcher"/></returns>
        /// <remarks>I forgot where exactly this places you after it's done</remarks>
        /// <exception cref="InvalidOperationException">Thrown if a match is not found</exception>
        [PublicAPI]
        public CodeMatcher RemoveInstructions(params CodeMatch[] matches) =>
            matcher.MatchForward(useEnd: false, matches)
                   .ThrowIfInvalid("Failed to find the match")
                   .RemoveInstructions(matches.Length);
    }

    extension(CodeInstruction)
    {
        /// <summary>
        /// Creates a CodeInstruction with the <see cref="OpCodes.Stsfld">stsfld</see>
        /// or <see cref="OpCodes.Stfld">stfld</see> instruction for the field specified by the given expression.
        /// </summary>
        /// <param name="expression">An expression that uses the field</param>
        /// <example><code>
        /// using FrankenToilet.Core.Extensions;
        /// ...
        /// var inst = CodeInstruction.StoreField(() => someObject.someField);
        /// </code></example>
        public static CodeInstruction StoreField(LambdaExpression expression)
        {
            var operand = GetFieldInfo(expression);
            return new CodeInstruction(operand.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, operand);
        }

        /// <summary>
        /// Creates a CodeInstruction with the <see cref="OpCodes.Ldsfld">ldsfld</see>
        /// or <see cref="OpCodes.Ldfld">ldfld</see> instruction for the field specified by the given expression.
        /// </summary>
        /// <param name="expression">An expression that uses the field</param>
        /// <example><code>
        /// using FrankenToilet.Core.Extensions;
        /// ...
        /// var inst = CodeInstruction.LoadField(() => someObject.someField);
        /// </code></example>
        public static CodeInstruction LoadField(LambdaExpression expression)
        {
            var operand = GetFieldInfo(expression);
            return new CodeInstruction(operand.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, operand);
        }
    }
    /// <summary>
    /// Gets the <see cref="FieldInfo"/> represented by the given <see cref="LambdaExpression"/>.
    /// </summary>
    /// <param name="expression">An expression that uses the field</param>
    /// <exception cref="ArgumentException">If the expression is not a <see cref="MemberExpression"/> for a field</exception>
    /// <example><code>
    /// using FrankenToilet.Core.Extensions;
    /// ...
    /// var fieldInfo1 = HarmonyExtensions.GetFieldInfo(() => SomeType.someField);
    /// var fieldInfo2 = HarmonyExtensions.GetFieldInfo((SomeType someObject) => someObject.someField);
    /// </code></example>
    public static FieldInfo GetFieldInfo(LambdaExpression expression)
    {
        return expression.Body is MemberExpression { Member: FieldInfo fieldInfo }
                   ? fieldInfo
                   : throw new ArgumentException("Expression does not refer to a field.");
    }
    /// <summary>
    /// Gets the <see cref="MethodInfo"/> represented by the given <see cref="LambdaExpression"/>.
    /// </summary>
    /// <param name="expression">An expression that uses the method</param>
    /// <exception cref="ArgumentException">If the expression is not a <see cref="MethodCallExpression"/> or if the method is somehow null</exception>
    /// <example><code>
    /// using FrankenToilet.Core.Extensions;
    /// ...
    /// var methodInfo1 = HarmonyExtensions.GetFieldInfo(() => SomeType.SomeMethod());
    /// var methodInfo2 = HarmonyExtensions.GetFieldInfo((SomeType someObj) => someObj.SomeMethod());
    /// var methodInfo3 = HarmonyExtensions.GetFieldInfo((SomeType someObj, int arg) => someObj.SomeMethod(arg));
    /// </code></example>
    public static MethodInfo GetMethodInfo(LambdaExpression expression)
    {
        return expression.Body is MethodCallExpression { Method: { } methodInfo }
                   ? methodInfo
                   : throw new ArgumentException("Expression does not refer to a method.");
    }
}