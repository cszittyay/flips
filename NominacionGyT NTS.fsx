
#r "nuget: Flips,2.4.10"
fsi.ShowDeclarationValues <- false

open Flips
open Flips.Types
open Flips.UnitsOfMeasure
open Flips.SliceMap

type [<Measure>] USD
type [<Measure>] GJ

type Contrato = Contrato of string

type ContratoGas = {
    Contrato: Contrato
    PuntoRX: string
    CMD: float<GJ>
    Precio: float
}

type ContratoTransporte = {
    Contrato: Contrato
    PuntoRX: string
    PuntoEX: string
    CMD: float
    Precio: float
}

type Entrega = {
    PuntoEX: string
    Entrega: float
}


// Declare the parameters for our model
let puntosRX = ["V061"; "V33"; "V40"]
let puntosEX = ["E01"; "E02"; "E03"]

let contratosGas = 
    [
        ({Contrato = Contrato "C1"; PuntoRX = "V061"; CMD = 100.0<GJ>; Precio = 2.50});
        ({Contrato = Contrato "C2"; PuntoRX = "V33"; CMD = 120.0<GJ>; Precio = 2.60});
        ({Contrato = Contrato "C3"; PuntoRX = "V40"; CMD = 130.0<GJ>; Precio = 2.55})
    ]

let dContratosGas = contratosGas |> List.map (fun c -> c.Contrato, c) |> Map

let contratosTransporte = 
    [
        ({Contrato = Contrato "T1"; PuntoRX = "V061"; PuntoEX = "E01"; CMD = 100.0; Precio = 2.50});
        ({Contrato = Contrato "T2"; PuntoRX = "V33"; PuntoEX = "E02"; CMD = 120.0; Precio = 2.60});
        ({Contrato = Contrato "T3"; PuntoRX = "V40"; PuntoEX = "E03"; CMD = 130.0; Precio = 2.55});

    ]
    

let entregas = 
    [
        { PuntoEX = "E01"; Entrega = 100.0 }
        { PuntoEX = "E02"; Entrega = 150.0 }
        { PuntoEX = "E03"; Entrega = 200.0 }
    ]



// Create Decisions for each puntoRX and puntoEX using a DecisionBuilder
// Turn the result into a `SMap2`   
let nomGas =        
    DecisionBuilder<GJ> "NomGas" {
        for cto in dContratosGas.Keys  -> Continuous (0.<GJ>, dContratosGas.[cto].CMD)
    } |> SMap.ofSeq


//// Create Decisions for each puntoEX using a DecisionBuilder
//// Turn the result into a `SMap2`   
//let nomTransporte =        
//    DecisionBuilder<GJ> "NomTransporte" {
//        for cto in contratosTransporte do
//            Continuous (0.0<GJ>, cto.CMD<GJ>)
//    } |> SMap2.ofSeq
