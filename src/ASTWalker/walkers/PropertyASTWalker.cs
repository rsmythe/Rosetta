﻿/// <summary>
/// PropertyASTWalker.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.AST
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Symbols;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Text;

    using Rosetta.Translation;
    using Rosetta.AST.Helpers;

    /// <summary>
    /// Walks a property AST node.
    /// </summary>
    public class PropertyASTWalker : CSharpSyntaxWalker, IASTWalker
    {
        // Protected for testability
        protected CSharpSyntaxNode node;

        // Protected for testability
        protected PropertyDeclarationTranslationUnit propertyDeclaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyASTWalker"/> class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="propertyDeclaration"></param>
        protected PropertyASTWalker(CSharpSyntaxNode node, PropertyDeclarationTranslationUnit propertyDeclaration)
        {
            var propertyDeclarationSyntaxNode = node as PropertyDeclarationSyntax;
            if (propertyDeclarationSyntaxNode == null)
            {
                throw new ArgumentException(
                    string.Format("Specified node is not of type {0}",
                    typeof(PropertyDeclarationSyntax).Name));
            }

            if (propertyDeclaration == null)
            {
                throw new ArgumentNullException(nameof(propertyDeclaration));
            }

            this.node = node;
            this.propertyDeclaration = propertyDeclaration;

            // Going through accessors in the node and filling the translation unit with initial data
            this.VisitNode(propertyDeclarationSyntaxNode);
        }

        /// <summary>
        /// Copy initializes a new instance of the <see cref="PropertyASTWalker"/> class.
        /// </summary>
        /// <param name="other"></param>
        /// <remarks>
        /// For testability.
        /// </remarks>
        public PropertyASTWalker(PropertyASTWalker other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            this.node = other.node;
            this.propertyDeclaration = other.propertyDeclaration;
        }

        /// <summary>
        /// Factory method for class <see cref="PropertyASTWalker"/>.
        /// </summary>
        /// <param name="node"><see cref="CSharpSyntaxNode"/> Used to initialize the walker.</param>
        /// <returns></returns>
        public static PropertyASTWalker Create(CSharpSyntaxNode node)
        {
            PropertyDeclaration helper = new PropertyDeclaration(node as PropertyDeclarationSyntax);

            var propertyDeclaration = PropertyDeclarationTranslationUnit.Create(
                helper.Visibility,
                TypeIdentifierTranslationUnit.Create(helper.Type),
                IdentifierTranslationUnit.Create(helper.Name),
                helper.HasGet,
                helper.HasSet);

            return new PropertyASTWalker(node, propertyDeclaration);
        }

        /// <summary>
        /// Walk the whole tree starting from specified <see cref="CSharpSyntaxNode"/> and 
        /// build the translation unit tree necessary for generating TypeScript output.
        /// </summary>
        /// <returns>The root of the translation unit tree.</returns>
        public ITranslationUnit Walk()
        {
            // Visiting
            this.Visit(node);

            // Returning root
            return this.propertyDeclaration;
        }

        #region Walk events

        /// <summary>
        /// 
        /// </summary>
        public event WalkerEvent GetAccessorVisited;

        /// <summary>
        /// 
        /// </summary>
        public event WalkerEvent SetAccessorVisited;

        #endregion

        private void VisitNode(PropertyDeclarationSyntax node)
        {
            foreach (var accessor in node.AccessorList.Accessors)
            {
                IASTWalker walker = BlockASTWalker.Create(accessor.Body);
                ITranslationUnit translationUnit = walker.Walk();

                // TODO: call events
                if (accessor.Kind() == SyntaxKind.GetAccessorDeclaration)
                {
                    this.propertyDeclaration.SetGetAccessor(translationUnit);
                }
                else if (accessor.Kind() == SyntaxKind.SetAccessorDeclaration)
                {
                    this.propertyDeclaration.SetSetAccessor(translationUnit);
                }
            }
        }

        private void InvokeGetAccessorVisited(object sender, WalkerEventArgs e)
        {
            if (this.GetAccessorVisited != null)
            {
                this.GetAccessorVisited(sender, e);
            }
        }

        private void InvokeSetAccessorVisited(object sender, WalkerEventArgs e)
        {
            if (this.SetAccessorVisited != null)
            {
                this.SetAccessorVisited(sender, e);
            }
        }
    }
}
