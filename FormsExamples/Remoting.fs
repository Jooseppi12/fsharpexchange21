namespace FormsExamples

open WebSharper
open WebSharper.Forms

module Server =

    [<Rpc>]
    let Validate (input: string) =
        async {
            if input.StartsWith "a" then
                return WebSharper.Forms.Result.Success ()
            else
                return Result.Failure [ErrorMessage.Create("invalidUsername", "Username must start with a")]
        }
