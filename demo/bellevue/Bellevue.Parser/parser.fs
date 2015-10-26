
namespace Bellevue.Parser

type Tokens =
    | Literal of string
    | Formula of string 

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

    let rec parse acc chars = seq {
        let emitLiteral() = seq {
            if acc <> [] then 
                yield acc |> List.rev |> toString |> Literal
        }

        let emitFormula body = seq {
            if body <> [] then 
                yield body |> toString |> Formula
        }

        match chars with 
        | Bracketed ['@'; '*' ] ['*'; '@' ] (body, chars) ->
            yield! emitLiteral() 
            yield! parse [] chars
        | Bracketed ['@'; '{' ] ['}'] (body, chars) ->
            yield! emitLiteral()
            yield! emitFormula body 
            yield! parse [] chars
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
