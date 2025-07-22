// <copyright file="OperatorNodeFactory.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTreeEngine
{
    /// <summary>
    /// Operator node factory.
    /// </summary>
    internal class OperatorNodeFactory
    {
        /// <summary>
        /// Dictionary to facilitate Operator node creation.
        /// </summary>
        private static Dictionary<char, Type> operators = new Dictionary<char, Type>
        {
            { '+', typeof(AdditionOperatorNode) },
            { '-', typeof(SubtractionOperatorNode) },
            { '*', typeof(MultiplicationOperatorNode) },
            { '/', typeof(DivisionOperatorNode) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNodeFactory"/> class.
        /// </summary>
        public OperatorNodeFactory()
        {
        }

        /// <summary>
        /// Returns a list of all supported operators.
        /// </summary>
        /// <returns>The operators.</returns>
        public List<char> GetOperators()
        {
            return operators.Keys.ToList();
        }

        /// <summary>
        /// Creates an instance of an Operator node given an operator.
        /// </summary>
        /// <returns>The operator node.</returns>
        /// <param name = "op">Operator.</param>
        public OperatorNode? CreateOperatorNode(char op)
        {
            if (operators.ContainsKey(op))
            {
                object? operatorNodeObject = System.Activator.CreateInstance(operators[op]);
                if (operatorNodeObject is OperatorNode)
                {
                    return (OperatorNode)operatorNodeObject;
                }

                throw new Exception("Unhandled exception");
            }

            return null;
        }

        /// <summary>
        /// Gets the precedence.
        /// </summary>
        /// <returns>The precedence for the requested operator.</returns>
        /// <param name = "op">Operator.</param>
        public ushort GetPrecedence(char op)
        {
            if (operators.ContainsKey(op))
            {
                object? operatorNodeObject = Activator.CreateInstance(operators[op]);
                if (operatorNodeObject is OperatorNode operatorNode)
                {
                    return operatorNode.Precedence;
                }

                throw new Exception("Invalid operator node type");
            }

            throw new KeyNotFoundException($"Operator '{op}' is not supported.");
        }
    }
}
