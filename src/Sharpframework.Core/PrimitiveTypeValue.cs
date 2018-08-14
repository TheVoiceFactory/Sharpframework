using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharpframework.Core
{
    public class CoreObject { }
    public interface IDimension
    {
        string DimensionID { get; }
    }

    public interface IHasPrimitiveValue<TValue> { TValue PrimitiveValue { get; } }

    public abstract class PrimitiveType
        : CoreObject
    {
    } // End of Class PrimitiveType

    public abstract class PrimitiveType<TValue, TDerivedType>
        : PrimitiveType//CoreObject
        , IPrimitiveTypeValue<TValue>
    where TDerivedType : PrimitiveType<TValue, TDerivedType>, new()
    {
        public override string ToString()
        {
            return this.PrimitiveValue.ToString();
        }
        public override bool Equals(object obj)
        {
            return this.PrimitiveValue . Equals(obj);
        }
        public override int GetHashCode()
        {
            return this.PrimitiveValue.GetHashCode();
        }
        public TValue PrimitiveValue { get { return _val; } set { _val = PreAssignement ( value ); } }

        public virtual TValue PreAssignement ( TValue value )
        {
            return value;
        }
        private TValue _val;

        protected PrimitiveType ()
        { }

        protected PrimitiveType ( TValue val )
        { _val = val; }

        private Boolean _SetValue ( TValue value )
        {
            PrimitiveValue = value;

            return true;
        }

        private Boolean _SetValue ( Object value ) => false;

 
        Boolean IValueGetter.GetValue<ValueType> ( ref ValueType value )
        {
            if ( !typeof ( ValueType ).IsAssignableFrom ( typeof ( TValue ) ) ) return false;

            value = (ValueType)(Object)PrimitiveValue;

            return true;
        }

        TValue IValueManager<TValue>.Value { get => PrimitiveValue; set => PrimitiveValue = value; }

        Object IValueProvider.Value => PrimitiveValue;

        TValue IValueProvider<TValue>.Value => PrimitiveValue;

        Boolean IValueSetter.SetValue<ValueType> ( ValueType value )
            => _SetValue ( (dynamic) value );


        public static implicit operator TValue ( PrimitiveType<TValue, TDerivedType> val )
        {
            if ( val == null ) return default ( PrimitiveType<TValue, TDerivedType> );
            return val.PrimitiveValue;
        }

        public static implicit operator PrimitiveType<TValue, TDerivedType>( TValue val )
        {
            TDerivedType t = new TDerivedType();

            t.PrimitiveValue = val;
            return t;
        }
        public static TDerivedType ToDerivedType ( TValue val )
        {
            TDerivedType t = new TDerivedType();

            t.PrimitiveValue = val;
            return t;
        }

        public static implicit operator TDerivedType ( PrimitiveType<TValue, TDerivedType> val )//TDerivedType TDerivedType(TValue val)
        {
            TDerivedType t = new TDerivedType();

            t.PrimitiveValue = val;
            return t;
        }
    }

    public abstract class DimensionalPrimitiveType<TValue, TDerivedType, TDimension>
        : PrimitiveType<TValue, TDerivedType>
        where TDimension : IDimension
        where TDerivedType : PrimitiveType<TValue, TDerivedType>, new()
    {
    }

}
