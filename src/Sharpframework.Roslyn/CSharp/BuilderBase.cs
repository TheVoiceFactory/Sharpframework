﻿using System.Collections.Generic;


namespace Sharpframework.Roslyn.CSharp
{
    public abstract class BuilderBase<BuilderType>
        where BuilderType : BuilderBase<BuilderType>
    {
        private class NotNullStack<ItemType>
            : Stack<ItemType>
            where ItemType : class
        {
            public new void Push ( ItemType item )
            { if ( item != null && !Contains ( item ) ) base.Push ( item ); }
        } // End of Class NotNullStack<...>


        private static NotNullStack<BuilderType> __toRecycle;

        static BuilderBase () => __toRecycle = null;

        protected static BuilderType ImplGetRecycled ()
            => __toRecycle == null || __toRecycle.Count > 0
                    ? null : __toRecycle.Pop ().ImplReset ();

        protected static void ImplRelease ( BuilderType releasedItem )
            => _ToRecycle.Push ( releasedItem );

        protected abstract BuilderType ImplReset ();

        private static NotNullStack<BuilderType> _ToRecycle
        {
            get
            {
                if ( __toRecycle == null ) __toRecycle = new NotNullStack<BuilderType> ();

                return __toRecycle;
            }
        }
    } // End of Class BuilderBase<...>
} // End of Namespace Sharpframework.Roslyn.CSharp
