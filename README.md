# Recap: replaying a pcap file over the network
This is an example of writing a non-interactive element on [Pax](https://github.com/niksu/pax).
It also serves as an example of how to write Pax elements in F#.

## Building
1. set the environment variable `KNEECAP_DIR` to point to your clone of
   [Kneecap](https://github.com/niksu/kneecap)'s repo, and `PAX` to point to
   your clone of [Pax](https://github.com/niksu/pax).
2. ensure that you've built Pax, and that its dependencies are satisfied.
3. run `make`.

## Configuration
Edit the file [recap.json](recap.json) to specify the pcap file you want played,
and the logical port you want it played over.

## Running
`sh -e run.sh`.
