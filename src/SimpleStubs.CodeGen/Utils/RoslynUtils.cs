﻿using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Etg.SimpleStubs.CodeGen.Utils
{
    static class RoslynUtils
    {
        public static UsingDirectiveSyntax UsingDirective(string nameSpace)
        {
            return SF.UsingDirective(SF.IdentifierName(nameSpace));
        }

        public static IEnumerable<MethodDeclarationSyntax> GetMethodDeclarations(TypeDeclarationSyntax interfaceNode)
        {
            return interfaceNode.DescendantNodes().OfType<MethodDeclarationSyntax>();
        }

        public static ParameterSyntax CreateParameter(string type, string name)
        {
            return SF.Parameter(new SyntaxList<AttributeListSyntax>(), new SyntaxTokenList(), SF.IdentifierName(type), SF.Identifier(new SyntaxTriviaList().Add(SF.Space), name, new SyntaxTriviaList()), null);
        }

        public static BaseListSyntax BaseList(params string[] names)
        {
            return SF.BaseList(SF.SeparatedList<BaseTypeSyntax>(names.Select(name => SF.SimpleBaseType(SF.IdentifierName(name)))));
        }

        public static List<ParameterSyntax> GetMethodParameterSyntaxList(IMethodSymbol methodSymbol)
        {
            var paramsSyntaxList = new List<ParameterSyntax>();
            foreach (IParameterSymbol param in methodSymbol.Parameters)
            {
                ParameterSyntax paramSyntax = SF.Parameter(SF.Identifier(param.Name)).WithType(SF.ParseTypeName(param.Type.GetFullyQualifiedName()));
                paramsSyntaxList.Add(paramSyntax);
            }

            return paramsSyntaxList;
        }

        public static List<IMethodSymbol> GetAllMethods(INamedTypeSymbol interfaceType)
        {
            var methodsToStub = new List<IMethodSymbol>(interfaceType.GetMembers().OfType<IMethodSymbol>());
            methodsToStub.AddRange(GetAllInheritedMethods(interfaceType));
            return methodsToStub;
        }

        public static IEnumerable<IMethodSymbol> GetAllInheritedMethods(ITypeSymbol typeSymbol)
        {
            var methods = new List<IMethodSymbol>();
            if (typeSymbol.AllInterfaces.Any())
            {
                foreach (var baseInterfaceType in typeSymbol.AllInterfaces)
                {
                    methods.AddRange(baseInterfaceType.GetMembers().OfType<IMethodSymbol>());
                }
            }

            return methods;
        }

        public static SyntaxKind GetVisibilityKeyword(ISymbol stubbedInterface)
        {
            return stubbedInterface.DeclaredAccessibility ==
                Accessibility.Internal ? SyntaxKind.InternalKeyword : SyntaxKind.PublicKeyword;
        }
    }
}