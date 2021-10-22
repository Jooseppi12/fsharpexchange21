namespace FormsExamples

open WebSharper
open WebSharper.Forms
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Templating
open WIG
open System

[<JavaScript>]
module Templates =
    type MainTemplate = Templating.Template<"Main.html", ClientLoad.FromDocument, ServerLoad.WhenChanged>

[<JavaScript>]
module Examples =
    let e1<'T> : Form<(string * string), (Var<string> -> Var<string> -> 'T) -> 'T> =
        Form.Return (fun i1 i2 -> (i1, i2))
        <*> Form.Yield ""
        <*> Form.Yield ""

[<JavaScript>]
module Client =
    type SimpleRecord =
        {
            Username: string
            Password: string
        }
    let SimpleForm () =
        Form.Return (fun username password -> {Username=username; Password=password} )
        <*> (Form.Yield "" |> Validation.IsNotEmpty "Username must not be empty")
        <*> (Form.Yield ""
            |> Validation.IsNotEmpty "Password must not be empty"
            |> Validation.IsMatch ".*[A-Z]+.*" "Password must contain an uppercase letter"
            |> Validation.IsMatch ".*[0-9]+.*" "Password must contain a number")
        |> Form.WithSubmit
        |> Form.MapToAsyncResult (fun user -> 
            async {
                let! res = Server.Validate user.Username
                match res with
                | Success _ ->
                    WIG.Toastify(
                        ToastifyConfig(
                            Text="Successful!",
                            Close = true,
                            Style = New ["background", "green" :>_]
                        )).ShowToast()
                | Failure _ ->
                    WIG.Toastify(
                        ToastifyConfig(
                            Text="Failure!",
                            Close = true,
                            Style = New ["background", "red" :>_]
                        )).ShowToast()
                return res
            }
        )
        |> Form.TransmitView

    let SimpleFormRender () =
        SimpleForm ()
        |> Form.Render (fun user pass submitter res ->
            Templates.MainTemplate.SimpleForm()
                .Username(user)
                .Password(pass)
                .Trigger(fun _ -> submitter.Trigger())
                .Errors(
                    res
                    |> Doc.BindView (fun errors ->
                        match errors with
                        | Result.Success _ -> Doc.Empty
                        | Result.Failure errors ->
                            errors
                            |> List.map (fun x ->
                                p [attr.``class`` "error"] [text x.Text]
                            )
                            |> Doc.Concat
                    )
                )
                .Doc()
        )

    type OrderType =
        | Gift of string
        | ReplacementItem of string

        override this.ToString() =
            match this with
            | Gift r ->
                if r |> String.IsNullOrEmpty then
                    ""
                else
                    sprintf "gifting it to %s" <| r.Trim()
            | ReplacementItem i ->
                if i |> String.IsNullOrEmpty then
                    ""
                else
                    sprintf "replaceable with %s" <| i.Trim()
    
    let OrderItem() =
        Form.Return (fun item quantity orderType -> (item, quantity, orderType))
        <*> (Form.Yield ""
            |> Validation.IsNotEmpty "Please enter an item name.")
        <*> (Form.Yield 0
            |> Validation.Is (fun x -> x > 0) "Number of items to order must be greater than 0")
        <*> Form.Do {
            let! isGift = Form.Yield true
            if isGift then
                return! Form.Yield ""
                    |> Form.Map (OrderType.Gift)
            else
                return!
                    Form.Yield ""
                    |> Form.Map (OrderType.ReplacementItem)
        }
        |> Form.WithSubmit
    
    let OrderForm() =
        Form.ManyForm Seq.empty (OrderItem()) Form.Yield
        |> Validation.Is (not << Seq.isEmpty) "Please enter at least one item."
        |> Form.WithSubmit
        |> Form.Run (fun items ->
            JS.Alert (items |> Seq.map (fun (item, q, orderType) -> sprintf "Ordered item: %s x %d" item q) |> String.concat "\n")
        )

    let OrderFormRender() =
        OrderForm()
        |> Form.Render(fun items submitter ->
            div [attr.style "display: flex; flex-direction: row;"] [
                div [attr.style "flex: 1;"] [
                    table [attr.style "display: table; width: 100%;"] [
                        thead [] [
                            tr [] [
                                th [] [text "Item"]
                                th [] [text "Quantity"]
                                th [] [text "Note"]
                                th [] [text "Operation"]
                            ]
                        ]
                        tbody [] [
                            items.Render (fun ops x ->
                                tr [] [
                                    td [] [textView (x.View |> View.Map (fun (x, _, _) -> x))]
                                    td [] [textView (x.View |> View.Map (fun (_, x, _) -> string x))]
                                    td [] [textView (x.View |> View.Map (fun (_, _, x) -> string x))]
                                    td [] [button [on.click (fun _ _ -> ops.Delete())] [text "Delete"]]
                                ]
                            )
                        ]
                    ]
                    Doc.Button "Submit" [] submitter.Trigger
                ]
                div [attr.style "border: solid 1px #888; padding: 10px; margin: 20px; width: 500px;"] [
                    h3 [] [text "Add a new item"]
                    items.RenderAdder (fun item quantity itemType submit ->
                        div [] [
                            p [] [
                                Doc.Input [attr.placeholder "ItemName"] item
                            ]
                            p [] [
                                Doc.IntInputUnchecked [] quantity
                            ]
                            p [] [
                                itemType.RenderPrimary (fun orderType ->
                                    div [] [
                                        label [] [text "Is it a gift?"]
                                        Doc.CheckBox [] orderType 
                                    ]
                                )
                                itemType.RenderDependent (fun orderTypeItem ->
                                    Doc.Concat [
                                        Doc.Input [
                                            attr.placeholderDyn
                                                <| itemType.View.Map(fun isGift ->
                                                    match isGift with
                                                    | Result.Success (Gift _) ->
                                                        "Recipient"
                                                    | Result.Success (ReplacementItem _) ->
                                                        "Replacement item"
                                                    | _ ->
                                                        ""
                                                )
                                        ] orderTypeItem
                                    ]
                                )
                            ]
                            p [] [Doc.Button "Add" [] submit.Trigger]
                            submit.View.ShowErrors(fun errors ->
                                errors
                                |> List.map (fun x ->
                                    p [attr.``class`` "error"] [text x.Text]
                                )
                                |> Doc.Concat
                            )
                        ]
                    )
                ]
            ]
        )