// <copyright file="ConstantNode.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

namespace ExpressionTreeEngine
{
    /// <summary>
    /// The ConstantNode Class that is used to create a Node for a constant
    /// value in the expression.
    /// </summary>
    internal class ConstantNode : ExpressionTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNode"/> class.
        /// </summary>
        /// <param name="value">the value of the constant Node.</param>
        public ConstantNode(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the value of the constant.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Returns the value of the constant when ran.
        /// </summary>
        /// <returns>double result of evaluation.</returns>
        public override double Evaluate()
        {
            return this.Value;
        }
    }
}
