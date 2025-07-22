// <copyright file="ExpressionTreeTests.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using System.Globalization;
using ExpressionTreeEngine;
using NUnit.Framework;

namespace SpreadsheetTests
{
    /// <summary>
    /// Tests for the ExpressionTree Class.
    /// </summary>
    [TestFixture]
    public class ExpressionTreeTests
    {
        /// <summary>
        /// "Tests the Evaluate function of the ExpressionTree
        /// class with various mathematical expressions.
        /// </summary>
        /// <param name="expression">The Expression that is to be evaluated.</param>
        /// <returns>The double value of the Evaluate method.</returns>
        [Test]
        [TestCase("3+5", ExpectedResult = 8.0)] // expression with a single operator
        [TestCase("100/10*10", ExpectedResult = 100.0)] // mixing operators (/ and *) with same precedence
        [TestCase("100/(10*10)", ExpectedResult = 1.0)] // mixing operators (/ and *) with same precedence and parenthesis
        [TestCase("7-4+2", ExpectedResult = 5.0)] // mixing operators (+ and 2) with same precedence
        [TestCase("10/(7-2)", ExpectedResult = 2.0)] // operators with different precedence with parentheses - higher precedence first
        [TestCase("(12-2)/2", ExpectedResult = 5.0)] // operators with different precedence with parentheses - low precedence first
        [TestCase("(((((2+3)-(4+5)))))", ExpectedResult = -4.0)] // extra parentheses and negative result
        [TestCase("2*3+5", ExpectedResult = 11.0)] // operators with different precedence - higher precedence first
        [TestCase("2+3*5", ExpectedResult = 17.0)] // operators with different precedence - lower precedence first
        [TestCase("2 + 3 * 5", ExpectedResult = 17.0)] // spaces and mixing operators (+ and *) with different precedence
        [TestCase("5/0", ExpectedResult = double.PositiveInfinity)] // Dividing a floating-point value by zero do3sn't throw and exception:it results in positive infinity. negative infinity, or not a number (NaN), according to othe rules of IEEE 754 arithmatic.
        public double TestEvalutateNormalCases(string expression)
        {
            ExpressionTree exp = new (expression);
            return exp.Evaluate();
        }

        /// <summary>
        /// Tests the behavior of the construction of the ExpressionTree when
        /// an invalid expression with mismatched parenthesis is given.
        /// </summary>
        /// <param name="expression">The Expression that is to be evaluated.</param>
        [TestCase("((2+5))-2(2+3))")]
        public void TestConstructInvalidExpression(string expression)
        {
            Assert.That(del: () => new ExpressionTree(expression), expr: Throws.TypeOf<System.Exception>());
        }

        /// <summary>
        /// Tests the behavior of the ExpressionTree's Evalueate when
        /// an unsupported operator is given.
        /// </summary>
        /// <param name="expression">The Expression that is to be evaluated.</param>
        [TestCase("4%2")]
        public void TestEvaluateUnsupportedOperator(string expression)
        {
            Assert.That(del: () => new ExpressionTree(expression), expr: Throws.TypeOf<System.Collections.Generic.KeyNotFoundException>());
        }

        /// <summary>
        /// Tests the behavior of the ExpressionTree's Evaluate function when the
        /// sum of two maximum double values is calculated, verifying that the
        /// result correctly identifies an infinite value.
        /// </summary>
        [Test]
        public void TestInfinity()
        {
            string maxValue = double.MaxValue.ToString(format: "F", CultureInfo.InvariantCulture);
            double result = new ExpressionTree($"{maxValue}+{maxValue}").Evaluate();
            Assert.That(double.IsInfinity(result), Is.True);
        }

        /// <summary>
        /// Tests the ExpressionTree's ability to evaluate expressions containing
        /// variable names, ensuring it correctly calculates the result after
        /// variable values are assigned.
        /// </summary>
        [Test]
        public void TestExpressionsWithVariableValues()
        {
            ExpressionTree exp = new ("A3+5"); // Testing a single variable addition exression
            exp.SetVariable("A3", 23);
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 28));

            exp = new ExpressionTree("B2+A3*5"); // Testing a multi-variable exression
            exp.SetVariable("A3", 3);
            exp.SetVariable("B2", 2);
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 17));
        }

        /// <summary>
        /// Tests that variables are added to the dictionary when the expression tree
        /// is built and have a default value of 0.
        /// </summary>
        [Test]
        public void TestVariableDefaultValue()
        {
            ExpressionTree exp = new ("A3+5"); // Testing a single variable addition exression
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 5));
        }

        /// <summary>
        /// Tests that the variable dictionary is reset when a new expression is input.
        /// </summary>
        [Test]
        public void TestVariableReset()
        {
            ExpressionTree exp = new ("A3+5"); // Testing a single variable addition exression
            exp.SetVariable("A3", 23);
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 28));

            exp = new ("A3+5"); // Testing a single variable addition exression
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 5));
        }

        /// <summary>
        /// Tests that the variable values persist as long as the expression in NOT changed.
        /// </summary>
        [Test]
        public void TestVariablePersistance()
        {
            ExpressionTree exp = new ("A3+5"); // Testing a single variable addition exression
            exp.SetVariable("A3", 23);
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 28));

            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 28)); // tests that after Evaluate(), Variable stays the same.

            exp.SetVariable("B3", 10);
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 28)); // tests that if another variable is set, the original stays the same.

            exp.SetVariable("A3", 10);
            Assert.That(actual: exp.Evaluate(), Is.EqualTo(expected: 15)); // tests that if the variable is given i different value it still function as expected.
        }
    }
}
