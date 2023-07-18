# Distant Object Enhancement (DOE) :: Changes

* 2023-0718: 2.1.1.13 (LisiasT) for KSP >= 1.3.1
	+ An embarrassing memory leak was detected and fixed.
* 2023-0421: 2.1.1.12 (LisiasT) for KSP >= 1.3.1
	+ "Dumb mistakes fixed" edition.
	+ Closes issues:
		- [#34](https://github.com/net-lisias-ksp/DistantObject/issues/34) Hundreds of errors in KSP.log
		- [#25](https://github.com/net-lisias-ksp/DistantObject/issues/25) Rework the Settings.
		- [#14](https://github.com/net-lisias-ksp/DistantObject/issues/14) Disabling "changeSkybox" non functional
		- [#1](https://github.com/net-lisias-ksp/DistantObject/issues/1)
* 2022-1114: 2.1.1.11 (LisiasT) for KSP >= 1.3.1
	+ Brings to mainstream the Experimental features from 2.1.1.10
		- Fixes the Sky dimming for the Sun
		- Implements some parameters to customise the dimming for planets.
	+ (formally) Work issues:
		- [#23](https://github.com/net-lisias-ksp/DistantObject/issues/23) Parameterise the FoV of the celestial body inducing the SkyBox to dim #23 
* 2022-0908: 2.1.1.10 (LisiasT) for KSP >= 1.3.1 **EXPERIMENTAL**
	+ Fixes the Sky dimming for the Sun
	+ Implements some parameters to customise the dimming for planets.
	+ Work issues:
		- [#23](https://github.com/net-lisias-ksp/DistantObject/issues/23) Parameterise the FoV of the celestial body inducing the SkyBox to dim #23 
* 2022-0727: 2.1.1.9 (LisiasT) for KSP >= 1.3.1
	+ Bug hunting release.
	+ Closes issues:
		- [#16](https://github.com/net-lisias-ksp/DistantObject/issues/16) Update KSPe.Light for KSPe
		- [#15](https://github.com/net-lisias-ksp/DistantObject/issues/15) When switching to the MAP, the skybox keeps the current DOE's state!
		- [#10](https://github.com/net-lisias-ksp/DistantObject/issues/10) Prevent the body label on mouseover to be activated when there's something between...
		- [#8](https://github.com/net-lisias-ksp/DistantObject/issues/8) Labels on ALT+RMB are being drawn twice...
* 2022-0724: 2.1.1.8 (LisiasT) for KSP >= 1.3.1
	+ A memory leak was detected and fixed.
	+ Updates KSPe.Light to fix a borderline situation related to the "unreparse" stunt.
* 2022-0418: 2.1.1.7 (LisiasT) for KSP >= 1.3.1
	+ A major screwup on Mono's libraries was detected and worked around by KSPe.Light.
	+ Formalizes support for KSP 1.3.1. #HURRAY!!
* 2021-1020: 2.1.1.6 (LisiasT) for 1.4.1 <= KSP <= 1.12.2
	+ Some brain-farts of mine on handling Scene switch were fixed
		- Thanks for the [report](https://forum.kerbalspaceprogram.com/index.php?/topic/205063-145/&do=findComment&comment=4044226), [dartgently](https://forum.kerbalspaceprogram.com/index.php?/profile/204885-darthgently/)!
		- And for the [one](https://forum.kerbalspaceprogram.com/index.php?/topic/205063-145/&do=findComment&comment=4042216) from [Krazy1](https://forum.kerbalspaceprogram.com/index.php?/profile/203523-krazy1/) too! While fixing the previous, I detected what could be happening on this one and (hopefully) fixed it.
	+ Testings down to KSP 1.3.1 suggests it works on these, but
		- "Development" was done on KSP 1.4.1 and 1.4.3, and then tested against 1.7.3 and 1.12.2 and no problems (others than my own ones) were found! **#HURRAY!!**
		- KSP 1.3.1 appears to work, but I didn't "certified" it yet. Try at your own risk :) 
		- On the bottom line, the thing **runs** downto 1.3.1, but I'm not confident enough yet.
* 2021-1007: 2.1.1.5 (LisiasT) for 1.4.5 <= KSP <= 1.12.2
	+ Über refactoring
		- Creating a shareable MeshEngine
		- Decoupling PartModule details from the Engine
			- Now it can be extended by creating new DLLs, instead of recompiling the thing!
	+ Adding (preliminary) support for ReStock
		- To tell you the true, just a more thoughtfully implementation of Stock MODEL sections.   
	+ Significant performance enhancements and CPU savings
	+ New Render Mode to allow smooth transitions at the cost of memory.
		- Vessels are not removed from the cache, unless destroyed
		- May use **a lot** of memory!
	+ Option to show the names of all visible bodies
		- Use \<ALT\> while RightClicking the mouse.
* 2021-0929: 2.1.0.0 (LisiasT) for KSP >= 1.8
    + Preliminary version under Lisias' Management
    + No new features or bugfixes. Yet. ;) 
