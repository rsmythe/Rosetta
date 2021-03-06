﻿/// <summary>
/// MockedClassDeclarationTranslationUnit.cs
/// Andrea Tino - 2015
/// </summary>

namespace Rosetta.Translation.Mocks
{
    using System;
    using System.Collections.Generic;

    using Rosetta.Translation;

    /// <summary>
    /// Mock for <see cref="ClassDeclarationTranslationUnit"/>.
    /// </summary>
    public class MockedClassDeclarationTranslationUnit : ClassDeclarationTranslationUnit
    {
        protected MockedClassDeclarationTranslationUnit(ClassDeclarationTranslationUnit original)
            : base(original)
        {
        }
        
        public static MockedClassDeclarationTranslationUnit Create(ClassDeclarationTranslationUnit classDeclarationTranslationUnit)
        {
            return new MockedClassDeclarationTranslationUnit(classDeclarationTranslationUnit);
        }

        public new ITranslationUnit Name => base.Name;

        public IEnumerable<ITranslationUnit> MemberDeclarations
        {
            get { return this.memberDeclarations; }
        }

        public IEnumerable<ITranslationUnit> ConstructorDeclarations
        {
            get { return this.constructorDeclarations; }
        }

        public IEnumerable<ITranslationUnit> PropertyDeclarations
        {
            get { return this.propertyDeclarations; }
        }

        public IEnumerable<ITranslationUnit> MethodDeclarations
        {
            get { return this.methodDeclarations; }
        }
    }
}
