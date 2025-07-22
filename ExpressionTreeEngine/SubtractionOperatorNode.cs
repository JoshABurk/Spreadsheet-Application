// <copyright file="SubtractionOperatorNode.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

namespace ExpressionTreeEngine
{
    /// <summary>
    /// The SubtractionNode Class that handles the '-' operator.
    /// </summary>
    internal class SubtractionOperatorNode : OperatorNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractionOperatorNode"/> class.
        /// </summary>
        public SubtractionOperatorNode()
            : base('-')
        {
        }

        /// <summary>
        /// Gets or sets subtraction's precedence to 2.
        /// </summary>
        public override ushort Precedence { get; set; } = 2;

        /// <summary>
        /// Subtracts the right child from the left child.
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
                return this.Left.Evaluate() - this.Right.Evaluate();
            }
            catch (Exception)
            {
                Console.WriteLine("Error applying op to children");
                throw new Exception("Left or Right child's Value was not set.");
            }
        }
    }
}
