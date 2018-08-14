using System;


namespace Sharpframework.Serialization
{
    public abstract class SerializationContext<MyselfType>
        : ISerializationContext
        , ISerializeData
    where MyselfType    : SerializationContext<MyselfType>
                        , new ()
    {
        private static MyselfType __none;


        static SerializationContext () { __none = null; }

        protected SerializationContext ()
        {
            SerializeData = true;
        } // End of Default Constructor


        public static MyselfType None
        {
            get
            {
                if ( __none == null ) __none = new MyselfType ();

                return __none;
            }
        }

        public Boolean SerializeData { get; set; }
    } // End of Class SerializationContext<...>
} // End of Namespace Sharpframework.Serialization
