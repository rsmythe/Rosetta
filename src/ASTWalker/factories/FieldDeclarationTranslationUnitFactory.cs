﻿/// <summary>
/// FieldDeclarationTranslationUnitFactory.cs
/// Andrea Tino - 2016
/// </summary>

namespace Rosetta.AST.Factories
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Rosetta.Translation;
    using Rosetta.AST.Helpers;

    /// <summary>
    /// Factory for <see cref="FieldDeclarationTranslationUnit"/>.
    /// </summary>
    public class FieldDeclarationTranslationUnitFactory : ITranslationUnitFactory
    {
        // TODO: Create common base class for all translation unit factories

        private readonly CSharpSyntaxNode node;
        private readonly SemanticModel semanticModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDeclarationTranslationUnitFactory"/> class.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="semanticModel">The semantic model</param>
        public FieldDeclarationTranslationUnitFactory(CSharpSyntaxNode node, SemanticModel semanticModel = null)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node), "A node must be specified!");
            }

            this.node = node;
            this.semanticModel = semanticModel;
        }

        /// <summary>
        /// Creates a <see cref="MethodDeclarationTranslationUnit"/>.
        /// </summary>
        /// <returns>A <see cref="MethodDeclarationTranslationUnit"/>.</returns>
        public ITranslationUnit Create()
        {
            if (this.DoNotCreateTranslationUnit)
            {
                return null;
            }

            var helper = new FieldDeclaration(this.node as FieldDeclarationSyntax, this.semanticModel);

            var fieldDeclaration = this.CreateTranslationUnit(helper.Visibility,
                TypeIdentifierTranslationUnit.Create(helper.Type), IdentifierTranslationUnit.Create(helper.Name));

            return fieldDeclaration;
        }

        /// <summary>
        /// Gets the <see cref="CSharpSyntaxNode"/>.
        /// </summary>
        protected CSharpSyntaxNode Node
        {
            get { return this.node; }
        }

        /// <summary>
        /// Gets a value indicating whether the factory should return <code>null</code>.
        /// </summary>
        protected virtual bool DoNotCreateTranslationUnit
        {
            get { return false; }
        }

        /// <summary>
        /// Creates the translation unit.
        /// </summary>
        /// <param name="visibility"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual ITranslationUnit CreateTranslationUnit(
            VisibilityToken visibility, ITranslationUnit type, ITranslationUnit name)
        {
            return FieldDeclarationTranslationUnit.Create(visibility, type, name);
        }
    }
}
