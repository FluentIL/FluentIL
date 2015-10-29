
namespace Bellevue.Parser

type Tokens =
    | Literal of string
    | Formula of string
    | Assignment of string * string
    member x.TryLiteral(literal:string byref) = 
        match x with
        | Literal(l) -> literal <- l; true
        | _ -> false
    member x.TryFormula(formula:string byref) = 
        match x with
        | Formula(f) -> formula <- f; true
        | _ -> false
    member x.TryAssignment(variable: string byref, value: string byref) =
        match x with 
        | Assignment(var, v) -> variable <- var; value <- v; true
        | _ -> false
        

module private Util =
    let toString chars =
       System.String(chars |> Array.ofList)

    let (|StartsWith|_|) prefix list =
        let rec loop = function
            | [], rest -> Some(rest)
            | p::prefix, r::rest when p = r -> loop (prefix, rest)
            | _ -> None
        loop (prefix, list)

    let rec parseBracketedBody closing acc = function
        | StartsWith closing (rest) -> Some(List.rev acc, rest)
        | c::chars -> parseBracketedBody closing (c::acc) chars
        | _ -> None

    let (|Bracketed|_|) opening closing = function
        | StartsWith opening chars -> parseBracketedBody closing [] chars
        | _ -> None
  
    let (|Delimited|_|) delim = (|Bracketed|_|) delim delim

    let (|Id|_|) chars =
        let (|StartsWithAlpha|_|) = function
            | x::xs when System.Char.IsLetter(x) || x = '_' 
                -> Some (x, xs)
            | _ -> None
        
        let (|LettersDigitsUnderlines|_|) chars = 
            let rec loop acc chars = 
                match chars with 
                | x::xs when System.Char.IsLetterOrDigit(x) || x = '_'
                    -> loop (x::acc) xs
                | _ -> 
                    match acc with 
                    | [] -> Some([], chars)
                    | _ -> Some((acc |> List.rev), chars)
            loop [] chars

        match chars with
        | StartsWithAlpha (c, LettersDigitsUnderlines(d, body)) ->
            Some ((c::d |> toString), body)
        | _ -> None

    let (|Equals|_|) chars = 
        let rec loop = function
            | ' '::xs -> (loop xs)
            | '='::xs -> Some(xs)
            | _ -> None
        loop chars
    
    let (|Ignorables|) chars =
        let rec loop = function
            | ' '::xs -> (loop xs)
            | '\t'::xs -> (loop xs)
            | xs -> xs
        loop chars

    let rec parse acc chars = seq {
        let emitLiteral() = seq {
            if acc <> [] then 
                yield acc |> List.rev |> toString |> Literal
        }

        let rec parseInstructions chars = seq { 
            let emitFormula body = seq {
                if body <> [] then 
                    yield body |> toString |> Formula
            }

            let (|Instruction|_|) chars = 
                let rec loop acc chars =
                    match chars with 
                    | x::xs when x = ';' || x = '\n' -> 
                        if acc <> [] 
                        then Some((acc |> List.rev), xs)
                        else loop [] xs
                    | x::xs -> loop (x::acc) xs
                    | [] ->
                        if acc <> [] 
                        then Some((acc |> List.rev), [])
                        else None
                    | _ -> None
                loop [] chars

            match chars with 
            | Instruction(instruction, body) ->
                match instruction with 
                | Ignorables(Id(variable, Equals(value))) -> 
                    yield Assignment(variable, value |> toString)
                | _ -> yield! emitFormula instruction

                yield! parseInstructions body
            | _ -> ()
        }
        
        match chars with 
        | Bracketed ['@'; '*' ] ['*'; '@' ] (body, xs) ->
            yield! emitLiteral() 
            yield! parse [] xs
        | Bracketed ['@'; '{' ] ['}'] (body, xs) ->
            yield! emitLiteral()
            yield! parseInstructions body
            yield! parse [] xs
        | '@'::Id(id, xs) ->
            yield! emitLiteral()
            yield Formula(id)
            yield! parse [] xs
        | h::t -> 
            yield! parse (h::acc) t
        | [] ->
            yield! emitLiteral()
    }

module TextParser = 
    open Util

    let Parse input = 
        input 
        |> List.ofSeq
        |> parse [] 
