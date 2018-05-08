#!/usr/bin/dotnet /usr/share/dotnet/sdk/2.1.300-rc1-008673/FSharp/fsi.exe

// Hm, printfn "plain string" ist a type error ...
let greeting = "Hello, World!"
    in
    printfn "%s" greeting

for arg in fsi.CommandLineArgs do
    printfn "%s" arg

/// You use 'let' to define a function. This one accepts an integer argument and returns an integer. 
/// Parentheses are optional for function arguments, except for when you use an explicit type annotation.
let sampleFunction1 x = x*x + 3

/// Apply the function, naming the function return result using 'let'. 
/// The variable type is inferred from the function return type.
let result1 = sampleFunction1 4573

// This line uses '%d' to print the result as an integer. This is type-safe.
// If 'result1' were not of type 'int', then the line would fail to compile.
printfn "The result of squaring the integer 4573 and adding 3 is %d" result1

/// When needed, annotate the type of a parameter name using '(argument:type)'.  Parentheses are required.
let sampleFunction2 (x:int) = 2*x*x - x/5 + 3

let result2 = sampleFunction2 (7 + 4)
printfn "The result of applying the 2nd sample function to (7 + 4) is %d" result2

/// Conditionals use if/then/elif/else.
///
/// Note that F# uses whitespace indentation-aware syntax, similar to languages like Python.
let sampleFunction3 x = 
    if x < 100.0 then 
        2.0*x*x - x/5.0 + 3.0
    else 
        2.0*x*x + x/5.0 - 37.0

let result3 = sampleFunction3 (6.5 + 4.5)

// This line uses '%f' to print the result as a float.  As with '%d' above, this is type-safe.
printfn "The result of applying the 2nd sample function to (6.5 + 4.5) is %f" result3
;;

module Immutability =
    /// Binding a value to a name via 'let' makes it immutable.
    ///
    /// The second line of code fails to compile because 'number' is immutable and bound.
    /// Re-defining 'number' to be a different value is not allowed in F#.
    let number = 2
    // let number = 3

    /// A mutable binding.  This is required to be able to mutate the value of 'otherNumber'.
    let mutable otherNumber = 2

    printfn "'otherNumber' is %d" otherNumber

    // When mutating a value, use '<-' to assign a new value.
    //
    // Note that '=' is not the same as this.  '=' is used to test equality.
    otherNumber <- otherNumber + 1

    printfn "'otherNumber' changed to be %d" otherNumber
 ;;

 module IntegersAndNumbers = 

    /// This is a sample integer.
    let sampleInteger = 176

    /// This is a sample floating point number.
    let sampleDouble = 4.1

    /// This computed a new number by some arithmetic.  Numeric types are converted using
    /// functions 'int', 'double' and so on.
    let sampleInteger2 = (sampleInteger/4 + 5 - 7) * 4 + int sampleDouble

    /// This is a list of the numbers from 0 to 99.
    let sampleNumbers = [ 0 .. 99 ]

    /// This is a list of all tuples containing all the numbers from 0 to 99 and their squares.
    let sampleTableOfSquares = [ for i in 0 .. 99 -> (i, i*i) ]

    // The next line prints a list that includes tuples, using '%A' for generic printing.
    printfn "The table of squares from 0 to 99 is:\n%A" sampleTableOfSquares
 ;;

 module Booleans =

    /// Booleans values are 'true' and 'false'.
    let boolean1 = true
    let boolean2 = false

    /// Operators on booleans are 'not', '&&' and '||'.
    let boolean3 = not boolean1 && (boolean2 || false)

    // This line uses '%b'to print a boolean value.  This is type-safe.
    printfn "The expression 'not boolean1 && (boolean2 || false)' is %b" boolean3
 ;;

 module StringManipulation = 

    /// Strings use double quotes.
    let string1 = "Hello"
    let string2  = "world"

    /// Strings can also use @ to create a verbatim string literal.
    /// This will ignore escape characters such as '\', '\n', '\t', etc.
    let string3 = @"C:\Program Files\"

    /// String literals can also use triple-quotes.
    let string4 = """The computer said "hello world" when I told it to!"""

    /// String concatenation is normally done with the '+' operator.
    let helloWorld = string1 + " " + string2 

    // This line uses '%s' to print a string value.  This is type-safe.
    printfn "%s" helloWorld

    /// Substrings use the indexer notation.  This line extracts the first 7 characters as a substring.
    /// Note that like many languages, Strings are zero-indexed in F#.
    let substring = helloWorld.[0..6]
    printfn "%s" substring
 ;;

 module Tuples =

    /// A simple tuple of integers.
    let tuple1 = (1, 2, 3)

    /// A function that swaps the order of two values in a tuple. 
    ///
    /// F# Type Inference will automatically generalize the function to have a generic type,
    /// meaning that it will work with any type.
    let swapElems (a, b) = (b, a)

    printfn "The result of swapping (1, 2) is %A" (swapElems (1,2))

    /// A tuple consisting of an integer, a string,
    /// and a double-precision floating point number.
    let tuple2 = (1, "fred", 3.1415)

    printfn "tuple1: %A\ttuple2: %A" tuple1 tuple2
 ;;

/// Tuples are normally objects, but they can also be represented as structs.
///
/// These interoperate completely with structs in C# and Visual Basic.NET; however,
/// struct tuples are not implicitly convertible with object tuples (often called reference tuples).
///
/// The second line below will fail to compile because of this.  Uncomment it to see what happens.
let sampleStructTuple = struct (1, 2)
//let thisWillNotCompile: (int*int) = struct (1, 2)

// Although you can
let convertFromStructTuple (struct(a, b)) = (a, b)
let convertToStructTuple (a, b) = struct(a, b)

printfn "Struct Tuple: %A\nReference tuple made from the Struct Tuple: %A" sampleStructTuple (sampleStructTuple |> convertFromStructTuple)
;;
