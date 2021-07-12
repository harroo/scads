# varibales
buildpath = build/Output.exe

sourcepath = src/*.cs

dependsargs = -r:lib/DSharpPlus.dll -r:lib/Newtonsoft.Json.dll

# compile
output: $(sourcepath)
	mcs $(dependsargs) $(sourcepath) -out:$(buildpath)

# clean
clean:
	rm $(buildpath)

# run
run:
	mono $(buildpath)
