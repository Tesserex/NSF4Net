using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using Example;
using NSF4Net;

var player = new NsfPlayer(48000);
var waveSource = new NsfWaveSource(player);

var path = @"C:\junk\Code\NSFAudio\mm5.nsf";
if (path != null)
{
    player.LoadNsf(path);

    UpdateDisplay(player.Nsf);
}

var _soundOut = new WasapiOut();
_soundOut.Initialize(waveSource);
_soundOut.Play();

// respond to arrow keys
while (true)
{
    var key = Console.ReadKey(true);
    switch (key.Key)
    {
        case ConsoleKey.LeftArrow:
            if (player.Nsf != null) player.SelectSong(player.Nsf.current_song - 1);
            break;
        case ConsoleKey.RightArrow:
            if (player.Nsf != null) player.SelectSong(player.Nsf.current_song + 1);
            break;
        case ConsoleKey.UpArrow:
            waveSource.Volume += 0.1;
            break;
        case ConsoleKey.DownArrow:
            waveSource.Volume -= 0.1;
            break;
    }
    UpdateDisplay(player.Nsf);
}

void UpdateDisplay(NSF? nsf)
{
    Console.Clear();
    if (nsf is null)
    {
        Console.WriteLine("No NSF loaded");
        return;
    }
    Console.SetCursorPosition(0, 0);

    Console.WriteLine("{0} - {1}", nsf.Name, nsf.Author);
    Console.WriteLine(nsf.Copyright);
    Console.WriteLine("System: {0}", nsf.IsPal ? "PAL" : "NTSC");
    Console.WriteLine();
    Console.WriteLine("Track: {0}/{1}", nsf.current_song, nsf.num_songs);
    Console.WriteLine("Volume: {0:P0}", waveSource.Volume);

    // controls on bottom of console
    Console.SetCursorPosition(0, Console.WindowHeight - 1);
    Console.Write("Use arrows for track and volume");
}
