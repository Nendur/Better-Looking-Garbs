#!/bin/bash
if [ "`uname -s`" == "Darwin" ]; then
	MODS="$HOME/Documents/Paradox Interactive/Crusader Kings II/mod/"
else
	MODS="$HOME/.paradoxinteractive/Crusader Kings II/mod/"
fi
BLGDIR="${MODS}betterlookinggarbs"
ADDONS="${MODS}blgsubmods"
BLGZIP="${MODS}betterlookinggarbs.zip"
BLGTMP=`mktemp -d`

RUNDIR="$( dirname "$0" )"
cd "$RUNDIR"
if [ -z "$1" ] || [ "$1" != "-continue" ]; then
	if [ -f betterlookinggarbs/betterlookinggarbsextra.zip ] && [ ! betterlookinggarbs/betterlookinggarbsextra.zip -ef "${BLGDIR}/betterlookinggarbsextra.zip" ]; then
		echo Installing mod files from `pwd` to $MODS
		[ -d "$BLGDIR" -a -d betterlookinggarbs ] && rm -r "$BLGDIR"
		[ -d "$ADDONS" -a -d blgsubmods ] && rm -r "$ADDONS"
		cp -rf betterlookinggarbs betterlookinggarbs.mod blgsubmods "$MODS"
		#cp -f betterlookinggarbsinstall.sh "$ADDONS"
	elif [ -f "$BLGZIP" ]; then
		echo Extracting installer files.
		if [ -z `which unzip` ];then
			echo "unzip command required for extraction."
			exit
		fi
		unzip -uoq "$BLGZIP" betterlookinggarbsextra.zip -d"$BLGTMP"
		#unzip -uoq "$BLGZIP" betterlookinggarbsinstall.sh -d"$BLGTMP"
		#unzip -uoq "$BLGZIP" betterlookinggarbs.fix -d"$MODS"
		#mv -f "$MODS/betterlookinggarbs.fix" "$MODS/betterlookinggarbs.mod"
		#if [ -f "${MODS}/betterlookinggarbs.mod.mod" ]; then
		#	rm "${MODS}/betterlookinggarbs.mod.mod"
		#fi
		#bash "$BLGTMP/betterlookinggarbsinstall.sh" -continue
		#exit
		ADDONS="${MODS}submods"
	fi
fi

if [ -f "${MODS}Historical_Immersion_Project/SWMH_changelog.txt" -a -f "${MODS}Historical_Immersion_Project/EMF_changelog.txt" -a ! -f "${MODS}blgcchip.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgcchip*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgcchip.mod" ]; then
		[ -d "${MODS}/blgcchip" ] && rm -r "${MODS}/blgcchip.mod" "${MODS}/blgcchip"
		mv "${ADDONS}"/blgcchip* "$MODS"
	fi
	echo "HIP compatibility add-on installed."
else
	if [ -f "${MODS}Historical Immersion Project/SWMH_changelog.txt" -o -f "${MODS}Historical_Immersion_Project/SWMH_changelog.txt" -o -f "${MODS}blgccswmh.mod" -a ! -f "${MODS}blgcchip.zip" ]; then
		if [ -f "$BLGZIP" ]; then
			unzip -uoq "$BLGZIP" "submods/blgccswmh*" -d"$MODS"
		fi
		if [ -f "${ADDONS}/blgccswmh.mod" ]; then
			[ -d "${MODS}/blgccswmh" ] && rm -r "${MODS}/blgccswmh.mod" "${MODS}/blgccswmh"
			mv "${ADDONS}"/blgccswmh* "$MODS"
		fi
		echo "HIP SWMH compatibility add-on installed."
	fi
	if [ -f "${MODS}Historical Immersion Project/EMF_changelog.txt" -o -f "${MODS}Historical_Immersion_Project/EMF_changelog.txt" -o -f "${MODS}blgccemf.mod" -a ! -f "${MODS}blgcchip.zip" ]; then
		if [ -f "$BLGZIP" ]; then
			unzip -uoq "$BLGZIP" "submods/blgccemf*" -d"$MODS"
		fi
		if [ -f "${ADDONS}/blgccemf.mod" ]; then
			[ -d "${MODS}/blgccemf" ] && rm -r "${MODS}/blgccemf.mod" "${MODS}/blgccemf"
			mv "${ADDONS}"/blgccemf* "$MODS"
		fi
		echo "HIP EMF compatibility add-on installed."
	fi
fi
if [ -d "${MODS}CK2Plus" -o -f "${MODS}ck2plus.zip" -o -f "${MODS}blgccck2+.mod" -a ! -f "${MODS}blgccck2+.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccck2+*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccck2+.mod" ]; then
		[ -d "${MODS}/blgccck2+" ] && rm -r "${MODS}/blgccck2+.mod" "${MODS}/blgccck2+"
		mv "${ADDONS}"/blgccck2+* "$MODS"
	fi
	echo "CK2 Plus compatibility add-on installed."
fi
if [ -d "${MODS}A Game of Thrones" -o -f "${MODS}a game of thrones.zip" -o -f "${MODS}blgccagot.mod" -a ! -f "${MODS}blgccagot.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccagot*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccagot.mod" ]; then
		[ -d "${MODS}/blgccagot" ] && rm -r "${MODS}/blgccagot.mod" "${MODS}/blgccagot"
		[ -d "${MODS}/blgagot" ] && rm -r "${MODS}/blgagot.mod" "${MODS}/blgagot"
		mv "${ADDONS}"/blgccagot* "$MODS"
	fi
	echo "A Game of Thrones compatibility add-on installed."
fi
if [ -d "${MODS}ElderKings" -o -f "${MODS}ek021.zip" -o -f "${MODS}blgccelderkings.mod" -a ! -f "${MODS}blgccelderkings.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccelderkings*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccelderkings.mod" ]; then
		[ -d "${MODS}/blgccelderkings" ] && rm -r "${MODS}/blgccelderkings.mod" "${MODS}/blgccelderkings"
		mv "${ADDONS}"/blgccelderkings* "$MODS"
	fi
	echo "Elder Kings compatibility add-on installed."
fi
if [ -d "${MODS}Faerun" -o -f "${MODS}faerun.zip" -o -f "${MODS}blgccfaerun.mod" -a ! -f "${MODS}blgccfaerun.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccfaerun*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccfaerun.mod" ]; then
		[ -d "${MODS}/blgccfaerun" ] && rm -r "${MODS}/blgccfaerun.mod" "${MODS}/blgccfaerun"
		mv "${ADDONS}"/blgccfaerun* "$MODS"
	fi
	echo "Faerûn compatibility add-on installed."
fi
if [ -d "${MODS}WTWSMS" -o -f "${MODS}wtwsms.zip" -o -f "${MODS}blgccwtwsms.mod" -a ! -f "${MODS}blgccwtwsms.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccwtwsms*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccwtwsms.mod" ]; then
		[ -d "${MODS}/blgccwtwsms" ] && rm -r "${MODS}/blgccwtwsms.mod" "${MODS}/blgccwtwsms"
		mv "${ADDONS}"/blgccwtwsms* "$MODS"
	fi
	echo "WTWSMS compatibility add-on installed."
fi
if [ -d "${MODS}Britannia" -o -f "${MODS}britannia.zip" -o -f "${MODS}blgccwinterking.mod" -a ! -f "${MODS}blgccwinterking.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccwinterking*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccwinterking.mod" ]; then
		[ -d "${MODS}/blgccwinterking" ] && rm -r "${MODS}/blgccwinterking.mod" "${MODS}/blgccwinterking"
		mv "${ADDONS}"/blgccwinterking* "$MODS"
	fi
	echo "The Winter King compatibility add-on installed."
fi
if [ -d "${MODS}Lux Invicta" -o -f "${MODS}Lux Invicta.mod" -o -f "${MODS}blgccluxinvicta.mod" -a ! -f "${MODS}blgccluxinvicta.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccluxinvicta*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccluxinvicta.mod" ]; then
		[ -d "${MODS}/blgccluxinvicta" ] && rm -r "${MODS}/blgccluxinvicta.mod" "${MODS}/blgccluxinvicta"
		mv "${ADDONS}"/blgccluxinvicta* "$MODS"
	fi
	echo "Lux Invicta compatibility add-on installed."
fi
if [ -d "${MODS}Tianxia" -o -f "${MODS}Tianxia.mod" -o -f "${MODS}blgcctianxia.mod" -a ! -f "${MODS}blgcctianxia.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgcctianxia*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgcctianxia.mod" ]; then
		[ -d "${MODS}/blgcctianxia" ] && rm -r "${MODS}/blgcctianxia.mod" "${MODS}/blgcctianxia"
		mv "${ADDONS}"/blgcctianxia* "$MODS"
	fi
	echo "Tianxia compatibility add-on installed."
fi
if [ -f "${MODS}mythos3.zip" -o -f "${MODS}mythos3.mod" -o -d "${MODS}Mythos 2" -o -f "${MODS}Mythos2.mod" -o -f "${MODS}blgccmythos.mod" -a ! -f "${MODS}blgccmythos.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccmythos*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccmythos.mod" ]; then
		[ -d "${MODS}/blgccmythos" ] && rm -r "${MODS}/blgccmythos.mod" "${MODS}/blgccmythos"
		mv "${ADDONS}"/blgccmythos* "$MODS"
	fi
	echo "Mythos compatibility add-on installed."
fi
if [ -f "${MODS}aftertheendfanfork.zip" -o -f "${MODS}AfterTheEndFanFork.mod" -o -f "${MODS}blgccaftertheend.mod" -a ! -f "${MODS}blgccaftertheend.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccaftertheend*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccaftertheend.mod" ]; then
		[ -d "${MODS}/blgccaftertheend" ] && rm -r "${MODS}/blgccaftertheend.mod" "${MODS}/blgccaftertheend"
		mv "${ADDONS}"/blgccaftertheend* "$MODS"
	fi
	echo "After the End compatibility add-on installed."
fi
if [ -d "${MODS}Rise to Power" -o -f "${MODS}risetopower.zip" -o -f "${MODS}blgrisetopower.mod" -a ! -f "${MODS}blgrisetopower.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgrisetopower*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgrisetopower.mod" ]; then
		[ -d "${MODS}/blgrisetopower" ] && rm -r "${MODS}/blgrisetopower.mod" "${MODS}/blgrisetopower"
		mv "${ADDONS}"/blgrisetopower* "$MODS"
	fi
	echo "Rise to Power compatibility add-on installed."
fi
if [ -d "${MODS}Dark World Reborn" -o -f "${MODS}Dark World Reborn.mod" -o -d "${MODS}LuxuriaFantasia" -o -f "${MODS}LuxuriaFantasia.mod" -o -f "${MODS}blgdarkworld.mod" -a ! -f "${MODS}blgdarkworld.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgdarkworld*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgdarkworld.mod" ]; then
		[ -d "${MODS}/blgdarkworld" ] && rm -r "${MODS}/blgdarkworld.mod" "${MODS}/blgdarkworld"
		mv "${ADDONS}"/blgdarkworld* "$MODS"
	fi
	echo "Dark World compatibility add-on installed."
fi
if [ -f "${MODS}blgccironman.mod" -o -n "`grep ironman=yes \"${MODS}/../settings.txt\"`" -a ! -f "${MODS}blgccironman.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccironman*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccironman.mod" ]; then
		[ -d "${MODS}/blgccironman" ] && rm -r "${MODS}/blgccironman.mod" "${MODS}/blgccironman"
		mv "${ADDONS}"/blgccironman* "$MODS"
	fi
	echo "Ironman add-on installed."
fi
if [ -f "${MODS}blgccgeneric.mod" -a ! -f "${MODS}blgccgeneric.zip" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgccgeneric*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgccgeneric.mod" ]; then
		[ -d "${MODS}/blgccgeneric" ] && rm -r "${MODS}/blgccgeneric.mod" "${MODS}/blgccgeneric"
		mv "${ADDONS}"/blgccgeneric* "$MODS"
	fi
	echo "Generic compatibility add-on installed."
fi
if [ -f "${MODS}blgnodisease.mod" ]; then
	if [ -f "$BLGZIP" ]; then
		unzip -uoq "$BLGZIP" "submods/blgnodisease*" -d"$MODS"
	fi
	if [ -f "${ADDONS}/blgnodisease.mod" ]; then
		[ -d "${MODS}/blgnodisease" ] && rm -r "${MODS}/blgnodisease.mod" "${MODS}/blgnodisease"
		mv "${ADDONS}"/blgnodisease* "$MODS"
	fi
	echo "Vanilla disease graphics add-on installed."
fi

if [ -n "$1" ] && [ -d "$1/dlc" ]; then
	CK2=$1
elif [ -n "$2" ] && [ -d "$2/dlc" ]; then
	CK2=$2
else
	if [ "`uname -s`" == "Darwin" ]; then
		BLGSTEAM="${HOME}/Library/Application Support/Steam/SteamApps/"
	elif [ -d "${HOME}/.steam/steam/SteamApps" ]; then
		BLGSTEAM="${HOME}/.steam/steam/SteamApps/"
	else
		BLGSTEAM="${HOME}/.steam/steam/steamapps/"
	fi
	echo Searching "${BLGSTEAM}common/Crusader Kings II"
	if [ -d "${BLGSTEAM}common/Crusader Kings II" ]; then
		CK2="${BLGSTEAM}common/Crusader Kings II"
	elif [ -r "${BLGSTEAM}libraryfolders.vdf" ]; then
		for i in `grep '^\s*\"path\"\s*\".*\"' "${BLGSTEAM}libraryfolders.vdf"`; do
			if [ $i != '"path"' ]; then
				searchpath=${i#\"}
				searchpath=${searchpath%\"}
				echo Searching $searchpath/steamapps/common/Crusader Kings II
				if [ -d "$searchpath/steamapps/common/Crusader Kings II" ]; then
					CK2="$searchpath/steamapps/common/Crusader Kings II"
				fi
			fi
		done
	fi
fi
if [ -z "$CK2" ]; then
	echo The Crusader Kings II installation directory was not found.
	echo Start the installation script with the correct path as the argument to continue the installation.
	exit
fi
echo Crusader Kings II installation found in $CK2

DLC="$CK2/dlc/dlc"
LIST=""
#First pass
if [[ -f "${DLC}014.dlc" ]] || [[ -f "${DLC}047.dlc" ]]; then
	LIST="$LIST 1dlc014odlc047/*"
fi
if [ -f "${DLC}016.dlc" ]; then
	LIST="$LIST 1dlc016/*"
	russian=1
fi
if [ -f "${DLC}020.dlc" ]; then
	LIST="$LIST 1dlc020/*"
	norse=1
fi
if [ -f "${DLC}063.dlc" ]; then
	conclave=1
	LIST="$LIST 1dlc063/*"
fi
if [ -f "${DLC}074.dlc" ]; then
	holyfury=1
	LIST="$LIST 1dlc074/*"
fi
#Second pass
if [ -f "${DLC}047.dlc" ]; then
	earlyeast=1
	LIST="$LIST 2dlc047/*"
fi
if [ -f "${DLC}067.dlc" ]; then
	reapers=1
	LIST="$LIST 2dlc067/*"
fi
if [ -f "${DLC}070.dlc" ]; then
	monks=1
	LIST="$LIST 2dlc070/*"
fi
#Third pass
if [ -f "${DLC}045.dlc" ]; then
	charlemagne=1
	LIST="$LIST 3dlc045/*"
fi

#Main pass
if [ -f "${DLC}002.dlc" ]; then
	echo Mongol Face Pack present.
	LIST="$LIST dlc002/*"
	mongol=1
	if [ -f "${DLC}016.dlc" ]; then
		LIST="$LIST dlc002adlc016/*"
	fi
else echo Mongol Face Pack absent.; fi

if [ -f "${DLC}008.dlc" ]; then
	LIST="$LIST dlc008/*"
fi

if [ -f "${DLC}011.dlc" ]; then
	echo Legacy of Rome present.
	LIST="$LIST dlc011/*"
else echo Legacy of Rome absent.; fi
if [ -f "${DLC}012.dlc" ]; then
	LIST="$LIST dlc012/*"
fi

if [ -f "${DLC}013.dlc" ]; then
	echo African Portraits present.
	LIST="$LIST dlc013/*"
	westafrican=1
else echo African Portraits absent.; fi

if [ -f "${DLC}014.dlc" ]; then
	echo Mediterranean Portraits present.
	LIST="$LIST dlc014/* dlc014odlc047/*"
	byzantine=1
else echo Mediterranean Portraits absent.; fi

if [ -f "${DLC}015.dlc" ]; then
	LIST="$LIST dlc015/*"
	[ $charlemagne -eq 1 ] && LIST="$LIST dlc015adlc045/*"
fi
if [[ $russian -eq 1 ]]; then
	echo Russian Portraits present.
	LIST="$LIST dlc016/*"
	[[ $byzantine -eq 1 ]] && LIST="$LIST dlc014adlc016/*"
else echo Russian Portraits absent.; fi

if [ -f "${DLC}018.dlc" ]; then
	echo Sunset Invasion present.
	LIST="$LIST dlc018/*"
else echo Sunset Invasion absent.; fi

if [[ $norse -eq 1 ]]; then
	echo Norse Portraits present.
	LIST="$LIST dlc020/*"
else echo Norse Portraits absent.; fi
if [ -f "${DLC}021.dlc" ]; then
	LIST="$LIST dlc021/*"
fi

if [ -f "${DLC}022.dlc" ]; then
	LIST="$LIST dlc022/*"
	if [ -f "${DLC}063.dlc" ]; then
		LIST="$LIST dlc022adlc063/*"
	fi
fi
if [ -f "${DLC}024.dlc" ]; then
	echo The Old Gods present.
	LIST="$LIST dlc024/*"
else echo The Old Gods absent.; fi

if [ -f "${DLC}027.dlc" ]; then
	LIST="$LIST dlc027/*"
fi
if [ -f "${DLC}028.dlc" ]; then
	echo Celtic Portraits present.
	celtic=1
	LIST="$LIST dlc028/*"
else echo Celtic Portraits absent.; fi

if [ -f "${DLC}033.dlc" ]; then
    holy=1
	LIST="$LIST dlc033/*"
fi
if [ -f "${DLC}034.dlc" ]; then
	echo Warriors of Faith Unit Pack present.
	LIST="$LIST dlc034/*"
	[ -f "${DLC}040.dlc" ] && LIST="$LIST dlc034adlc040/*"
else echo Warriors of Faith Unit Pack absent.; fi

if [ -f "${DLC}037.dlc" ]; then
	LIST="$LIST dlc037/*"
fi
if [ -f "${DLC}038.dlc" ]; then
	LIST="$LIST dlc038/*"
	if [ -f "${DLC}015.dlc" ]; then
		LIST="$LIST dlc038adlc015/*"
	fi
fi

if [ -f "${DLC}039.dlc" ]; then
	echo Rajas of India present.
	indian=1
	LIST="$LIST dlc039/*"
else echo Rajas of India absent.; fi

if [ -f "${DLC}040.dlc" ]; then
	LIST="$LIST dlc040/*"
fi
if [ -f "${DLC}041.dlc" ]; then
	echo Turkish Portraits present.
	LIST="$LIST dlc041/*"
else echo Turkish Portraits absent.; fi

if [[ -f "${DLC}043.dlc" ]] && [[ $indian -eq 1 ]]; then
	LIST="$LIST dlc043adlc039/*"
fi
if [ -f "${DLC}044.dlc" ]; then
	echo Persian Portraits present.
	persian=1
	LIST="$LIST dlc044/*"
else
	echo Persian Portraits absent.
	#if [[ $byzantine -eq 1 ]] || [[ -f "${DLC}047.dlc" ]]; then
	#	LIST="$LIST dlc044rdlc014odlc047/*"
	#fi
fi

if [[ $charlemagne -eq 1 ]]; then
	echo Charlemagne present.
	if [[ -z $holy ]]; then
		LIST="$LIST dlc033rdlc045/*"
	fi
	[ -f "${DLC}040.dlc" ] && LIST="$LIST dlc045adlc040/*"
	[ -f "${DLC}051.dlc" ] && LIST="$LIST dlc045adlc051/*"
else echo Charlemagne absent.; fi

if [ -f "${DLC}046.dlc" ]; then
	echo Early Western Clothing Pack present.
	LIST="$LIST dlc046/*"
	if [[ $byzantine -eq 1 ]]; then
		LIST="$LIST dlc046adlc014/*"
	fi
else echo Early Western Clothing Pack absent.; fi
if [[ $earlyeast -eq 1 ]]; then
	echo Early Eastern Clothing Pack present.
	LIST="$LIST dlc047/*"
	[ -z $byzantine ] && LIST="$LIST dlc014odlc047/*"
else echo Early Eastern Clothing Pack absent.; fi

if [ -f "${DLC}052.dlc" ]; then
	echo Iberian Portraits present.
	southern=1
	LIST="$LIST dlc052/*"
	if [[ -f "${DLC}046.dlc" ]]; then
		LIST="$LIST dlc046adlc052/*"
	fi
else
	echo Iberian Portraits absent.;
fi

if [ -f "${DLC}057.dlc" ]; then
	echo Cuman Portraits present.
	horselords=1
	LIST="$LIST dlc057/*"
	if [[ ! -f "${DLC}041.dlc" ]]; then
		LIST="$LIST dlc041rdlc057/*"
	fi
else
	echo Cuman Portraits absent.
fi
if [[ -f "${DLC}058.dlc" ]] && [[ -z $holy ]]; then
	LIST="$LIST dlc033rdlc058/*"
fi
if [[ -f "${DLC}058.dlc" ]] && [[ $charlemagne -eq 1 ]]; then
	LIST="$LIST dlc045adlc058/*"
fi
if [ -f "${DLC}060.dlc" ]; then
	LIST="$LIST dlc060/*"
fi

if [[ $conclave -eq 1 ]]; then
	echo Conclave Content Pack present.
	LIST="$LIST dlc063/*"
else
	echo Conclave Content Pack absent.
	if [[ $celtic -eq 1 ]] && [[ $mongol -eq 1 ]] && [[ $norse -eq 1 ]]; then
		LIST="$LIST dlc063rdlc002adlc020adlc028/*"
	fi
fi

if [[ -f "${DLC}065.dlc" ]] || [[ -f "${DLC}072.dlc" ]]; then
	echo South Indian Portraits present.
	LIST="$LIST dlc065odlc072/*"
	if [ -z $indian ]; then
		LIST="$LIST dlc039rdlc065odlc072/*"
	else
		LIST="$LIST dlc039adlc065odlc072/*"
	fi
else echo South Indian Portraits absent.; fi

if [[ $reapers -eq 1 ]]; then
	echo "The Reaper's Due Content Pack present."
	LIST="$LIST dlc067/*"
else
	echo "The Reaper's Due Content Pack absent."
	if [[ $westafrican -eq 1 ]]; then
		LIST="$LIST dlc067rdlc013/*"
	fi
fi

if [[ $monks -eq 1 ]]; then
	echo "Monks and Mystics Content Pack present."
	LIST="$LIST dlc070/*"
else
	echo "Monks and Mystics Content Pack absent."
	if [ -f "${DLC}037.dlc" ]; then
		LIST="$LIST dlc070rdlc037/*"
	fi
	if [[ $charlemagne -eq 1 ]]; then
		LIST="$LIST dlc070rdlc045/*"
	fi
	if [ -f "${DLC}059.dlc" ]; then
		LIST="$LIST dlc070rdlc059/*"
	fi
fi

if [ -f "${DLC}073.dlc" ]; then
	echo "Jade Dragon present."
	LIST="$LIST dlc073/*"
	if [[ $horselords -eq 1 ]]; then
		LIST="$LIST dlc073adlc057/*"
	fi
else
	echo "Jade Dragon absent."
fi

if [[ $holyfury -eq 1 ]]; then
	echo "Holy Fury present."
	LIST="$LIST dlc074/*"
	if [[ $russian -eq 1 ]]; then
		LIST="$LIST dlc074adlc016/*"
	fi
	if [ -f "${DLC}046.dlc" ]; then
		LIST="$LIST dlc074adlc046/*"
	fi
else
	echo "Holy Fury absent."
	#if [[ $monks -eq 1 ]]; then
	#	LIST="$LIST dlc074rdlc070/*"
	#fi
fi

if [ -z "$LIST" ]; then
	echo Better Looking Garbs installation completed without installing DLC dependent files.
else
	if [ -f "${BLGTMP}/betterlookinggarbsextra.zip" ]; then
		EXTRAZIP="${BLGTMP}/betterlookinggarbsextra.zip"
	elif [ -f betterlookinggarbsextra.zip ]; then
		EXTRAZIP="${PWD}/betterlookinggarbsextra.zip"
	elif [ -f "${BLGDIR}/betterlookinggarbsextra.zip" ]; then
		EXTRAZIP="${BLGDIR}/betterlookinggarbsextra.zip"
	elif [ -f "${MODS}/betterlookinggarbsextra.zip" ]; then
		EXTRAZIP="${MODS}/betterlookinggarbsextra.zip"
	else
		echo "The betterlookinggarbsextra.zip file could not be found."
		exit
	fi
	echo Extracting $LIST from $EXTRAZIP
	if [ -z `which unzip` ];then
		echo "unzip command required for extraction."
		exit
	fi
	cd $BLGTMP
	unzip -quoPhr37yw3tre4gg84y "$EXTRAZIP" $LIST
	#Alternate 7z x -Phr37yw3tre4gg84y "$EXTRAZIP" $LIST
	echo Finished extractions.
fi

#Combining files
mkdir merge
for dir in *dlc*
do
	if [ -d $dir ]; then
		cd $dir
		cp -rf * ../merge
		cd ..
	fi
done
cd merge

if [ -f "$BLGZIP" ]; then
	if [ -z `which zip` ];then
		echo "zip command required for merging."
		exit
	fi
	zip -rq "$BLGZIP" *
	zip -qd "$BLGZIP" common/on_actions/BLG_installcheck.txt
	echo Merged with betterlookinggarbs.zip
fi
if [ -d "$BLGDIR" ]; then
	cp -rf * "$BLGDIR"
	if [ -f "$BLGDIR/common/on_actions/BLG_installcheck.txt" ];then
		rm "$BLGDIR/common/on_actions/BLG_installcheck.txt"
	fi
	echo Merged with betterlookinggarbs
fi

cd "$MODS"
rm -r "$BLGTMP"
echo Better Looking Garbs installation complete.
