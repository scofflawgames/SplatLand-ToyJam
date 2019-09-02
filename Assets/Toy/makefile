#Windows 7:
#RM=del /y

#Windows 8.1:
#RM=del /S

OUTDIR=out

all: $(OUTDIR) tools
	$(MAKE) -C src

tools: $(OUTDIR)
	$(MAKE) -C tool

$(OUTDIR):
	mkdir $(OUTDIR)

debug: clean all
release: clean all
rebuild: clean all

clean:
ifeq ($(OS),Windows_NT)
	del /S /Q *.o *.a *.exe
#	rmdir /S /Q $(OUTDIR)
else ifeq ($(shell uname), Linux)
	find . -type f -name '*.o' -exec rm -f -r -v {} \;
	find . -type f -name '*.a' -exec rm -f -r -v {} \;
#	rm $(OUTDIR)/* -f
	find . -empty -type d -delete
endif
