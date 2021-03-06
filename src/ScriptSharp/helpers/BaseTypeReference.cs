﻿/// <summary>
/// BaseTypeReference.cs
/// Andrea Tino - 2016
/// </summary>

namespace Rosetta.ScriptSharp.AST.Helpers
{
    using System;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Rosetta.AST.Helpers;

    /// <summary>
    ///Decorates <see cref="AttributeDecoration"/>.
    /// </summary>
    public class BaseTypeReference : Rosetta.AST.Helpers.BaseTypeReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseTypeReference"/> class.
        /// </summary>
        /// <param name="baseTypeSyntaxNode"></param>
        /// <remarks>
        /// This is a minimal constructor, some properties might be unavailable.
        /// </remarks>
        public BaseTypeReference(BaseTypeSyntax baseTypeSyntaxNode)
            : this(baseTypeSyntaxNode, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="baseTypeSyntaxNode"></param>
        /// <param name="semanticModel"></param>
        /// <param name="kind"></param>
        /// <remarks>
        /// Type kind will be stored statically.
        /// </remarks>
        public BaseTypeReference(BaseTypeSyntax baseTypeSyntaxNode, SemanticModel semanticModel)
            : this(baseTypeSyntaxNode, semanticModel, Microsoft.CodeAnalysis.TypeKind.Unknown)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeReference"/> class.
        /// </summary>
        /// <param name="baseTypeSyntaxNode"></param>
        /// <param name="semanticModel"></param>
        /// <param name="kind"></param>
        /// <remarks>
        /// Type kind will be stored statically.
        /// </remarks>
        public BaseTypeReference(BaseTypeSyntax baseTypeSyntaxNode, SemanticModel semanticModel, Microsoft.CodeAnalysis.TypeKind kind)
            : base(baseTypeSyntaxNode, semanticModel, kind)
        {
        }

        /// <summary>
        /// Gets the base type name.
        /// 
        /// TODO: Improve the logic of this method in order to abstract a common component for semantically detecting the ScriptNamespace
        /// </summary>
        public override string FullName
        {
            get
            {
                if (this.SemanticModel != null)
                {
                    var symbol = this.SemanticModel.GetSymbolInfo(this.BaseTypeSyntaxNode.Type).Symbol;
                    if (symbol != null)
                    {
                        var attributeDatas = symbol.GetAttributes();
                        string overriddenName = null;
                        foreach (var attributeData in attributeDatas)
                        {
                            var scriptNamespaceHelper = new ScriptNamespaceAttributeOnType(new AttributeSemantics(attributeData));
                            if (scriptNamespaceHelper.HasScriptNamespaceAttributeDecoration)
                            {
                                overriddenName = scriptNamespaceHelper.OverridenName;
                            }
                        }

                        if (overriddenName != null)
                        {
                            return $"{overriddenName}.{symbol.Name}";
                        }
                    }
                }

                return base.FullName;
            }
        }
    }
}
