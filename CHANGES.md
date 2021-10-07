# Distant Object Enhancement (DOE) :: Changes

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
