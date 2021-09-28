# Distant Object Enhancement :: Change Log

* 2015-0502: 1.5.5 (MOARdV) for KSP 1.0.2
	+ For KSP 1.0.2
		- Option to show config button only in Space Center view (Gribbleshnibit8View).
		- Labels for worlds that are not visible (such as blocked by a nearby world) no longer show up.
		- Some assorted tweaks in an effort to deal with a couple of other bugs.
		- Ghost flares should be fixed.
* 2015-0429: 1.5.4 (MOARdV) for KSP 1.0.
	+ Fix for App Launcher extra button.
* 2015-0428: 1.5.3 (MOARdV) for KSP 1.0.
	+ Maintenance release for KSP 1.0.
* 2015-0215: 1.5.2 (MOARdV) for KSP 0.90
	+ For KSP v0.90
		- Fixed flares rendering when their world is rendered (eg, Minmus and its flare rendering at the same time).
		- Internal reorganization of the flare management code to make it less costly to execute, and easier to change.
* 2014-0729: 1.3.1 (MOARdV) for KSP 0.24
	+ Patch by MOARdV
	+ 0.24 compatibility
	+ Two null reference exceptions fixed
	+ Removed System.Threading.Tasks
* 2014-0303: 1.3 (Rubber Ducky) for KSP 0.23.5 -- MIA
	+ Dynamic skybox fading
	+ Added settings GUI
	+ Vessel rendering overall should be stable now
	+ Vessel rendering now creates a database of part models and draws from there, instead of cloning the part reference object
	+ Vessel rendering no longer attempts to draw incompatible parts in many cases
	+ Probably some other minor things
* 2014-0218: 1.2 (Rubber Ducky) for KSP 0.23.5 -- MIA
	+ Planet color definitions added for Real Solar System
	+ Planet color definitions added for Real Solar System (metaphor's reconfiguration)
	+ Planet color definitions added for PlanetFactory default planets
	+ Planet color definitions added for Alternis Kerbol
	+ Fixed issue with plugin trying to render launch clamps at large distances and causing ships to explode
	+ Fixed issue with plugin incorrectly loading custom planet color definitions
	+ Added some more information to print to the console for easier debugging
	+ Added setting to easily toggle vessel rendering
	+ Vessel rendering is now disabled by default
* 2014-0217: 1.1 (Rubber Ducky) for KSP 0.23.5 -- MIA
	+ Fixed issue with plugin trying to render flags and EVA Kerbals
* 2014-0216: 1.0 (Rubber Ducky) for KSP 0.23.5 -- MIA
	+ Initial Release
