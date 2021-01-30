# GinsorAudioTool2Plus
Ginsor's Audio Tool 2 for Destiny, now open and improved!

![We're back!](https://thumbs.gfycat.com/VeneratedEvenArabianwildcat-size_restricted.gif)

# Onward!

Hello all, I have fixed Ginsor's Audio Tool 2 such that it should no longer trigger virus warnings when you download it. I have also fixed a couple of bugs that would keep databases from generating correctly on first use (making the app unusable). I have also fixed an issue that would cause the audio to crackle during playback. Also, the database files are now processed as JSON and are relatively human-readable.

The latest available GitHub release should reflect the contents of `master`.

Read on for more technical details

The virus alert was happening because the executable Ginsor distributes is not the 'real' executable.
It is just a stub program that decrypts a very large (and very obvious) encrypted payload embedded within the executable, which is actually a .NET assembly (in this case a .dll file), loads this assembly, and then loads the real application out of this.
Most antivirus systems *very logically* flag "big encrypted payload that gets executed" as "very bad".

This annoys me. As such, I've used [dnSpy](https://github.com/0xd4d/dnSpy) to extract the *actual* main assembly. This is fairly straightforward.
You simply open Ginsor's original tool in DnSpy and look in the entry point for calls to `Decrypt()` and `LoadAssembly()`.
My approach was to add a breakpoint on the `LoadAssembly()` line and dump the decrypted byte array to disk with the extension `.dll`. Then you just open said DLL in DnSpy, give it an entry point and convert assembly type to an executable and, just like that, it's suddenly not a virus anymore!

I also fixed an issue where filenames (specifically the newer pkg files that have the region in the filename) could be incorrectly calculated, causing the database building stage to fail.
The generated database files are now JSON instead of Ginsor's proprietary (and, weirdly, *encrypted* format)

I have additionally corrected the stuttering/crackle that would occur in basically every audio clip. The crackle was occurring due to an apparent bug in WEMSharp, Ginsor's chosen Audiokinetic Wwise decoder.
To get around this, I acquired the source code for the tool 'ww2ogg', rebuilt it as a library, and used *its* decoder instead. Now, the resulting Ogg isn't actually quite playable yet, (ww2ogg's author is actually very forthcoming about this, however >\_>)

To make it playable, it needs to be fixed with a 3rd party tool called "ReVorb".
Ginsor was actually running this tool on any clips that you *saved*, but this naturally has no effect on playback within the app.
As such, I tracked down the source code for ReVorb and have packaged it as a library alongside the tool. The cleaned up audio is now the *only* audio you'll get, which should be a more pleasant experience.

# But why the tricks?
Frankly, I don't know. This endeavor has really left me with more questions than answers.
Based on Ginsor's failed [DMCA takedown request](https://github.com/github/dmca/blob/master/2020/08/2020-08-12-GinsorAudioTool2Plus.md) I assume he feels he has some right to keep his secrets.

On some level I understand this mentality, after all, he *did* do quite a lot of the original work.
On the other hand, I personally consider his approach to be in incredibly poor taste (I know, pot, meet kettle).
There's a few reasons for my opinion.

First (and certainly not least) of which is his use of a modified (and bundled) WEMSharp build.
WEMSharp is a GPL3 library. As such, his use of a self-modified version *requires* that he release its source as well as his tool's source.
Instead, it seems he took great lengths to hide this fact from us which is *mad sketchy, yo*. Ginsor's approach also leads to an "Ivory Tower" effect where knowledge is needlessly kept only in the hands of a chosen few. (Does he want to be the only one that knows how .pkg files work or something? Very odd.)

I think this should be for everyone. It is now. I hope it stays that way.
