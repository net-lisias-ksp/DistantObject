#!/usr/bin/env bash

source ./CONFIG.inc

check() {
	for d in . $KSP_DEV ; do
		if [ ! -d "${d}/GameData/${TARGETBINDIR}/" ] ; then
			rm -f "${d}/GameData/${TARGETBINDIR}/"
		fi
		mkdir -p "${d}/GameData/${TARGETBINDIR}"
		if [[ ! -z "${PD_DLLS}" ]] ; then
			mkdir -p "${d}/GameData/${TARGETBINDIR}/PluginData/"
		fi
		if [[ ! -z "${PD_SUB_DIRS}" ]] ; then
			for dd in ${PD_SUB_DIRS} ; do
				mkdir -p "${d}/GameData/${TARGETBINDIR}/PluginData/${dd}/"
			done
		fi
		rm -f ${d}/GameData/${TARGETBINDIR}/KSPe.Light.*
	done

	if [[ -d "./bin/Release" && -d "./bin/Debug" ]] ; then
		echo "Conflicting Release and Debug dirs. Clean and rebuild!"
		exit -1
	fi

	if [[ ! -d "./bin/Release" && ! -d "./bin/Debug" ]] ; then
		echo "Absent Release and Debug dirs. Nothingn to do! Rebuild!"
		exit -2
	fi
}

deploy() {
	local DLL=$1.dll

	if [ -f "./bin/Release/${DLL}" ] ; then
		cp "./bin/Release/${DLL}" "./GameData/${TARGETBINDIR}/"
		if [ -d "${KSP_DEV}/GameData/${TARGETBINDIR}/" ] ; then
			cp "./bin/Release/${DLL}" "${KSP_DEV}/GameData/${TARGETBINDIR}/"
		fi
	fi
	if [ -f "./bin/Debug/${DLL}" ] ; then
		if [ -d "${KSP_DEV}/GameData/${TARGETBINDIR}/" ] ; then
			cp "./bin/Debug/${DLL}" "${KSP_DEV}/GameData/${TARGETBINDIR}/"
		fi
	fi
}

deploy_lib() {
	local DLL=$1.dll

	if [ -f "./bin/Release/${DLL}" ] ; then
		cp "./bin/Release/${DLL}" "${LIB}"
	fi
}

deploy_plugindata() {
	local DLL=$1.dll

	if [ -f "./bin/Release/${DLL}" ] ; then
		for d in . ${KSP_DEV} ; do
			cp "./bin/Release/${DLL}" "${d}/GameData/${TARGETBINDIR}/PluginData"
		done
	fi
	if [ -f "./bin/Debug/${DLL}" ] ; then
		if [ ! -z ${KSP_DEV} ] ; then
			cp "./bin/Debug/${DLL}" "${KSP_DEV}/GameData/${TARGETBINDIR}/PluginData"
		fi
	fi
}

deploy_plugindata_sub() {
	local DLL=$1.dll
	local TARGET=${PD_SUB_RULES[$1]}.dll

	if [ -f "./bin/Release/${DLL}" ] ; then
		for d in . ${KSP_DEV} ; do
			cp "./bin/Release/${DLL}" "${d}/GameData/${TARGETBINDIR}/PluginData/${TARGET}"
		done
	fi
	if [ -f "./bin/Debug/${DLL}" ] ; then
		if [ ! -z ${KSP_DEV} ] ; then
			cp "./bin/Debug/${DLL}" "${KSP_DEV}/GameData/${TARGETBINDIR}/PluginData/${TARGET}"
		fi
	fi
}

deploy_gamedata() {
	local PLACE=$1
	local DLL=$2.dll

	if [ -f "./bin/Release/${DLL}" ] ; then
		for d in . ${KSP_DEV} ; do
			cp "./bin/Release/${DLL}" "${d}/GameData/${PLACE}_${DLL}"
		done
	fi
	if [ -f "./bin/Debug/${DLL}" ] ; then
		if [ ! -z ${KSP_DEV} ] ; then
			cp "./bin/Debug/${DLL}" "${KSP_DEV}/GameData/${PLACE}_$DLL"
		fi
	fi
}

VERSIONFILE=${PACKAGE}.version

check
cp $VERSIONFILE "./GameData/${TARGETDIR}"
cp CHANGE_LOG.md "./GameData/${TARGETDIR}"
cp README.md  "./GameData/${TARGETDIR}"
cp LICENSE* "./GameData/${TARGETDIR}"
cp NOTICE "./GameData/${TARGETDIR}"


for dll in $LIB_DLLS ; do
    deploy_lib $dll
done

for dll in $DLLS ; do
    deploy $dll
done

for dll in $PD_DLLS ; do
    deploy_plugindata $dll
done

for dll in ${!PD_SUB_RULES[@]} ; do
    deploy_plugindata_sub $dll
done

for dll in $GD_DLLS ; do
    deploy_gamedata $GD_PRIORITY $dll
done

echo "${VERSION} Deployed into ${KSP_DEV}"
