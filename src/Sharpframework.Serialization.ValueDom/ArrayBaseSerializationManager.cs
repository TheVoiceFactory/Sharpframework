using System;
using System.Collections;
using System.Collections.Generic;

using Sharpframework.Core;


namespace Sharpframework.Serialization.ValueDom
{
    using Sharpframework.Serialization;


    public abstract class ArrayBaseSerializationManager<    ArrayType,
                                                            ArrayItemType,
                                                            AdapterType,
                                                            SerializationContextType,
                                                            MyselfType>
        : ValueSequenceSerializationManager<    ArrayType,
                                                ArrayItemType,
                                                AdapterType,
                                                SerializationContextType,
                                                MyselfType>
    where ArrayType                 : IEnumerable<ArrayItemType>
    where AdapterType               : class
                                    , ISymbolTableAdapter<IValueUnit>//IValueSequence>
                                    , new ()
    where SerializationContextType  : ISerializationContext
    where MyselfType                : ArrayBaseSerializationManager<    ArrayType,
                                                                        ArrayItemType,
                                                                        AdapterType,
                                                                        SerializationContextType,
                                                                        MyselfType>
                                    , new ()
    {
        protected override ArrayType ImplDeserializeEnumerable (
            IValueSequence                      serialization,
            IRepositorySerializationContext     context )
        {
            Object              array           = null;
            Int32               arraySize       = 0;
            Int32               idx             = 0;
            String              itemStrValue    = null;
            ICollection         coll            = null;
            IList               list            = null;
            NumericStringValue  numericString   = null;

            if ( (coll = serialization.Value as ICollection) != null )
                arraySize = coll.Count;

            IEnumerator enumerator  = ImplDeserializeEnumerable (
                                            serialization.Value, context ).GetEnumerator ();
            if ( enumerator         == null ) return default ( ArrayType );
            if ( !enumerator.MoveNext ()    ) return default ( ArrayType );

            array = new ArrayItemType [ arraySize ];

            list = array as IList;

            do
                try
                {
                    if ( (numericString = enumerator.Current as NumericStringValue) != null )
                        list [ idx++ ] = numericString.ToType ( typeof ( ArrayItemType ), null );
                    else if ( Type.GetTypeCode ( typeof ( ArrayItemType ) ) == TypeCode.DateTime
                            && (itemStrValue = enumerator.Current as String) != null )
                        list [ idx++ ] = DateTime.ParseExact ( itemStrValue, "o", null );

                    else
                        list [ idx++ ] = enumerator.Current;
                }
                catch   { break; ; }
            while ( enumerator.MoveNext () );

            return (ArrayType) array;
        } // End of ImplDeserializeEnumerable (...)
    } // End of Class ArrayBaseSerializationManager<...>
} // End of Namespace Sharpframework.Serialization.ValueDom
