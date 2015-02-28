namespace mindTheApp

open System

module parser =

    type PResult<'t,'s>(result : Choice<'t,string list>, state: 's) =
        class 
            member this.Result with get() = result
            member this.State with get() = state
        end

    type parser<'t,'s> = {
        Apply : 't -> PResult<'t,'s>
    } with
        member o.Bind(p') =
            {
                Apply = fun s ->
                    let r = o.Apply s
                    match r.Result with
                    | Choice1Of2 r -> p' r
                    | _ -> r
            }


    "I/ActivityManager(  747):"

    type Parser() = class end 
