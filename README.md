# GinsorAudioTool2Plus
Ginsor's Audio Tool 2 for Destiny, now with less bullshit!

Hello all, I have fixed Ginsor's Audio Tool 2 such that it should no longer trigger virus warnings when you download it. I have also fixed a couple of bugs that would keep databases from generating correctly on first use (making the app unusable). I have also fixed an issue that would cause the audio to crackle during playback. Also, the database files are now processed as JSON and are relatively human-readable.

Linky [here](https://github.com/ElbowBaggins/GinsorAudioTool2Plus/releases/download/2.0.1.0/GinsorAudioTool2Plus.7z)

Read on for more technical details

The virus alert was happening because the executable Ginsor distributes is not the 'real' executable. It is just a stub program that decrypts a very large (and very obvious) encrypted payload embedded within the executable, which is actually a .NET assembly (in this case a .dll file), loads this assembly, and then loads the real application out of this. Most antivirus systems *very logically* flag "big encrypted payload that gets executed" as "very bad". It is also an Activision-tier dick move meant to keep you from examining (and in this case fixing) his software.

After researching the tool a bit, this seems to be because Ginsor apparently is against releasing his own code, presumably so no one can see how actually terrible it is.

As such, I've decided it's everyone's lucky day! Simply download [dnSpy](https://github.com/0xd4d/dnSpy) as well and you can use it to open the executable file, edit the code, and recompile it to your heart's content! This is actually how I extracted the encrypted assembly. I just had the decrypt function dump it to disk during start up, opened the resulting dll in dnSpy, gave it a valid entry point, resaved as .exe, and Bob's your uncle.

I also fixed an issue where filenames (specifically the newer pkg files that have the region in the filename) could be incorrectly calculated, causing the database building stage to fail.

The crackle was occurring due to an apparent bug in WEMSharp, Ginsor's chosen Audiokinetic Wwise decoder. To get around this, I acquired the source code for the tool 'ww2ogg', rebuilt it as a library, and used *its* decoder instead. Now, the resulting Ogg isn't actually quite playable yet, (ww2ogg's author is actually very forthcoming about this, however >\_>)

To make it playable, it needs to be fixed with a 3rd party tool called "ReVorb". Ginsor was actually running this tool on his generated Oggs, (to no avail, WEMSharp makes them too broken to fix this way). This "fix" was being applied to files saved to disk but not those played in the UI, since it was having to run an external program to do the fix. I have converted this program to a library and the ReVorb stage is now handled internally, allowing the corrected stream to be played back. This also makes it so that we no longer have to include the res/revorb.exe file. Oh, and it actually works.
