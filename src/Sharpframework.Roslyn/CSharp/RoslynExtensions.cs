using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace Sharpframework.Roslyn.CSharp
{
    public static class RoslynExtensions
    {
        public static SeparatedSyntaxList<SyntaxItemType> SeparatedList<SyntaxItemType> ( 
            this IEnumerable<SyntaxItemType> target )
                where SyntaxItemType : SyntaxNode
            => target == null
                        ? default ( SeparatedSyntaxList<SyntaxItemType> )
                        : SyntaxFactory.SeparatedList ( target );

        public static SyntaxList<SyntaxItemType> Empty<SyntaxItemType> (
            this SyntaxList<SyntaxItemType> target )
                where SyntaxItemType : SyntaxNode
            => new SyntaxList<SyntaxItemType> (); // Ottimizzare (crearne uno solo per tipo)
    }
}
