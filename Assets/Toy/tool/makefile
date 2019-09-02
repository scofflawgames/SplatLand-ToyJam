#config
ifeq ($(OS),Windows_NT)
CSC=csc
else
CSC=mcs #mono
endif

#output
OUTDIR=../out

#targets
all: generate_ast

generate_ast:
ifeq ($(OS),Windows_NT)
	$(CSC) -out:$(OUTDIR)/generate_ast.exe generate_ast.cs
	$(OUTDIR)/generate_ast.exe ../src
else
	$(CSC) -out:$(OUTDIR)/generate_ast generate_ast.cs
	mono $(OUTDIR)/generate_ast ../src
endif

clean:
ifeq ($(OS),Windows_NT)
	$(RM) *.o *.a *.exe
else ifeq ($(shell uname), Linux)
	find . -type f -name *.o -exec rm -f -r -v {} \;
	find . -type f -name *.a -exec rm -f -r -v {} \;
endif

rebuild: clean all
