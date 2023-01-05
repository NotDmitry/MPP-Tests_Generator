﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;


namespace Tests_Generator.Core;

public class Generator
{
    private readonly SyntaxTree _tree;

    // Generate syntax tree
    public Generator(string code)
    {
        _tree = CSharpSyntaxTree.ParseText(code);
    }

    public List<TargetContainer> GenerateInfo()
    {
        var result = new List<TargetContainer>();
        if (_tree is null)
            return result;

        Dictionary<ClassDeclarationSyntax, NamespaceDeclarationSyntax> names = new();
        List<ClassDeclarationSyntax> generatedClasses = new List<ClassDeclarationSyntax>();

        var usings = _tree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>();
        var namespaces = _tree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>();
        var publicClasses = _tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
            .Where(cl => cl.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();

        var additionalUsings = new SyntaxList<UsingDirectiveSyntax>(usings)
            .Add(UsingDirective(ParseName("System")))
            .Add(UsingDirective(ParseName("System.Collections.Generic")))
            .Add(UsingDirective(ParseName("System.Linq")))
            .Add(UsingDirective(ParseName("System.Text")))
            .Add(UsingDirective(ParseName("System.Threading.Tasks")))
            .Add(UsingDirective(ParseName("Microsoft.VisualStudio.TestTools.UnitTesting")));

        foreach (var publicClass in publicClasses)
        {
            var generatedClass = GenerateClass(publicClass);
            generatedClasses.Add(generatedClass);
            names.Add(generatedClass, publicClass.Parent as NamespaceDeclarationSyntax);
        }

        foreach (var generatedClass in generatedClasses)
        {
            var namespaceName = names.GetValueOrDefault(generatedClass).Name;
            var namespaceMember = NamespaceDeclaration(
                ParseName(namespaceName.ToFullString() + ".Tests"));

            var resultCode = CompilationUnit()
                .WithUsings(additionalUsings
                    .Add(UsingDirective(ParseName(namespaceName.ToString()))))
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(namespaceMember
                        .WithMembers(
                            List<MemberDeclarationSyntax>(SingletonList(generatedClass))
                        )
                    )
                ).NormalizeWhitespace().ToFullString();

            result.Add(new TargetContainer(generatedClass.Identifier.Text, resultCode));
        }
        return result;
    }

    public ClassDeclarationSyntax GenerateClass(ClassDeclarationSyntax srcClass)
    {
        // Class attribute
        var attribute = SingletonList(AttributeList(
             SingletonSeparatedList(Attribute(IdentifierName("TestClass")))));

        // Public scope
        var modifier = TokenList(Token(SyntaxKind.PublicKeyword));

        // Class methods
        var testMethods = GenerateMethods(srcClass);

        // Create new class declaration
        var testClass = ClassDeclaration(srcClass.Identifier.Text + "Tests")
            .WithAttributeLists(attribute)
            .WithModifiers(modifier)
            .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(testMethods));

        return testClass;
    }


    public List<MemberDeclarationSyntax> GenerateMethods(ClassDeclarationSyntax testClass)
    {
        List<MemberDeclarationSyntax> resultMethods = new List<MemberDeclarationSyntax>();

        // Attribute for test methods
        var attribute = SingletonList(AttributeList(
            SingletonSeparatedList(Attribute(IdentifierName("TestMethod")))));

        // Public scope
        var modifier = TokenList(Token(SyntaxKind.PublicKeyword));

        // Return type for test method
        var returnType = PredefinedType(Token(SyntaxKind.VoidKeyword));

        // Test method body (mock)
        var body = Block(SingletonList(ParseStatement("Assert.Fail(\"autogenerated\");")));

        // Grouped overloaded methods
        var overloadedMethods = testClass.DescendantNodes().OfType<MethodDeclarationSyntax>()
            .GroupBy(method => method.Identifier.Text);

        foreach (var overloadedMethodsGroup in overloadedMethods)
        {
            var methods = overloadedMethodsGroup.ToList();

            int i = 1;
            string adding = methods.Count > 1 ? i.ToString() : string.Empty;

            foreach (var method in methods)
            {
                resultMethods.Add(MethodDeclaration(returnType,
                    Identifier(method.Identifier.Text + adding + "Test"))
                    .WithAttributeLists(attribute)
                    .WithModifiers(modifier)
                    .WithBody(body));

                if (adding != string.Empty) 
                    i++;
            }
        }

        return resultMethods;
    }
}