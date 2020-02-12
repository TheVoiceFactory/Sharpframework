using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;


namespace Sharpframework.Roslyn.CSharp
{
    public static class CompilationHelper
    {
        private static readonly PortableExecutableReference []  DefaultReferredAssemblies;
        private static List<Diagnostic>                         __diagnostics;

        static CompilationHelper ()
        {
            __diagnostics = new List<Diagnostic> ();

            String assemblyPath = Path.GetDirectoryName ( typeof ( Object ).Assembly.Location );

            DefaultReferredAssemblies = new PortableExecutableReference []
            {
                MetadataReference.CreateFromFile (
                    Path.Combine ( assemblyPath, "mscorlib.dll" ) ),

                MetadataReference.CreateFromFile (
                    Path.Combine ( assemblyPath, "System.dll" ) ),

                MetadataReference.CreateFromFile (
                    Path.Combine ( assemblyPath, "System.Core.dll" ) ),

                MetadataReference.CreateFromFile (
                    Path.Combine ( assemblyPath, "System.Runtime.dll" ) )
            };
            //CompilationOptions = new CSharpCompilationOptions ();
        }


        public static IEnumerable<Diagnostic> Diagnostics
        { get => __diagnostics; }


        public static Assembly CompileLibrary (
            String                      assemblyName,
            IEnumerable<Assembly>       referredAssemblies,
            SyntaxTree                  syntaxTree )
                => CompileLibrary ( assemblyName, referredAssemblies, syntaxTree, null );

        
#if DEBUG
        public static Assembly CompileLibrary (
            String                      assemblyName,
            IEnumerable<Assembly>       referredAssemblies,
            SyntaxTree                  syntaxTree,
            CSharpCompilationOptions    compilationOtions )
        {
            Assembly            assembly;
            Byte []             buffer;
            CSharpCompilation   compilation;
            SyntaxTree          encodedTreeStx;
            Encoding            encoding;
            String              sourceCodePath;
            SourceText          sourceText;
            String              symbolsName;
            CSharpSyntaxNode    syntaxRootNode;

            encoding        = Encoding.UTF8;
            sourceCodePath  = Path.ChangeExtension ( assemblyName, "cs" );
            symbolsName     = Path.ChangeExtension ( assemblyName, "pdb" );
            syntaxRootNode  = syntaxTree.GetRoot () as CSharpSyntaxNode;
            buffer          = encoding.GetBytes ( syntaxRootNode.ToFullString () );
            sourceText      = SourceText.From (
                                buffer, buffer.Length, encoding, canBeEmbedded: true );
            encodedTreeStx  = CSharpSyntaxTree.Create (
                                syntaxRootNode, null, sourceCodePath, encoding );

            compilation     = CreateLibraryCompilation (
                                assemblyName, referredAssemblies, encodedTreeStx, compilationOtions );

            __diagnostics.Clear ();
            __diagnostics.AddRange ( compilation.GetDiagnostics () );

            if ( __diagnostics.Count > 0 )
                if ( __diagnostics.FirstOrDefault ( diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error ) != null )
                    return null;

            using ( Stream assemblyStream   = new MemoryStream () )
            using ( Stream symbolsStream    = new MemoryStream () )
            {
                List<EmbeddedText>  embeddedTexts;
                EmitOptions         emitOptions;
                EmitResult          emitResult;

                emitOptions = new EmitOptions (
                                    debugInformationFormat  : DebugInformationFormat.PortablePdb,
                                    pdbFilePath             : symbolsName );

                embeddedTexts = new List<EmbeddedText>
                {
                    EmbeddedText.FromSource(sourceCodePath, sourceText)
                };

                emitResult = compilation.Emit (
                                peStream        : assemblyStream,
                                pdbStream       : symbolsStream,
                                embeddedTexts   : embeddedTexts,
                                options         : emitOptions );

                __diagnostics.AddRange ( emitResult.Diagnostics );

                if ( !emitResult.Success && __diagnostics.Count > 0 )
                    if ( __diagnostics.FirstOrDefault ( diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error ) != null )
                        return null;

                assemblyStream.Seek ( 0, SeekOrigin.Begin );
                symbolsStream?.Seek ( 0, SeekOrigin.Begin );

                assembly = AssemblyLoadContext.Default.LoadFromStream ( assemblyStream, symbolsStream );

                return assembly;
            }
        }
#else
        public static Assembly CompileLibrary (
            String                      assemblyName,
            IEnumerable<Assembly>       referredAssemblies,
            SyntaxTree                  syntaxTree,
            CSharpCompilationOptions    compilationOtions )
        {
            Assembly            assembly;
            CSharpCompilation   compilation;
            CSharpSyntaxNode    syntaxRootNode;

            syntaxRootNode  = syntaxTree.GetRoot () as CSharpSyntaxNode;

            compilationOtions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithOptimizationLevel(OptimizationLevel.Release).WithOverflowChecks(false);
            compilation     = CreateLibraryCompilation (
                                assemblyName, referredAssemblies, syntaxTree, compilationOtions );

            __diagnostics.Clear ();
            __diagnostics.AddRange ( compilation.GetDiagnostics () );

            if ( __diagnostics.Count > 0 )
                if ( __diagnostics.FirstOrDefault ( diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error ) != null )
                    return null;

            using ( Stream assemblyStream = new MemoryStream () )
            {
                EmitResult emitResult = compilation.Emit ( assemblyStream );

                __diagnostics.AddRange ( emitResult.Diagnostics );

                if ( !emitResult.Success && __diagnostics.Count > 0 )
                    if ( __diagnostics.FirstOrDefault ( diagnostic =>
                            diagnostic.IsWarningAsError ||
                            diagnostic.Severity == DiagnosticSeverity.Error ) != null )
                        return null;

                assemblyStream.Seek ( 0, SeekOrigin.Begin );

                assembly = AssemblyLoadContext.Default.LoadFromStream ( assemblyStream );

                return assembly;
            }
        }
#endif

        public static CSharpCompilation CreateLibraryCompilation (
            String                  assemblyName,
            IEnumerable<Assembly>   referredAssemblies,
            SyntaxTree              syntaxTree )
            => CreateLibraryCompilation ( assemblyName, referredAssemblies, syntaxTree, null );

        public static CSharpCompilation CreateLibraryCompilation (
            String                      assemblyName,
            IEnumerable<Assembly>       referredAssemblies,
            SyntaxTree                  syntaxTree,
            CSharpCompilationOptions    options )
        {
            IEnumerable<SyntaxTree> _SyntaxTrees ()
            { yield return syntaxTree; yield break; }

            return CreateLibraryCompilation ( assemblyName, referredAssemblies, _SyntaxTrees (), options );
        }

        public static CSharpCompilation CreateLibraryCompilation (
            String                  assemblyName,
            IEnumerable<Assembly>   referredAssemblies,
            IEnumerable<SyntaxTree> syntaxTrees )
                => CreateLibraryCompilation ( assemblyName, referredAssemblies, syntaxTrees, null );
        public static CSharpCompilation CreateLibraryCompilation (
            String                      assemblyName,
            IEnumerable<Assembly>       referredAssemblies,
            IEnumerable<SyntaxTree>     syntaxTrees,
            CSharpCompilationOptions    options )
        {
            IEnumerable<MetadataReference> _References ()
            {
                foreach ( MetadataReference assemblyRef in DefaultReferredAssemblies )
                    yield return assemblyRef;


                foreach ( Assembly assembly in referredAssemblies )
                    yield return MetadataReference.CreateFromFile ( assembly.Location );

                yield break;
            }

            if ( options == null )
            {
#if DEBUG
                OptimizationLevel optimizationLevel = OptimizationLevel.Debug;
#else
                OptimizationLevel optimizationLevel = OptimizationLevel.Release;
#endif
                options = new CSharpCompilationOptions (
                                    outputKind        : OutputKind.DynamicallyLinkedLibrary,
                                    optimizationLevel : optimizationLevel );
            }

            return CSharpCompilation.Create (
                            assemblyName    : assemblyName,
                            options         : options,
                            references      : _References (),
                            syntaxTrees     : syntaxTrees );

        }
    }
}
