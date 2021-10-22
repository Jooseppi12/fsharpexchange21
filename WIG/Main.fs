namespace WIG

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

module Definition =
    let ToastifyGravity =
        Pattern.EnumStrings "ToastifyGravity" ["top"; "bottom"]
    let ToastifyPosition =
        Pattern.EnumStrings "ToastifyPosition" ["left"; "right"]

    let ToastifyConfig =
        Pattern.Config "ToastifyConfig"
            {
                Required = []
                Optional = [
                    "text", T<string> // Text content for Toastify
                    "node", T<Dom.Element> // Dom.Element to attach as Toastify content
                    "duration", T<int> // Duration in milliseconds
                    "destination", T<string> // Url to navigate to upon click
                    "newWindow", T<bool> // Decides whether the destination should be opened in a new window
                    "close", T<bool> // Control to show a close icon
                    "gravity", ToastifyGravity.Type // Toast location vertically
                    "position", ToastifyPosition.Type // Toast location horizontally
                    "callback", T<unit> ^-> T<unit> // Callback when toast notification disappears
                    "style", T<obj> // Style object for Toastify
                ]
            }

    let Toastify =
        Class "Toastify"
        |+> Instance [
            // property
            "version" =? T<string>
            // 
            "showToast" => T<unit> ^-> T<unit>
            "hideToast" => T<unit> ^-> T<unit>
        ]
        |+> Static [
            Constructor (ToastifyConfig)        
        ]

    let Assembly =
        Assembly [
            Namespace "WIG" [
                 Toastify
                 ToastifyConfig
                 ToastifyGravity
                 ToastifyPosition
            ]
            Namespace "WIG.Resources" [
                Resource "ToastifyJS" "https://cdn.jsdelivr.net/npm/toastify-js"
                |> AssemblyWide
                Resource "ToastifyCSS" "https://cdn.jsdelivr.net/npm/toastify-js/src/toastify.min.css"
                |> AssemblyWide
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member ext.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
