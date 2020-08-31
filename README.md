# GinsorAudioTool2Plus
Ginsor's Audio Tool 2 for Destiny, now with less bullshit!

![We're back!](https://thumbs.gfycat.com/VeneratedEvenArabianwildcat-size_restricted.gif)

# Copyright and License Disclaimer
The original version of Ginsor's Audio Tool links against the GPL3 library 'WEMSharp' and is sufficiently dependent on it that the tool itself is considered a "Modified Version" under the GPL3 as well:

> To "modify" a work means to copy from **or** _adapt all or part of the work_ in a fashion requiring copyright permission, other than the making of an exact copy. The resulting work is called a "modified version" of the earlier work or a work "based on" the earlier work.

It then follows that
> A "covered work" means either the unmodified Program or _a work based on the Program_
meaning that the original version of Ginsor's Audio Tool is a covered work under the GPL3. Ginsor was also _conveying_ this work.
> To "convey" a work means any kind of propagation that enables other parties to make or receive copies.

So, then, that means that Ginsor has been "conveying a covered work". Let's see what the license has to say about that.
> When you convey a covered work, **you waive any legal power to forbid circumvention of technological measures** to the extent such circumvention is effected by exercising rights under this License with respect to the covered work, and **you disclaim any intention to limit operation or modification of the work as a means of enforcing, against the work's users**, your or third parties' legal rights to forbid circumvention of technological measures.

Which means you're not allowed to tell people that they aren't allowed to reverse-engineer it.

> You may convey a work based on the Program, or the modifications to produce it from the Program, in the form of source code under the terms of section 4, provided that you also meet all of these conditions:
>
> a) The work must carry prominent notices stating that you modified it, and giving a relevant date..
>
> b) The work must carry prominent notices stating that _it is released under this License_ and any conditions added under section 7.  This requirement modifies the requirement in section 4 to "keep intact all notices".
>
> c) _You must license the entire work, **as a whole**, under this License to anyone who comes into possession of a copy_.  _This License will therefore apply, along with any applicable section 7 additional terms, **to the whole of the work, <ins>and all its parts</ins>, regardless of how they are packaged**.  This License gives **no permission to license the work in any other way**_, but it does not invalidate such permission if you have separately received it.

Essentially, the original version of the tool is so dependent on WEMSharp that it forcibly licenses the tool as GPL3 also. And since the tool itself is GPL3...

> Each time you convey a covered work, _the recipient automatically receives a license from the original licensors_, to run, **modify and propagate** that work, subject to this License.

Again, that basically says that as long as I release my code, I can do whatever I want.

Finally,
> _You may not impose **any** further restrictions on the exercise of the rights granted or affirmed under this License_.  For example, you may not impose a license fee, royalty, or other charge for exercise of rights granted under this License, and _you may not initiate litigation (including a cross-claim or counterclaim in a lawsuit) alleging that **any** patent claim is infringed by making, using, selling, offering for sale, or importing the Program or any portion of it._

Which is just more of the same.

And as a bonus

> **If you convey a covered work**, knowingly relying on a patent license, **and the Corresponding Source of the work is not available for anyone to copy, free of charge and under the terms of this License**, through a publicly available network server or other readily accessible means, then you **must** either
> (1) **cause the Corresponding Source to be so available**, or
> (2) arrange to deprive yourself of the benefit of the patent license for this particular work, or
> (3) arrange, in a manner consistent with the requirements of this License, to extend the patent license to downstream recipients.

This one really applies to me more than him. My fork is also a "covered work", and when I was just patching the app directly in dnSpy that was, technically, a GPL3 violation. The license commands me to 'cause the Corresponding Source to be so available. I think we've hit that mark.

# Onward!

Hello all, I have fixed Ginsor's Audio Tool 2 such that it should no longer trigger virus warnings when you download it. I have also fixed a couple of bugs that would keep databases from generating correctly on first use (making the app unusable). I have also fixed an issue that would cause the audio to crackle during playback. Also, the database files are now processed as JSON and are relatively human-readable.

Linky [here](https://github.com/ElbowBaggins/GinsorAudioTool2Plus/releases/download/2.0.1.0/GinsorAudioTool2Plus.7z)

Read on for more technical details

The virus alert was happening because the executable Ginsor distributes is not the 'real' executable. It is just a stub program that decrypts a very large (and very obvious) encrypted payload embedded within the executable, which is actually a .NET assembly (in this case a .dll file), loads this assembly, and then loads the real application out of this. Most antivirus systems *very logically* flag "big encrypted payload that gets executed" as "very bad". It is also an Activision-tier dick move meant to keep you from examining (and in this case fixing) his software.

After researching the tool a bit, this seems to be because Ginsor apparently is against releasing his own code, presumably so no one can see how actually terrible it is.

As such, I've decided it's everyone's lucky day! Simply download [dnSpy](https://github.com/0xd4d/dnSpy) as well and you can use it to open the executable file, edit the code, and recompile it to your heart's content! This is actually how I extracted the encrypted assembly. I just had the decrypt function dump it to disk during start up, opened the resulting dll in dnSpy, gave it a valid entry point, resaved as .exe, and Bob's your uncle.

I also fixed an issue where filenames (specifically the newer pkg files that have the region in the filename) could be incorrectly calculated, causing the database building stage to fail.

The crackle was occurring due to an apparent bug in WEMSharp, Ginsor's chosen Audiokinetic Wwise decoder. To get around this, I acquired the source code for the tool 'ww2ogg', rebuilt it as a library, and used *its* decoder instead. Now, the resulting Ogg isn't actually quite playable yet, (ww2ogg's author is actually very forthcoming about this, however >\_>)

To make it playable, it needs to be fixed with a 3rd party tool called "ReVorb". Ginsor was actually running this tool on his generated Oggs, (to no avail, WEMSharp makes them too broken to fix this way). This "fix" was being applied to files saved to disk but not those played in the UI, since it was having to run an external program to do the fix. I have converted this program to a library and the ReVorb stage is now handled internally, allowing the corrected stream to be played back. This also makes it so that we no longer have to include the res/revorb.exe file. Oh, and it actually works.
