// See https://aka.ms/new-console-template for more information
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using NSFAudio;

var _devices = new List<MMDevice>();

using (var mmdeviceEnumerator = new MMDeviceEnumerator())
{
    using (
        var mmdeviceCollection = mmdeviceEnumerator.EnumAudioEndpoints(DataFlow.Render, DeviceState.Active))
    {
        foreach (var d in mmdeviceCollection)
        {
            _devices.Add(d);
        }
    }
}

var device = _devices[2];

var path = @"C:\junk\Code\NSFAudio\mm5.nsf";
if (path != null)
{
    var _waveSource = new NsfWaveSource(path);
    var _soundOut = new WasapiOut() { Latency = 100, Device = device };
    _soundOut.Initialize(_waveSource);
    _soundOut.Play();
}
