# Distant Object Enhancement :: Change Log

* 2019-1125: 2.0.0.1 (TheDarkBadger) for KSP 1.8.1
	+ Just an updated settings file so the defaults are back to normal
* 2019-1112: 2.0.0.0 (TheDarkBadger) for KSP 1.8.1
	+ Updated to KSP version 1.8.1
* 2018-1016: 1.9.1.1 (MOARdV) for KSP 1.3.
	+ For KSP 1.3.x - KSP 1.6.x
		- Recompiled against KSP 1.5.0 assemblies.
		- Updated the version file.
		- NOTE: This is my last planned release of Distant Object Enhancement.  I don't have the motivation to track down the remaining bugs in the sky dimming module, and I have made no changes to this mod for over a year.  It is available for adoption by anyone who will comply with the mod's license.
* 2017-0708: 1.9.1 (MOARdV) for KSP 1.3.
	+ For KSP 1.3.x / KSP 1.4.x
		- Body mouseover names should now be localized correctly.
		- RSS flares updated, courtesy PhineasFreak.
* 2017-0526: 1.9.0 (MOARdV) for KSP 1.3.0
	+ For KSP 1.3.0
		- Recompiled for KSP 1.3.0
* 2016-1020: 1.8.1 (MOARdV) for KSP 1.2
	+ For KSP 1.2
		- Fix potential exceptions related to the Blizzy Toolbar interface (thanks to Kerbas-ad-astra).
		- De-Linq the rest of DOE.
* 2016-1011: 1.8.0 (MOARdV) for KSP 1.2
	+ For KSP 1.2
		- Updated RSS config and body names, per pull request from Theysen.
* 2016-0623: 1.7.2 (MOARdV) for KSP 1.1.
	+ For KSP 1.1.x
		- Moved settings.cfg to PluginData to play better with ModuleManager.
		- Added some try/catch to deal with unexpected missing vessels triggering exceptions.
* 2016-0428: 1.7.1 (MOARdV) for KSP 1.1.0
	+ For KSP 1.1.0
		- Fixed color conversion that made vessel flares black instead of white.  Issue #30.
* 2016-0419: 1.7.0 (MOARdV) for KSP 1.1.
	+ For KSP 1.1.0
		- Changed default planet flare saturation to 100%.  Issue #25.
		- Flares behind the sun should no longer show up in front of the sun.  Issue #26.
		- Recompiled for KSP 1.1.  Issue #27, #28.
		- Changed the way dimming is applied to flares.
		- Fixed GUI layout glitch when sliders for percentages reach 100%.
* 2015-1121: 1.6.4 (MOARdV) for KSP 1.0.5
	+ For KSP 1.0.5
		- Recompiled for KSP 1.0.5 compatibility.  Restores missing vessel flares (Issue #23).
* 2015-1015: 1.6.3 (MOARdV) for KSP 1.0.4
	+ For KSP 1.0.4
		- Fix for ghost flares appearing when flares are disabled.  Issue #21.
* 2015-1010: 1.6.2 (MOARdV) for KSP 1.0.4
	+ For KSP 1.0.4
		- Fix for a nullref exception in VesselDraw with craft that have fixed solar panels, courtesy taniwha.
* 2015-0821: 1.6.1 (MOARdV) for KSP 1.0.4
	+ For KSP 1.0.4
		- Tweaks to body flare rendering to reduce the amount of unchanging data being stored by the mod and queried every update.
		- Changed computation of a constant used for flare brightness to allow for Kerbin being a child of something other than Kerbol (for instance, with the New Horizons mod).  Courtesy forum user Tynrael.
* 2015-0723: 1.6.0 (MOARdV) for KSP 1.0.4
	+ For KSP 1.0.4
		- Finally fixed vessel flare positions.
		- Changed equation used to determine vessel flare brightness so smaller satellites will be visible.
		- Internal code changes to eliminate some redundant updates.
* 2015-0708: 1.5.7 (MOARdV) for KSP 1.0.4
	+ For KSP 1.0.4
		- NullReferenceException in FlareDraw.OnDestroy has been fixed.
		- Sky dimming has changed again. Flares are dimmed less aggressively, particularly for very low max brightness settings.
		- The flare model's texture was resized and converted to .dds. If you are installing over an existing DOE, please make sure to delete GameData/DistantObject/Flare/model000.png
* 2015-0627: 1.5.6 (MOARdV) for KSP 1.0.4
	+ For KSP 1.0.4
		- Big flares appearing for small/dim worlds is fixed.  Issue #16.
		- A few changes to hopefully reduce memory footprint when some features are not used.
		- Sky dimming has been changed: Updates are shown immediately when "Apply" is pressed. Sky dimming now affects Tracking Station and Space Center views.  Planet dimming near the sun has been tweaked.
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
