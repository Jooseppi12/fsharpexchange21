namespace JavaScriptInterop

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    module Toast =
        // Basic approach, handle the input as an object
        [<Inline "Toastify($x).showToast()">]
        let showToast1 (x: obj) = X<unit>

        // Configuration type for Toastify
        type ToastConfig =
            {
                duration: int
                text: string
            }

        // Pass in a typed configuration option for the constructor parameter
        [<Inline "Toastify($x).showToast()">]
        let showToast2 (x: ToastConfig) = X<unit>


    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate.Main()
            .Example1(fun _ ->
                Toast.showToast1 (New [ "text", "Hello world!" :>_ ])
            )
            .Example2(fun _ ->
                Toast.showToast2 {duration = 4000; text = "Hello world from example2"}
            )
            .Doc()
        |> Doc.RunById "main"
