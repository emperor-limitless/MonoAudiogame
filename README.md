# MonoAudiogame
## What is this?
This is a result of me intending to use monogame to create an audiogame in C# but it's now left because I found a better solution in my case, But I've realized some people might find this useful so I've decided to publish it.
### What does this have.
This was intended to be an online game, So you have a client and server, A MessageHistory system for storing messages and removing them, A key config, Finite state machine, Menus, Configuration system to save data, Audio playback without the need to manually free unused data, And a packet to function system used with C#'s reflection.
This unfortunately does not contain things like encrypting assets/server packets because I didn't reach that state of development yet so if you want to add it, Just submit a pull request, You might need to read the LiteNetLib examples to understand how to properly implement encryption with packets.
### What can I do with this?
This is released under unlicense, I.E. Public domain, You can do whatever you want with it.
## Requirements
Dotnet 7.0 or newer.
## Note.
While this says monoAudiogame using MonoGame, It doesn't entirely use MonoGame for everything, As an example, WE use [synthizer](https://github.com/synthizer/synthizer) For audio, I hope to change this but I don't have the motivation to do it so if you want just submit a pulll request and we'll see.
It also doesn't entirely do things properly, Things to change are remove the Stopwatch timer and  rely on MonoGame's GameTime to create timers instead. Perhaps better encryption.
Also, The sounds are removed, You have to add you're own sounds if you want to hear the playback.
## How to build.
Either build from vscode's debug menu or
$ dotnet run
or, IF you just want to build without running
$ dotnet build
Or, IF you intend on releasing it to the public.
$ dotnet publish -c release
