#!/usr/bin/env bash

source ./CONFIG.inc

clean() {
	local DLL=$1.dll

	for d in . ${KSP_DEV} ; do
		rm -f "${d}/GameData/$DLL"
		rm -f "${d}/GameData/$TARGETBINDIR/$DLL"
		rm -f "${d}/GameData/$TARGETBINDIR/PluginData/$DLL"
		for dd in $PD_SUB_DIRS ; do
			rm -f "${d}/GameData/$TARGETBINDIR/PluginData/${dd}/$DLL"
		done
	done
}

clean_lib() {
	local DLL=$1.dll

	rm -f "$LIB/$DLL"
}

VERSIONFILE=$PACKAGE.version

rm -fR "./bin"
rm -fR "./obj"
rm -f "./GameData/$TARGETDIR/$VERSIONFILE"
rm -f "./GameData/$TARGETDIR/CHANGE_LOG.md"
rm -f "./GameData/$TARGETDIR/README.md"
rm -f "./GameData/$TARGETDIR/LICENSE*"
find "./GameData/$TARGETBINDIR" -name "KSPe.Light.*.dll" -delete
for dll in 000_KSPe $DLLS $PD_DLLS KSPe.UI ; do
    clean $dll
done
for dll in $LIB_DLLS ; do
    clean_lib $dll
done
