#source
SRC=$(wildcard *.cs)

#config
ifeq ($(OS),Windows_NT)
CSC=csc
else
CSC=mcs
endif

#output
ifeq ($(OS),Windows_NT)
OUTFILE=interpreter.exe
else
OUTFILE=interpreter
endif
OUTDIR=../out
OUT=$(addprefix $(OUTDIR)/,$(OUTFILE))

#targets
all:
	$(CSC) -out:$(OUT) $(SRC)

clean:
ifeq ($(OS),Windows_NT)
	$(RM) *.o *.a *.exe
else ifeq ($(shell uname), Linux)
	find . -type f -name *.o -exec rm -f -r -v {} \;
	find . -type f -name *.a -exec rm -f -r -v {} \;
endif

rebuild: clean all
