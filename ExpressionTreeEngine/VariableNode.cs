// <copyright file="VariableNode.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

namespace ExpressionTreeEngine
{
    /// <summary>
    /// The VariableNode Class for handling Variables in the Expression Tree.
    /// </summary>
    internal class VariableNode : ExpressionTreeNode
    {
        /// <summary>
        /// The dictionary that holds all of the Variables.
        /// </summary>
        private readonly Func<string, double> dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="dictionary`">The dictionary.</param>
        public VariableNode(string name, Func<string, double> dictionary)
        {
            this.Name = name;
            this.dict = dictionary;
        }

        /// <summary>
        /// Gets name of variable, will be replaced by value in the expression.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Evaluation returns the Value.
        /// </summary>
        /// <returns>double Value.</returns>
        public override double Evaluate()
        {
            return this.dict(this.Name);
        }
    }
}
