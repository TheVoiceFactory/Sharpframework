using System;
using System.ComponentModel;


namespace Sharpframework.Core
{
    /// <summary>Questa classe è un wrapper per un PropertyDescriptor, ed è pensata per fornire
    /// funzionalità di sostituzione e/o redirezione di un intero PropertyDescriptor o di alcune
    /// sue funzionalità.
    /// Per questo motivo questa classe costituisce semplicemente un modello e non è quindi
    /// direttamente instanziabile: l'implementazione di tutti i suoi membri si limita a delegare
    /// al PropertyDescriptor originario (passato in fase di costruzione o modificato in seguito
    /// per mezzo della proprietà protetta dedicata a questo scopo).
    /// Specializzando questa classe ci si potrà dunque concentrare solo sugli aspetti da
    /// modificare.
    /// Si noti che questa classe, ereditando da PropertyDescriptor è essa stessa dello stesso
    /// tipo; è proprio grazie a tale cartteristica, che essa può essere assegnata a tutti quei
    /// valori che prevedono come tipo di dato un PropertyDescriptor.</summary>
    public abstract class WrappedPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor  __targetPD;


        /// <summary>Costruttore Predefinito.</summary>
        /// <param name="originalPD">Il PropertyDescriptor originario, di cui questa classe
        /// costituisce il Wrapper.</param>
        protected WrappedPropertyDescriptor ( PropertyDescriptor originalPD )
            : base ( originalPD ) { __targetPD = originalPD; }

        /// <summary>Distruttore.</summary>
        ~WrappedPropertyDescriptor () { __targetPD = null; }


        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override Type ComponentType { get { return __targetPD.ComponentType; } }

        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override Boolean IsReadOnly { get { return __targetPD.IsReadOnly; } }

        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override Type PropertyType { get { return __targetPD.PropertyType; } }


        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <param name="component">Consultare la documentazione dell'implementazione di base.</param>
        /// <returns>Consultare la documentazione dell'implementazione di base.</returns>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override Boolean CanResetValue ( Object component )
        { return __targetPD.CanResetValue ( component ); }

        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <param name="component">Consultare la documentazione dell'implementazione di base.</param>
        /// <returns>Consultare la documentazione dell'implementazione di base.</returns>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override Object GetValue ( Object component )
        { return __targetPD.GetValue ( component ); }

        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <param name="component">Consultare la documentazione dell'implementazione di base.</param>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override void ResetValue ( Object component )
        { __targetPD.ResetValue ( component ); }

        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <param name="component">Consultare la documentazione dell'implementazione di base.</param>
        /// <param name="value">Consultare la documentazione dell'implementazione di base.</param>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override void SetValue ( Object component, Object value )
        { __targetPD.SetValue ( component, value ); }

        /// <summary>Ridefinisce l'implementazione di base del corrispondente membro.</summary>
        /// <param name="component">Consultare la documentazione dell'implementazione di base.</param>
        /// <returns>Consultare la documentazione dell'implementazione di base.</returns>
        /// <remarks>La presente implementazione si limita a delegare al corrispondente membro del
        /// PropertyDescriptor di cui il presente oggetto rapresenta il Wrapper.</remarks>
        public override Boolean ShouldSerializeValue ( Object component )
        { return __targetPD.ShouldSerializeValue ( component ); }


        /// <summary>Il PropertyDescriptor di cui il presente oggetto rappresenta il Wrapper.</summary>
        protected PropertyDescriptor MyOriginalPD
        { get { return __targetPD; } set { __targetPD = value; } }
    } // End of Class WrappedPropertyDescriptor
} // End of Namespace Sharpframework.Core
