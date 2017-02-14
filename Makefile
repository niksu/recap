# Replaying the contents of a pcap file over a specified network port.
# Nik Sultana, February 2017

recap.dll : recap.fs
	fsharpc --out:$@ --target:library $(KNEECAP_DIR)/kneecap/pcap.fs $< --lib:$(PAX)/Bin/ \
		--reference:Pax.exe --reference:SharpPcap.Dll --reference:PacketDotNet.Dll
