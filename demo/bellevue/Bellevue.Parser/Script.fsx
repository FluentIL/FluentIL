// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "parser.fs"
open Bellevue.Parser

// Define your library scripting code here

TextParser.Parse("This is a @hello() 
@section hello
")

