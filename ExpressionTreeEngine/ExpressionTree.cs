// <copyright file="ExpressionTree.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using System.Text;

namespace ExpressionTreeEngine
{
    /// <summary>
    /// The Expression Tree Class responsible for building the expression tree,
    /// setting variables, and evaluating the expression tree.
    /// </summary>
    public class ExpressionTree
    {
        /// <summary>
        /// The dictionary that holds all of the variables and their values.
        /// </summary>
        private Dictionary<string, double> variables = new Dictionary<string, double>();

        /// <summary>
        /// creates and operatorNodeFactory.
        /// </summary>
        private OperatorNodeFactory operatorNodeFactory = new OperatorNodeFactory();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="exp">The expression passed in that is to be made into an expression tree.</param>
        public ExpressionTree(string exp = "")
        {
            this.Expression = exp;
            List<string> postFixExp = this.ConvertToPostFix(exp);
            this.Root = this.Compile(postFixExp);
        }

        /// <summary>
        /// Gets or sets class string object to the expression.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets Class ExpressionTreeNode object for the Root node of the Built Expression Tree.
        /// </summary>
        public ExpressionTreeNode? Root { get; set; }

        /// <summary>
        /// Updates or adds a variable with the specified name and value to the variables dictionary.
        /// </summary>
        /// <param name="variableName">Holds the name of the variable that will be used as a key in the dictionary.</param>
        /// <param name="variableValue">Holds the double Value of the variableName.</param>
        public void SetVariable(string variableName, double variableValue)
        {
            this.variables[variableName] = variableValue;
        }

        /// <summary>
        /// Gets the variable names.
        /// </summary>
        /// <returns>Returns a list of variables.</returns>
        public List<string> GetVariables()
        {
            return this.variables.Keys.ToList();
        }

        /// <summary>
        /// Calculates and returns the result of evaluating the expression tree,
        /// starting from the root node.
        /// </summary>
        /// <returns>The Evaluated result of the expression.</returns>
        public double Evaluate()
        {
            double result = 0.0;
            if (this.Root != null)
            {
                result = this.Root.Evaluate();
            }

            return result;
        }

        /// <summary>
        /// Creates and returns an OperatorNode corresponding to the
        /// provided operator character.
        /// </summary>
        /// <returns>A constructed operator node of the specified operator.</returns>
        /// =<param name="op">Holds the operator character.</param>
        internal OperatorNode? CreateOperatorNode(char op)
        {
            return this.operatorNodeFactory.CreateOperatorNode(op);
        }

        /// <summary>
        /// Constructs the expression tree using a postfix expression.
        /// </summary>
        /// <returns>A newly constructed Node.</returns>
        /// <param name="postfix">The expression.</param>
        private ExpressionTreeNode? Compile(List<string> postfix)
        {
            var nodeStack = new Stack<ExpressionTreeNode>();

            foreach (var token in postfix)
            {
                if (double.TryParse(token, out double value))
                {
                    nodeStack.Push(new ConstantNode(value));
                }
                else if (!this.operatorNodeFactory.GetOperators().Contains(token[0])) // Assuming it's a variable
                {
                    // Adds variable to dictionary with a default value of 0.
                    if (!this.variables.ContainsKey(token))
                    {
                        this.variables[token] = 0.0;
                    }

                    nodeStack.Push(new VariableNode(token, name => this.variables[name]));
                }
                else // Operator
                {
                    var rightNode = nodeStack.Pop();
                    var leftNode = nodeStack.Pop();
                    var operatorNode = this.CreateOperatorNode(token[0]); // Assuming single char operators
                    if (operatorNode != null)
                    {
                        operatorNode.Left = leftNode;
                        operatorNode.Right = rightNode;
                        nodeStack.Push(operatorNode);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Unsupported operator: {token}");
                    }
                }
            }

            return nodeStack.Pop(); // The root of the expression tree
        }

        /// <summary>
        /// Converts an infix expression to a postfix expression.
        /// </summary>
        /// <returns>The postfix expression.</returns>
        /// <param name="exp">The infix expression.</param>
        private List<string> ConvertToPostFix(string exp)
        {
            List<string> output = new List<string>();
            Stack<char> stack = new Stack<char>();

            List<char> operators = this.operatorNodeFactory.GetOperators();

            int index = 0;
            while (index < exp.Length)
            {
                char token = exp[index]; // takes a character off of expression

                if (char.IsWhiteSpace(token)) // if its a whitespace ignore it
                {
                    index++;
                    continue;
                }

                // if its a number, letter, or a decimal increment through the expression till an operator or the end building a string.
                if (char.IsDigit(token) || char.IsLetter(token) || token == '.')
                {
                    var item = new StringBuilder();

                    while (index < exp.Length && (char.IsDigit(exp[index]) || char.IsLetter(exp[index]) || exp[index] == '.'))
                    {
                        item.Append(exp[index]);
                        index++;
                    }

                    output.Add(item.ToString());
                    continue;
                }
                else if (operators.Contains(token)) // if the token is an operator, use the factory to get operator precedent, then add to stack.
                {
                    while (stack.Count > 0 && stack.Peek() != '(' &&
                           this.operatorNodeFactory.GetPrecedence(token) <= this.operatorNodeFactory.GetPrecedence(stack.Peek()))
                    {
                        output.Add(stack.Pop().ToString());
                    }

                    stack.Push(token);
                }
                else if (token == '(') // push to stack open parenthesis
                {
                    stack.Push(token);
                }
                else if (token == ')') // Handling closed parenthesis
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        output.Add(stack.Pop().ToString());
                    }

                    if (stack.Count > 0)
                    {
                        stack.Pop();
                    }
                    else // if parenthesis dont match through exception
                    {
                        throw new Exception("Mismatched parentheses in expression.");
                    }
                }
                else // if none of the if statements catch then a foreign character has been used.
                {
                    throw new KeyNotFoundException($"Unexpected character in expression: {token}");
                }

                if (!char.IsDigit(token) && !char.IsLetter(token))
                {
                    index++;
                }
            }

            while (stack.Count > 0) // Pop any remaining operators and checks for any parenthesis remaining.
            {
                if (stack.Peek() == '(' || stack.Peek() == ')')
                {
                    throw new InvalidOperationException("Mismatched parentheses in expression.");
                }

                output.Add(stack.Pop().ToString());
            }

            return output;
        }
    }
}
