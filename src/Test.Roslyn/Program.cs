using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;

namespace Test.Roslyn
{
    class Program
    {
        // Install-Package Microsoft.CodeAnalysis.CSharp.Workspaces -Version 3.0.0 (?)
        // Install-Package Microsoft.CodeAnalysis -Pre
        // Install-Package Microsoft.Composition (?)
        // Install-Package Microsoft.Build.Utilities.Core
        const String SolutionPath = @"..\..\..\sharpframework.publish.sln";
        const String ProjectName = "Test.Roslyn.VisualStudio";

        public class SyntaxNodeReference
        {
            public SyntaxNodeReference ( SyntaxNode node, ReferenceLocation refLoc )
            {
                Node = node;
                Document = refLoc.Document;
                Location = refLoc.Location;
                Root = refLoc.Location.SourceTree.GetRoot ();

                _ReferenceLocation = refLoc;
            }

            public Document Document { get; }
            public SyntaxNode Node { get; }

            public Location Location { get; }

            public SyntaxNode Root { get; }

            private ReferenceLocation _ReferenceLocation { get; }
        }
        public class SyntaxNodeReferences : KeyedCollection<SyntaxNode, SyntaxNodeReference>
        {
            IEnumerable<Document> __documents = new List<Document> ();

            public void Add ( SyntaxNode node, ReferenceLocation refLoc )
            {
                __documents = null;
                base.Add ( new SyntaxNodeReference ( node, refLoc ) );
            }

            public void AddFromSymbolReference ( ReferencedSymbol symRef )
            {
                foreach ( ReferenceLocation refLoc in symRef.Locations )
                {
                    SyntaxNode rootSynNode;
                    SymbolInfo symbolInfo;
                    SyntaxNode synNode;

                    rootSynNode = refLoc.Location.SourceTree.GetRoot ();
                    synNode     = rootSynNode.FindNode ( refLoc.Location.SourceSpan );

                    if ( !(synNode.Parent is AttributeSyntax) ) continue;

                    // Can be optimized, in order to not query SemanticModel anytime.
                    symbolInfo = refLoc.Document.GetSemanticModelAsync ()
                                                    .Result.GetSymbolInfo ( synNode.Parent );

                    if ( symbolInfo.Symbol == symRef.Definition )
                        Add ( synNode.Parent.Parent.Parent, refLoc );
                    else
                        foreach ( ISymbol csi in symbolInfo.CandidateSymbols )
                            if ( csi.ContainingSymbol == symRef.Definition )
                            {
                                Add ( synNode.Parent.Parent.Parent, refLoc );
                                break;
                            }

                }

                return;
            }
            protected override void InsertItem ( int index, SyntaxNodeReference item )
            {
                __documents = null;
                base.InsertItem ( index, item );
            }

            protected override void RemoveItem ( int index )
            {
                __documents = null;
                base.RemoveItem ( index );
            }

            protected override void SetItem ( int index, SyntaxNodeReference item )
            {
                __documents = null;
                base.SetItem ( index, item );
            }
            public IEnumerable<SyntaxNodeReference> GetDocumentReferences ( Document document )
            {
                foreach ( SyntaxNodeReference item in this )
                    if ( item.Document == document )
                        yield return item;

                yield break;
            }

            public IEnumerable<Document> GetDocuments ()
            {
                if ( __documents == null )
                    return this.Select ( ( rfr, doc ) => rfr.Document ).Distinct ();

                return __documents;
            }
            protected override SyntaxNode GetKeyForItem ( SyntaxNodeReference item )
                => item == null ? null : item.Node;
        }

        private static ReferencedSymbol _GetSymbolReference (
            Project project, ISymbol searchedSymbol )
        {
            ReferencedSymbol                retVal = null;
            IEnumerable<ReferencedSymbol>   symbolRefs;

            symbolRefs = SymbolFinder.FindReferencesAsync (
                                            searchedSymbol, project.Solution ).Result;

            if ( symbolRefs == null ) return null;

            foreach ( ReferencedSymbol refSym in symbolRefs )
                if ( refSym.Definition == searchedSymbol )
                    if ( retVal == null )
                        retVal = refSym;
                    else
                        return null; // Should never be reached

            return retVal;
        }
        private static Project _OpenProject ( String solutionPath, String projectName )
        {
            Solution            solution;
            MSBuildWorkspace    workspace;

            if ( (workspace = MSBuildWorkspace.Create ()) == null )
                return null;

            workspace.LoadMetadataForReferencedProjects = true;

            return (solution = workspace.OpenSolutionAsync ( solutionPath ).Result) == null
                        ? null
                        : solution.Projects
                                        .Where ( ( proj ) => proj.Name == projectName )
                                        .FirstOrDefault ();
        }
        static void Main ( string [] args )
        {
            Compilation                         compilation;
            IEnumerable<SyntaxNodeReference>    docStxNodeRefs;
            INamedTypeSymbol                    epilogAttrSym;
            ReferencedSymbol                    attrSymDef;
            Type                                epilogAttrType;
            Project                             project;
            PrologEpilogRewriter                rewriter;
            SyntaxNode                          rewrittenNode;
            SyntaxNodeReferences                stxNodeRefs;
            IEnumerable<Document>               symDocs;

            project = _OpenProject ( SolutionPath, ProjectName );
            compilation     = project.GetCompilationAsync ().Result;
            epilogAttrType  = typeof ( MethodsEpilogAttribute );
            epilogAttrSym   = compilation.GetTypeByMetadataName ( epilogAttrType.FullName );

            // To Look for Symbol Declaration (note: in case of partial class definition, you can
            // receive more than one result).
            //IEnumerable<ISymbol> symbols =
            //    compilation.GetSymbolsWithName (
            //                    (String s) => { return s.Equals(epilogAttrType.Name); },
            //                    SymbolFilter.Type );

            if ( (attrSymDef = _GetSymbolReference ( project, epilogAttrSym )) == null ) return;

            stxNodeRefs = new SyntaxNodeReferences ();

            stxNodeRefs.AddFromSymbolReference ( attrSymDef );

            // Now, for example modify only the contents of the fist document containing the symbol
            symDocs         = stxNodeRefs.GetDocuments ();
            docStxNodeRefs  = stxNodeRefs.GetDocumentReferences ( symDocs.FirstOrDefault () );
            rewriter        = new PrologEpilogRewriter ();

            rewrittenNode = rewriter.Visit ( docStxNodeRefs.FirstOrDefault ().Root );

            Console.WriteLine ( rewrittenNode.NormalizeWhitespace ().ToFullString () );
            Console.ReadKey ();

            return;
        }
    }
}
