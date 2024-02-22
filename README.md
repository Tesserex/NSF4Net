# NSF4Net

NSF4Net is a headless library for playing Nintendo Sound Format (NSF) files, written entirely in .NET with no native dependencies.

## Credits

### Emulator
The emulator code is based on the My Nes emulator by Ala Ibrahim Hadid. The original source code can be found at https://github.com/bubdm/My-Nes.
Because of this, NES4Net is also licensed under the GNU General Public License (GPL) version 3.0.
The emulator code has been reduced to remove unnecessary components.

### Additional Help
NSF playback specific code is inspired by / borrowed from the NSFPlay project, originally by Brezza, and now forked and maintained by Brad Smith. The original source code can be found at https://github.com/bbbradsmith/nsfplay.

This readme file was partially written by GitHub Copilot.

## Usage

### Basic Usage
```csharp
using NSF4Net;

// Create a new NSF player, passing in the sampling rate.
var player = new NSFPlayer(48000);

// Load a file and start playing.
player.Load("file.nsf");
player.Playing = true;

// Change songs. The player will prevent out-of-bounds song selection.
player.SelectSong(2);

// Get NSF data
var name = player.Nsf.Name;
```

Playing the audio requires you to use your own audio library.
```csharp
// read samples into a buffer provided by an audio library
public int Read(byte[] buffer, int offset, int count)
{
    for (var bufPos = 0; bufPos < count;)
    {
        double sample = player.TickSample();

        // volume is controlled by the consumer
        var output = (ushort)(256 * Volume * sample);
        for (int i = 0; i < CHANNELS; i++)
        {
            buffer[bufPos++] = (byte)(output & 0xFF);
            buffer[bufPos++] = (byte)(output >> 8);
        }
    }

    return count;
}
```