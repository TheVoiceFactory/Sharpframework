using System.IO;

using Sharpframework.Core;


namespace Sharpframework.Serialization
{
    public interface ISymbolTableAdapter
    {
        IConvertibleString Converter { get; }
    } // End of Interface ISymbolTableAdapter

    public interface ISymbolTableAdapter<RootType>
        : ISymbolTableAdapter
    where RootType : ISymbolTableItem
    {
        RootType Read ( TextReader textReader );

        void Write ( TextWriter textWriter, RootType node );
    } // End of Interface ISymbolTableAdapter<...>
} // End of Namespace Sharpframework.Serialization
