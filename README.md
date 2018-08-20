# Sharpframework
An initial version of a smart framework for C# focused on a documental reactive approach based on DDD basics

# Premise
The experience growth in a large set of projects during the last 25 years makes evident that developers sometimes for laziness, sometimes for the appeal for glittered fashions or for  lack of historical perspective 
fall over and over in similar and repetitive bad patterns and habits.
They are mostly  unaware of that due to progressive changes in nomenclature of already known concepts and because every technology or its declination brings something new or better shaped. 
In most of the cases some experienced developers can identify elements and concepts already seen in other projects or that sometimes they have already developed by their self.
At the end some of these elements prove their usefulness, sometimes are abandoned, sometimes just reshaped and represented.
Our general impression is that if we develop enough some technology or architecture to "solve everything" or when we choose that for its spread adoption we introduce new problems in a sort of perverted cycle of already known old issues. 

# Goals

This repository should be a work-in-progress container of knowledge and experience cumulated and distilled in over 25 years of developing practice.

We want to show how it can be useful to respect a set of important principles to implement real-world use cases in the domain
of business and industrial process, making a selection of what we consider of proved value among all the tools, technologies and patterns we have known during our career, distilling them in a low ceremony, lightweight set of tools.

What we want to:
- focus our effort to develop a precise and complete model of a real-world case, typically a business model or a workflow
- give simplified practices for the description of data models as entities and aggregates as per the DDD practice
  - focusing on "documental" approach as it is nearer to real-world document workflow
  - considering in first the humanly readeable documents of a process to keep the development process near to the real world
  - workflows and models should implement in their high level implementation code a specific language near to all the stakeholders (ubiquitus language)
  - recduce or avoid the hassle to maintain ORM for relational storages
  - reduce maintianing cost of the data model and its storage keeping them strictly correspondent to OOP classes design
- use of "events" or "facts" that describe something that has be appended in your model
  - limit or avoid complex services that implement workflows and transactions required by the business model 
  - less dependency from interdisciplinary knowledge of the business 
  - better isolation of possible design erros to their domains
  - every "actor" needs to know less of the whole picture
  - design is implicitly compatible  with reactive architecture ( as every "actor" need just to implement its "reaction" to something happens)
- reduce the number of lines written to implement the model
  - every line is a cost (direct and indirect)
  - every line is a potential failure point 
  - every line slows slightly down the reading and the comprehension of the code
- make the code self-documenting and self-describing 
  - we know that "the truth is in the code"
  - why develop in another form a design just to be implemented in a language that is already complete, descriptive and readable?
  - why document in another form an already readable code that describes the model?
- isolate the code of the "model"  in a highly abstracted way
  - focusing developer on the meaning of the business model
  - give to the project an implicit compatibility to any form of IOC or DI
- hide to developers the infrastructure and the corresponding implementation technologies
- keep decoupled functional domains of the model 
  - every domain can have its specific describing language
  - every domain requires a specific model knowledge


# A tool for every job
    
  - Reducing the data model maintaining cost
  - Make our tool well suited to implement DDD
    - Development of an easy to use Hierarchical document<->c# class hydration/dehydration
    - [Serialization](src/Serialization.md)

## License
Sharpframework is not licensed in any form.
- Sharpframework is a preliminary publish of experimental code for study purpose.
- Any copyright, ownership or right about this code is reserved to authors.
- No derivative projects are allowed without explicit permission.
- No use in real-world business projects or products is allowed.
- You are allowed to read, study and fork this code for private study purpose only.  

## Contributors
Sharpframework is currently a preliminary version, Contributions are welcome just let we know you are interested to participate.
Contributing requires you ask explicitly to participate to projects.

## Copyright
Copyright © 2018 Roberto Duc, Carlo Verano
