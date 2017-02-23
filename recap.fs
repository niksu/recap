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
  let mutable count = 0
  let pcap_contents : pcap_file_contents = pcap.deserialise_pcap pcap_filename
  override this.process_packet (in_port : int, packet : byte[]) = ()

  interface IActive with
    member this.Start () =
      printfn "Playing the pcap file"
      List.iter (fun (pfp : pcap_file_packet) ->
        (*FIXME we lose timing resolution by going from microsec to millisec*)
        let packet_now_ms =
          ((1000000uL * uint64 pfp.header.ts_sec) + uint64 pfp.header.ts_usec) / 1000uL
        if now_ms <> 0uL then
          Thread.Sleep (int (packet_now_ms - now_ms))
        now_ms <- packet_now_ms
        count <- count + 1
        printfn "Sent packet %d of %d (%d bytes)"
          count (List.length pcap_contents.packets) (int pfp.header.incl_len)

        if PaxConfig.opt_verbose then
          Array.iter (fun (b : byte) ->
            let s : string =
              if b > 31uy && b < 127uy then
                (* FIXME crude coding style -- avoid forming singleton array from "b"*)
                System.Text.Encoding.ASCII.GetString([|b|])
              else "."
            System.Console.Write(s)) pfp.data
          System.Console.WriteLine()

        this.send_packet (out_port, pfp.data, int pfp.header.incl_len))
        (pcap_contents.packets : pcap_file_packet list)
      Frontend.shutdown()
    member this.PreStart (device : ICaptureDevice) =
      (*FIXME could keep a local list of registered devices, and broadcast to all of them, rather than to out_port*)
      ()
    member this.Stop () = ()

  interface IVersioned with
    member this.expected_major_Pax_version = 0
    member this.expected_minor_Pax_version = 2
    member this.expected_patch_Pax_version = 0
