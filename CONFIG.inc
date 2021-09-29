SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
source ${SCRIPT_DIR}/../CONFIG.inc

PACKAGE=DistantObject
TARGETDIR="$PACKAGE"
TARGETBINDIR="$TARGETDIR/"
PROJECTSDIR="$PACKAGE/"

DLLS="DistantObject"
PD_DLLS=""
declare -A PD_SUB_RULES=( )
PD_SUB_DLLS=`{ for K in "${!PD_SUB_RULES[@]}"; do echo $(basename ${PD_SUB_RULES[$K]}); done } | sort -u | tr '\n' ' '`
PD_SUB_DIRS=`{ for K in "${!PD_SUB_RULES[@]}"; do echo $(dirname ${PD_SUB_RULES[$K]}); done } | sort -u | tr '\n' ' '`
GD_DLLS=""
GD_PRIORITY="000"
LIB_DLLS="${DLLS}"

PROJECT_BRANCH=`git branch | grep \* | cut -d ' ' -f2`
if [ "$PROJECT_BRANCH" == "mestre" ] ; then
	# Release mode
	PROJECT_STATE=""
else
	# Note: Add a leading dash when this value is present!
	PROJECT_STATE=""
fi

VERSION=$( cat $PACKAGE.version | tr '\n' ' ' | sed -n -E 's/^.*?"VERSION\":\{.*"MAJOR":([0-9]+?),.*?"MINOR":([0-9]+?),.*?"PATCH":([0-9]+?),.*?"BUILD":([0-9]+?).*?\}.*$/\1.\2.\3.\4/p' )
