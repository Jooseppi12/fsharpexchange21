namespace FormsExamples

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI
open WebSharper.UI.Server

type EndPoint =
    | [<EndPoint "/">] SimpleExample
    | [<EndPoint "/order">] OrderExample

module Templating =
    open WebSharper.UI.Html

    // Compute a menubar where the menu item for the given endpoint is active
    let MenuBar (ctx: Context<EndPoint>) endpoint : Doc list =
        let ( => ) txt act =
             li [] [
                a [attr.href (ctx.Link act)] [text txt]
             ]
        [
            "Simple" => EndPoint.SimpleExample
            "Order" => EndPoint.OrderExample
        ]

    let Main ctx action (title: string) (body: Doc list) =
        Content.Page(
            Templates.MainTemplate()
                .Title(title)
                .MenuBar(MenuBar ctx action)
                .Body(body)
                .Doc()
        )

module Site =
    open WebSharper.UI.Html

    let SimplePage ctx =
        Templating.Main ctx EndPoint.SimpleExample "Simple" [
            client <@ Client.SimpleFormRender() @>
        ]

    let OrderPage ctx =
        Templating.Main ctx EndPoint.OrderExample "Order" [
            client <@ Client.OrderFormRender() @>
        ]

    [<Website>]
    let Main =
        Application.MultiPage (fun ctx endpoint ->
            match endpoint with
            | EndPoint.SimpleExample -> SimplePage ctx
            | EndPoint.OrderExample -> OrderPage ctx
        )
