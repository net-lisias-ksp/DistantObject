v1.7.0 (19 Apr 2016)
- Changed default planet flare saturation to 100%.  Issue #25.
- Flares behind the sun should no longer show up in front of the sun.  Issue #26.
- Recompiled for KSP 1.1.  Issue #27, #28.
- Changed the way dimming is applied to flares.
- Fixed GUI layout glitch when sliders for percentages reach 100%.

---

v1.6.4 (21 November 2015)
- Recompiled for KSP 1.0.5.  Guess 1.6.3 wasn't quite compatible after all.  Issue #23.

---

v1.6.3 (15 October 2015)
- Fix for ghost flares that appear when flare rendering is disabled.  Issue #21.

---

v1.6.2 (9 October 2015)
- Fix for a nullref exception in VesselDraw with craft that have fixed solar panels, courtesy taniwha.

---

v1.6.1 (21 August 2015)
- Tweaks to body flare rendering to reduce the amount of unchanging data being stored by the mod and queried every update.
- Changed computation of a constant used for flare brightness to allow for Kerbin being a child of something other than Kerbol (for instance, with the New Horizons mod).  Courtesy forum user Tynrael.

---

v1.6.0 (July 23, 2015)
- Finally fixed vessel flare positions.
- Changed equation used to determine vessel flare brightness so smaller satellites will be visible.
- Internal code changes to eliminate some redundant updates.

---

v1.5.7 (July 8, 2015)
- NullReferenceException in FlareDraw.OnDestroy has been fixed.
- Sky dimming has changed again.  Flares are dimmed less aggressively, particularly for very low max brightness settings.
- The flare model's texture was resized and converted to .dds.  If you are installing over an existing DOE, please make sure to delete GameData/DistantObject/Flare/model000.png

---

v1.5.6 (June 27, 2015)
- Big flares appearing for small/dim worlds is fixed.  Issue #16.
- A few changes to hopefully reduce memory footprint when some features are not used.
- Sky dimming has been changed: Updates are shown immediately when "Apply" is pressed. Sky dimming now affects Tracking Station and Space Center views.  Planet dimming near the sun has been tweaked.

---

v1.5.5 (May 2, 2015)
- Option to show config button only in Space Center view (Gribbleshnibit8View).
- Labels for worlds that are not visible (such as blocked by a nearby world) no longer show up.
- Some assorted tweaks in an effort to deal with a couple of other bugs.
- Ghost flares should be fixed.

---

v1.5.4 (April 29, 2015)
- Fix the App Launcher extra icon bug (Issue #12)

---

v1.5.3 (April 27, 2015)
- KSP 1.0 compatibility release

---

v1.5.2 (February 15, 2015)
- Fixed flares rendering when their world is rendered (eg, Minmus and its flare rendering at the same time).
- Internal reorganization of the flare management code to make it less costly to execute, and easier to change.

---

v1.5.1 (December 21, 2014)
- Removed requirement for blizzy's Toolbar being installed.

---

v1.5.0 (December 21, 2014)
- Disable flare rendering in MapView, since flare positions are completely wrong.
- Add support for stock KSP Toolbar (AppLauncher).

---

v1.4.2 (December 11, 2014)
- Move vessel flares back to camera layer 0 to reduce their displacement as a temporary workaround for Issue #3 (MOARdV)
- Fix flare drawing so it works with the CactEye telescope mod (Raven45)

---

v1.4.1
- Fix skybox max brightness not being read (Issue #1)

----

v1.4 (November 2, 2014) Distant Object Enhancement bis

- 0.25 compatibility
- Fix potential NULL reference exceptions when vessels are destroyed fixed (Anatid)
- Ability to show labels on flares (planets and visible vessels) (Anatid)
- Moved flares to a different rendering layer so they're not affected by lighting illuminating the vessel, such as the PlanetShine mod (Valerian)
- Tweaked GUI (MOARdV)
- Enabled GUI in Flight in addition to Space Center view (some changes still going back to the Space Center before they take effect) (MOARdV)
- Improved handling of missing config file or missing config file entries (MOARdV)


----

v1.3.1 (July 29, 2014)

Patch by MOARdV

- 0.24 compatibility
- Two null reference exceptions fixed
- Removed System.Threading.Tasks

----

v1.3 (March 3, 2014)

- Dynamic skybox fading
- Added settings GUI
- Vessel rendering overall should be stable now
- Vessel rendering now creates a database of part models and draws from there, instead of cloning the part reference object
- Vessel rendering no longer attempts to draw incompatible parts in many cases
- Probably some other minor things

----

v1.2 (February 18, 2014)

- Planet color definitions added for Real Solar System
- Planet color definitions added for Real Solar System (metaphor's reconfiguration)
- Planet color definitions added for PlanetFactory default planets
- Planet color definitions added for Alternis Kerbol
- Fixed issue with plugin trying to render launch clamps at large distances and causing ships to explode
- Fixed issue with plugin incorrectly loading custom planet color definitions
- Added some more information to print to the console for easier debugging
- Added setting to easily toggle vessel rendering
- Vessel rendering is now disabled by default

----

v1.1 (February 17, 2014)

- Fixed issue with plugin trying to render flags and EVA Kerbals

----

v1.0 (February 16, 2014)

- Initial Release
