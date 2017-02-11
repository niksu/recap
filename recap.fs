(*
Replaying the contents of a pcap file over a specified network port.
Nik Sultana, February 2017

Use of this source code is governed by the Apache 2.0 license; see LICENSE.

NOTE the technique used here to space packets wrt the original timing is VERY crude.
*)

module recap

open System.Threading
open SharpPcap
open Pax
open pcap

type Recap (pcap_filename : string, out_port : int) as this =
  inherit Pax.ByteBased_PacketProcessor ()
  let mutable now_ms : uint64 = 0uL
  let pcap_contents : pcap_file_contents = pcap.deserialise_pcap pcap_filename
  override this.process_packet (in_port : int, packet : byte[]) = ()
  interface IActive with
    member this.Start () =
      printfn "Starting to play the pcap file"
      List.iter (fun (pfp : pcap_file_packet) ->
        (*FIXME we lose timing resolution by going from microsec to millisec*)
        let packet_now_ms =
          ((1000000uL * uint64 pfp.header.ts_sec) + uint64 pfp.header.ts_usec) / 1000uL
        if now_ms <> 0uL then
          Thread.Sleep (int (packet_now_ms - now_ms))
        now_ms <- packet_now_ms
        printfn "Sent packet"
        this.send_packet (out_port, pfp.data, int pfp.header.incl_len))
        (pcap_contents.packets : pcap_file_packet list)
      Frontend.shutdown()
    member this.PreStart (device : ICaptureDevice) =
      (*FIXME could keep a local list of registered devices, and broadcast to all of them, rather than to out_port*)
      ()
    member this.Stop () = ()