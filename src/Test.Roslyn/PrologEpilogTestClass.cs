using System;


namespace Test.Roslyn
{
    //[MethodsProlog ( nameof ( PrologEpilogTestClass._Prolog ) )]
    [MethodsProlog ( "_Prolog" )]
    [MethodsEpilog ( "_Epilog" )]
    public class PrologEpilogTestClass
    {
        public static void Pluto ()
        {
            object s = "abc";
            Int32  j = 12;

            for ( Int32 k = 0 ; k < 10 ; k++ )
            {
                s += j++.ToString ();
            }
        }

        [MethodsProlog ( "_PrologBis" )]
        [MethodsEpilog ( "_EpilogBis" )]
        public void Pippo ()
        {
        }

        public void Paperino ()
        {
        }

        private static void _Epilog ()
        {
        }

        private static void _Prolog ()
        {
        }

        private static void _EpilogBis ()
        {
        }

        private static void _PrologBis ()
        {
        }
    }
}
