namespace mindTheApp

open System

module parser =
  open System.Text.RegularExpressions

  type PResult<'t,'s>(result : Choice<'t,string list>, state: 's) =
    class 
      member this.Result with get() = result
      member this.State with get() = state
      member this.Map f = PResult(f result,state)
    end

  type parser<'t,'s> = {
      Apply : 's -> PResult<'t,'s>
  } with
    member o.Bind(p' : 't -> parser<'x,'s>) =
      {
          Apply = fun s ->
              let r = o.Apply s
              match r.Result with
              | Choice1Of2 v -> r.State |> (p' v).Apply
              | Choice2Of2 e -> PResult<_,_>(Choice2Of2 e,r.State)
      }

  let mkParser f = {Apply = f}

  let (>>=) (m : parser<_,_>) p =
    m.Bind(p)

  let (>>>) m p = m >>= fun _ -> p

  let pConst x = mkParser <| fun input -> PResult(Choice1Of2 x, input)

  let (|>>) p f =
    p >>= fun x -> f x |> pConst

  let (<*) p1 p2 = p1 >>= fun x -> p2 >>> pConst x

  let pMay p = mkParser <| fun input ->
    let res = p.Apply input
    res.Map <| fun r -> match r with
      | Choice1Of2 x -> Some x |> Choice1Of2
      | Choice2Of2 _ -> Choice1Of2 None

  type Parsers =

    static member PStr c = mkParser <| function (input : string) ->
      if input.StartsWith c then
        PResult(Choice1Of2 c,input.Substring(c.Length))
      else
        PResult(Choice2Of2 [sprintf "Input %s dosen't match %s" input c],input.Substring(input.Length))

    static member PAnyChar = mkParser <| function (input : string) ->
      if input.Length > 0 then
        PResult(input.[0] |> Choice1Of2,input.Substring(1))
      else
        PResult(Choice2Of2 ["Input is empty"],input.Substring(input.Length))

    static member PReg re = mkParser <| fun (input : string) ->
      let m = Regex.Matches(input,re) // .Match(input,re)
      if m.Count > 0 then
        PResult(Choice1Of2 m.[0].Value,input.Substring(m.[0].Value.Length))
      else
        let msg = sprintf "Input %s did not match re '%s'" input re
        PResult(Choice2Of2 [msg],input)

    static member PSpc = Parsers.PReg "\\s"

    static member PInt =
      Parsers.PReg "\\d+" |>> Int32.Parse

    static member PBetween(delim1,delim2,p) =
      delim1 >>> p >>= fun r -> delim2 >>> pConst r

    static member PBetween(delim1,delim2,p) =
      Parsers.PBetween(Parsers.PStr delim1,Parsers.PStr delim2,p)

    static member PManyMon(zero,op,p) =
      pMay p >>= fun v -> match v with
        | Some x -> Parsers.PManyMon(zero,op,p) |>> (fun ps -> op x ps)
        | None -> pConst zero

    static member PMany(p) = Parsers.PManyMon([],(fun x xs -> x::xs),p)

    static member PMany(p) = Parsers.PManyMon("",(fun (x : string) xs -> x + xs),p)

    static member PManyS(p) = Parsers.PManyMon("",(fun (x : char) xs -> x.ToString() + xs),p)

module logParser =

  open parser

  type P = Parsers

  let pActivity =
    P.PStr "I/ActivityManager"
    >>> P.PBetween("(",")",P.PSpc >>> P.PInt <* P.PSpc)
    >>= (fun aid -> P.PStr ":" >>> P.PManyS(P.PAnyChar) |>> fun (x:string) -> aid,x)
