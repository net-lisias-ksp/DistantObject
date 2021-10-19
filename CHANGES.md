# Distant Object Enhancement (DOE) :: Changes

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
