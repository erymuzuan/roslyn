﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Composition
Imports System.Threading
Imports Microsoft.CodeAnalysis.CodeActions
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.Formatting
Imports Microsoft.CodeAnalysis.FxCopAnalyzers
Imports Microsoft.CodeAnalysis.FxCopAnalyzers.Design
Imports Microsoft.CodeAnalysis.Shared.Extensions
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic.FxCopAnalyzers.Design

    <ExportCodeFixProvider(LanguageNames.VisualBasic, Name:=StaticTypeRulesDiagnosticAnalyzer.RuleNameForExportAttribute), [Shared]>
    Public Class CA1052BasicCodeFixProvider
        Inherits CodeFixProvider

        Public NotOverridable Overrides ReadOnly Property FixableDiagnosticIds As ImmutableArray(Of String)
            Get
                Return ImmutableArray.Create(StaticTypeRulesDiagnosticAnalyzer.CA1052RuleId)
            End Get
        End Property

        Public NotOverridable Overrides Function GetFixAllProvider() As FixAllProvider
            Return WellKnownFixAllProviders.BatchFixer
        End Function

        Public NotOverridable Overrides Async Function RegisterCodeFixesAsync(context As CodeFixContext) As Task
            Dim document = context.Document
            Dim span = context.Span
            Dim cancellationToken = context.CancellationToken

            cancellationToken.ThrowIfCancellationRequested()
            Dim root = Await document.GetSyntaxRootAsync(cancellationToken)
            Dim classStatement = root.FindToken(span.Start).GetAncestor(Of ClassStatementSyntax)
            If classStatement IsNot Nothing Then
                Dim notInheritableKeyword = SyntaxFactory.Token(SyntaxKind.NotInheritableKeyword).WithAdditionalAnnotations(Formatter.Annotation)
                Dim newClassStatement = classStatement.AddModifiers(notInheritableKeyword)
                Dim newRoot = root.ReplaceNode(classStatement, newClassStatement)
                context.RegisterCodeFix(
                    New MyCodeAction(String.Format(FxCopRulesResources.StaticHolderTypeIsNotStatic, classStatement.Identifier.Text), document.WithSyntaxRoot(newRoot)),
                    context.Diagnostics)
            End If
        End Function

        Private Class MyCodeAction
            Inherits CodeAction.DocumentChangeAction

            Public Sub New(title As String, newDocument As Document)
                MyBase.New(title, Function(c) Task.FromResult(newDocument))
            End Sub
        End Class
    End Class
End Namespace
