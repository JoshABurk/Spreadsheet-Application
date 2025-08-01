﻿// <copyright file="DivisionOperatorNode.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

namespace ExpressionTreeEngine
{
    /// <summary>
    /// The DivisionNode Class that handles the '/' operator.
    /// </summary>
    internal class DivisionOperatorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionOperatorNode"/> class.
        /// </summary>
        public DivisionOperatorNode()
            : base('/')
        {
        }

        /// <summary>
        /// Gets or sets division's precedence to 3.
        /// </summary>
        public override ushort Precedence { get; set; } = 3;

        /// <summary>
        /// Divides the left child by the right child.
        /// </summary>
        /// <returns>double result of evaluation.</returns>
        public override double Evaluate()
        {
            if (this.Left == null || this.Right == null)
            {
                Console.WriteLine("Error: Left or Right op is null");
                throw new Exception("Left or Right child node is null.");
            }

            try
            {
                return this.Left.Evaluate() / this.Right.Evaluate();
            }
            catch (Exception)
            {
                Console.WriteLine("Error applying op to children");
                throw new Exception("Left or Right child's Value was not set.");
            }
        }
    }
}
