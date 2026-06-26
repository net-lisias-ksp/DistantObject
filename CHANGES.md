# Distant Object Enhancement (DOE) :: Changes

* 2026-0626: 2.2.1.7 (LisiasT) for KSP >= 1.3.1
	+ Small refactoring on the `Settings` infrastructure, saving some sweat from the GC.
	+ Updates `KSPe.Light`, fixing a weird and elusive bug that was hindering the per savegame Settings.
* 2026-0322: 2.2.1.6 (LisiasT) for KSP >= 1.3.1
	+ Updates `KSPe.Light` to 2.5.5.2, fixing a lame mistake that prevented it from working on KSP < 1.8.0 🤦
* 2025-1107: 2.2.1.5 (LisiasT) for KSP >= 1.3.1
	+ Bumping up the release, I botched the last one.
		- Don't ask, I'm sincerely puzzled about how I pulled this fast one on myself.
	+ Since we are here, merging up last minute performance improvements, see PL#60 (Thanks, [@Clayell](https://github.com/Clayell)!)
	+ Merges:
		- [PL#60](https://github.com/net-lisias-ksp/DistantObject/pull/60) Compare transforms instead of `transform.name`
* 2025-0927: 2.2.1.4 (LisiasT) for KSP >= 1.3.1
	+ Maintenance Release: bugfixes and (hopefully) performance enhancements.
	+ Closes issues:
		- [#55](https://github.com/net-lisias-ksp/DistantObject/issues/55) Flare Occlusion Performance Issues.
		- [#54](https://github.com/net-lisias-ksp/DistantObject/issues/54) data is getting screwed now and then when Switching to Map and back
	+ Reworks issues:
		- [#31](https://github.com/net-lisias-ksp/DistantObject/issues/31) Check the SkyBox Dimming when looking on the Planet from it's dark side.
* 2025-0924: 2.2.1.3 (LisiasT) for KSP >= 1.3.1
	+ ***DITCHED*** due a mistake while building it.
* 2025-0213: 2.2.1.2 (LisiasT) for KSP >= 1.3.1
	+ **Finally** implements Sky Undimming when on the dark side of a planet where the Sun is not visible!
	+ Fixes a potential glitch involving Kopernicus, discovered while implementing #31.
	+ Closes issues:
		- [#31](https://github.com/net-lisias-ksp/DistantObject/issues/31) Check the SkyBox Dimming when looking on the Planet from it's dark side.
* 2025-0213: 2.2.1.1 (LisiasT) for KSP >= 1.3.1
	+ ***DITCHED AGAIN***
* 2025-0212: 2.2.1.0 (LisiasT) for KSP >= 1.3.1
	+ ***DITCHED***
* 2024-0803: 2.2.0.2 (LisiasT) for KSP >= 1.3.1
	+ Updates `KSPe.Ligh` to the latest
		- fixes a bug on handling `Regex` on Windows pathnames
	+ Fixes a dumb mistake on customizing a `GUIStyle`.
	+ Add's (transparent) support for Kopernicus
	+ Adds a blacklist to prevent some bodies from being flared (and labeled)
	+ Allows customising the Fly Over Labels for vessels and bodies
	+ Allows per savegame settings
	+ Finally fix that pesky Settings Dialog being too tall.
	+ Closes issues:
		- [#43](https://github.com/net-lisias-ksp/DistantObject/issues/43) The Settings Dialog should not be taller than the screen
		- [#41](https://github.com/net-lisias-ksp/DistantObject/issues/41) Move <KRP_ROOT>/PluginData/DistantObject/Settings.cfg to inside the savegame's directory.
		- [#33](https://github.com/net-lisias-ksp/DistantObject/issues/33) Support Kopernicus
		- [#30](https://github.com/net-lisias-ksp/DistantObject/issues/30) Use the KSP's UI Multiplier Setting on the Labels!
		- [#19](https://github.com/net-lisias-ksp/DistantObject/issues/19) Add a BlackList to prevent undesired bodies from bring flared.
* 2024-0803: 2.2.0.1 (LisiasT) for KSP >= 1.3.1
	+ ***Withdrawed*** due a bug on customising a `GUIStyle`.
* 2024-0801: 2.2.0.0 (LisiasT) for KSP >= 1.3.1
	+ ***Withdrawed*** due a bug on `KSPe`.
