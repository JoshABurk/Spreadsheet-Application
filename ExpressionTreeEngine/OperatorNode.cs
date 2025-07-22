// <copyright file="OperatorNode.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

namespace ExpressionTreeEngine
{
    /// <summary>
    /// Superclass of all Operators to be used in the expression tree.
    /// </summary>
    internal abstract class OperatorNode : ExpressionTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class.
        /// </summary>
        /// <param name="c">The Opperator character.</param>
        public OperatorNode(char c)
        {
            this.Operator = c;
            this.Left = this.Right = null;
        }

        /// <summary>
        /// Gets or sets the Operator.
        /// </summary>
        public char Operator { get; set; }

        /// <summary>
        /// Gets or sets left child.
        /// </summary>
        public ExpressionTreeNode? Left { get; set; }

        /// <summary>
        /// Gets or sets right child.
        /// </summary>
        public ExpressionTreeNode? Right { get; set; }

        /// <summary>
        /// Gets or sets the order of operations applied to the expression.
        /// </summary>
        public abstract ushort Precedence { get; set; }
    }
}
