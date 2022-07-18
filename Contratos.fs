
module Contratos 
open System


type Zona =  Zona of string
type Contrato = Contrato of string

// 
type ContratoTransporte (nemonico, zonaEntrega, CDC, tarifa, factorAgrupado ) = 
        member this.Nemonico: Contrato = nemonico
        member this.ZonaEntrega:Zona = zonaEntrega
        member this.CDC: float = CDC
        member this.Tarifa:double = tarifa
        // Rango: 0..100
        // Indica la proporción de la CDC del contrato que se puede
        // agrupar con otro de misma ruta para nominarse en forma
        // proporcional a la CMD
        member this.FactorAgrupado:float = factorAgrupado



type EntregaZona (zona, entrega) =
        member this.Zona:Zona  = zona
        member this.Entrega: float = entrega
    

// transformar los contaros de una zona, en un contato de la misma zona
// con un volumen iguaa a la suma de una proporcion de la CMD de c/u
// el resto de la CMD al precio que ya tienen.

let factorUso = 0.7


// Transformar contratos
let transformar (ctosTte: ContratoTransporte list) =
      let result = Seq.empty<ContratoTransporte>
      let entregaZona = ctosTte |> List.groupBy (fun x -> x.ZonaEntrega) |> List.map (fun x -> fst x, snd x |> List.sumBy(fun z -> z.CDC * z.FactorAgrupado / 100.0))
      // Crear el contrato 'Agrupado' de la zona
      let agrupadosZona = entregaZona |> List.filter (fun ez -> snd ez > 0) 
      agrupadosZona |> List.map (fun x -> fst x,  ContratoTransporte( Contrato(sprintf "Agr%s" (string (fst x))) , (fst x) ,(snd x), 1.0, 0.0 ) )
      // for x in agrupadosZona do  Seq.append result [] 

let toMap (ctosTte : ContratoTransporte seq) =
      ctosTte |> Seq.map (fun x -> x.Nemonico, x) |> Map.ofSeq

