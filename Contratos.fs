
module Contratos 
open System


type Zona =  Zona of string

// 
type ContratoTransporte (nemonico, zonaEntrega, CDC, tarifa, factorAgrupado ) = 
        member this.Nemonico: string = nemonico
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



