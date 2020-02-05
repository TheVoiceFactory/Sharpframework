# Markdown file


IPlayer AddAReactionTo(FactSequence seq, Reaction r, optional Filter f)

ReactionsDictionary

FactSequence => enumerabile di Fact 


IFactBinder AddFactBinding ( IFact fact, delegate d, Ifact Emitted, optional Filter filter )

IPlayer AddReactionTo ( IFactBinderSequence seq )


Reaction (f1, optional n(f1))
    has an ID (deletion, setting)

IPlayer p = new Player ()
                .AddFactBinding (
                    new FactBinding ( f1, d1 ),
                .AddFactBinding (
                    new FactBinding ( f2, d2 )
                )
               )

SetOfPlayers esegue la Player.Submit (singleFactwithItsContext)

la Playet.Submit registra il fatto e processa i binding 
Il player ha una coda interna di fatti singoli

il palyer in fase di creazione dovrebbe dichiarare i fatti emessi 

