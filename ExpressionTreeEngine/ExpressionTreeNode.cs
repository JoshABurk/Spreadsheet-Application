// <copyright file="ExpressionTreeNode.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

namespace ExpressionTreeEngine
{
    /// <summary>
    /// The abstract EvaluateTreeNode that all children inherit to get the Evaluate method.
    /// </summary>
    public abstract class ExpressionTreeNode
    {
        /// <summary>
        /// The abstract Evaluate Function that all Child Classes overload.
        /// </summary>
        /// <returns>double result of evaluation.</returns>
        public abstract double Evaluate();
    }
}
