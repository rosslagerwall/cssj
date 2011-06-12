SplitJoin.exe: SplitJoin.cs
	gmcs -out:SplitJoin.exe SplitJoin.cs

clean:
	rm -f SplitJoin.exe

.PHONY: clean
