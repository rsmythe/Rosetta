﻿/// <summary>
/// MockedMethodDeclarationTranslationUnit.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Translation.Mocks
{
    using System;
    using System.Collections.Generic;

    using Rosetta.Translation;

    /// <summary>
    /// Mock for <see cref="MethodDeclarationTranslationUnit"/>.
    /// </summary>
    public class MockedMethodDeclarationTranslationUnit : MethodDeclarationTranslationUnit
    {
        protected MockedMethodDeclarationTranslationUnit(MethodDeclarationTranslationUnit original)
            : base(original)
        {
        }

        public static MockedMethodDeclarationTranslationUnit Create(MethodDeclarationTranslationUnit methodDeclarationTranslationUnit)
        {
            return new MockedMethodDeclarationTranslationUnit(methodDeclarationTranslationUnit);
        }

        public IEnumerable<ITranslationUnit> Statements
        {
            get { return this.statements; }
        }

        public new IEnumerable<ITranslationUnit> Arguments
        {
            get { return this.arguments; }
        }

        public new ITranslationUnit ReturnType
        {
            get { return this.returnType; }
        }
    }
}
