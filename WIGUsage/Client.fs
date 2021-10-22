namespace WIGUsage

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WIG

[<JavaScript>]
module Client =
    // The templates are loaded from the DOM, so you just can edit index.html
    // and refresh your browser, no need to recompile unless you add or remove holes.
    type IndexTemplate = Template<"wwwroot/index.html", ClientLoad.FromDocument>

    [<SPAEntryPoint>]
    let Main () =
        IndexTemplate.Main()
            .BottomLeft(fun _ ->
                WIG.Toastify(
                    ToastifyConfig(
                        Position = ToastifyPosition.Left,
                        Gravity = ToastifyGravity.Bottom,
                        Text = "I should be at the bottom-left corner"
                    ))
                    .ShowToast()
            )
            .Callback(fun _ ->
                WIG.Toastify(
                    ToastifyConfig(
                        Duration = 10000,
                        Close = true,
                        Callback = (fun _ -> JS.Alert("Toastify closed")),
                        Text = "I should have a close icon and a callback provided"
                    ))
                    .ShowToast()
            )
            .Destination(fun _ ->
                WIG.Toastify(
                    ToastifyConfig(
                        Destination = "https://google.com",
                        NewWindow = true,
                        Text = "I should go to google.com"
                    ))
                    .ShowToast()
            )
            .Doc()
        |> Doc.RunById "main"
