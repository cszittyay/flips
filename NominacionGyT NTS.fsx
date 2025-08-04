
#r "nuget: Flips,2.4.10"
fsi.ShowDeclarationValues <- false


open Flips
open Flips.Types
open Flips.SliceMap
open Flips.UnitsOfMeasure


open System.Diagnostics
Debugger.Break()

type [<Measure>] USD
type [<Measure>] GJ

type Punto = Punto of string
type Contrato = Contrato of string

type ContratoGas = {
    Contrato: Contrato
    PuntoRX: Punto
    CMD: float<GJ>
    Precio: float<USD/GJ>
}

type ContratoTransporte = {
    Contrato: Contrato
    PuntoRX: Punto
    PuntoEX: Punto
    CMD: float<GJ>
    Precio: float<USD/GJ>
}

type Entrega = {
    PuntoEX: Punto
    Entrega: float<GJ>
}


// Declare the parameters for our model
let puntosRX = [Punto "V061"; Punto "V33"; Punto "V40"]
let puntosEX = [Punto "E01"; Punto "E02"; Punto "E03"]

let contratosGas = 
    [
        ({Contrato = Contrato "C1"; PuntoRX = Punto "V061"; CMD = 100.0<GJ>; Precio = 2.50<USD/GJ>});
        ({Contrato = Contrato "C2"; PuntoRX = Punto "V33"; CMD = 120.0<GJ>; Precio = 2.60<USD/GJ>});
        ({Contrato = Contrato "C3"; PuntoRX = Punto "V40"; CMD = 130.0<GJ>; Precio = 2.55<USD/GJ>})
    ]

let dContratosGas = contratosGas |> List.map (fun c -> c.Contrato, c) |> Map

let dCtoGasPrecio = contratosGas |> List.map (fun c -> c.Contrato, c.Precio) |> SMap

let contratosTransporte = 
    [
        ({Contrato = Contrato "T1"; PuntoRX = Punto "V061"; PuntoEX = Punto "E01"; CMD = 100.0<GJ>; Precio = 2.50<USD/GJ>});
        ({Contrato = Contrato "T1"; PuntoRX = Punto "V33"; PuntoEX = Punto "E02"; CMD = 120.0<GJ>; Precio = 2.60<USD/GJ>});
        ({Contrato = Contrato "T1"; PuntoRX = Punto "V40"; PuntoEX = Punto "E03"; CMD = 130.0<GJ>; Precio = 2.55<USD/GJ>});

    ]
    
let dContratosTransporte = contratosTransporte |> List.map (fun c -> (c.Contrato, c.PuntoEX), c.CMD) |> SMap2.ofList

let dCtoTtePrecio = contratosTransporte |> List.map (fun c -> c.Contrato, c.Precio) |> SMap


let entregas = 
    [
        { PuntoEX = Punto "E01"; Entrega = 100.0<GJ> }
        { PuntoEX = Punto "E02"; Entrega = 150.0<GJ> }
        { PuntoEX = Punto "E03"; Entrega = 200.0<GJ> }
    ]

let dEntregas = entregas |> List.map (fun c -> c.PuntoEX, c.Entrega) |> SMap


// Create Decisions for each puntoRX and puntoEX using a DecisionBuilder
// Turn the result into a `SMap2`   
let nomGas =        
    DecisionBuilder<GJ> "NomGas" {
        for cto in dContratosGas.Keys  -> Continuous (0.<GJ>, dContratosGas.[cto].CMD)
    } |> SMap.ofSeq


let nomTte =        
    DecisionBuilder<GJ> "NomTte" {
        for (cto,pto) in dContratosTransporte.Keys   -> Continuous (0.<GJ>, dContratosTransporte.[cto, pto])
    } |> SMap2.ofSeq


// Create the Linear Expression for the objective
let objectiveExpression = sum (dCtoGasPrecio .* nomGas)


let objective = Objective.create "Costo Gas" Minimize objectiveExpression


// Agregar las restricciones


// Create a Constraints for the Max combined weight of items for each Location
let entregaConstraint =
    ConstraintBuilder "Entrega" {
        for pto in puntosEX ->
            sum(nomTte.[All, pto]) == dEntregas.[pto]
    }


let model =
    Model.create objective
    |> Model.addConstraints entregaConstraint


let result = Solver.solve Settings.basic model

printfn "-- Result --"


// Match the result of the call to solve
// If the model could not be solved it will return a `Suboptimal` case with a message as to why
// If the model could be solved, it will print the value of the Objective Function and the
// values for the Decision Variables
match result with
| Optimal solution ->
    printfn "Objective Value: %f" (Objective.evaluate solution objective)

    let snomGas = Solution.getValues solution nomGas
    let snomTte = Solution.getValues solution nomTte

    for ctoG in  snomGas.Keys do
        printfn "Item: %s\tValue: %f" (string ctoG) snomGas.[ctoG]

    printfn $"\nNominación Transporte:"

    for (ctoT,pto) in  snomTte.Keys do
        printfn "Cto: %s\tPto: %s\tValue: %f" (string ctoT) (string pto) snomTte.[ctoT,pto]

| _ -> printfn $"Unable to solve. Error: %A{result}"



