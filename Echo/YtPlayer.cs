using AngleSharp.Dom.Events;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Echo
{
    public static class YtPlayer
    {
        private const int BufferSizeSeconds = 30;
        private const int ReadChunkSize = 8192 * 8;

        private static bool _disposed;
        private static bool _playbackCompleted;
        private static CancellationTokenSource _cancellationSource;

        // Audio components
        private static WaveOutEvent _waveOut;
        private static BufferedWaveProvider _bufferedProvider;
        private static Process _ffmpegProcess;

        public static event Action<string, string, byte[]> OnVideoInfoReceived;

        static YtPlayer()
        {
            Initialize();
        }

        private static void Initialize()
        {
            _cancellationSource = new CancellationTokenSource();
            _disposed = false;
            _playbackCompleted = false;

            _waveOut = new WaveOutEvent { DesiredLatency = 300 };
            _bufferedProvider = new BufferedWaveProvider(new WaveFormat(44100, 16, 2))
            {
                BufferDuration = TimeSpan.FromSeconds(BufferSizeSeconds),
                DiscardOnBufferOverflow = false
            };
            _waveOut.Init(_bufferedProvider);
        }

        public static async Task SearchAsync(string query)
        {
            var youtube = new YoutubeClient();
            var searchResults = youtube.Search.GetVideosAsync(query, _cancellationSource.Token);
            var enumerator = searchResults.GetAsyncEnumerator();
            try
            {
                while(await enumerator.MoveNextAsync())
                {
                    using(var webClient = new WebClient())
                    {
                        var result = enumerator.Current;
                        var thumbnailURL = new Uri(result.Thumbnails[0].Url.Split('?')[0]);
                        var thumbnailData = await webClient.DownloadDataTaskAsync(thumbnailURL);
                        OnVideoInfoReceived?.Invoke(result.Title, result.Url, thumbnailData);
                    }
                }
            }
            catch(Exception e) { Console.WriteLine(e.Message); }
            finally
            {
                await enumerator.DisposeAsync();
            }
        }

        public static async Task PlayAsync(string url)
        {
            if (_disposed)
            {
                Initialize();
            }

            try
            {
                var youtube = new YoutubeClient();
                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
                var audioStream = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                if (audioStream == null)
                {
                    Console.WriteLine("No suitable audio stream found.");
                    return;
                }

                _ffmpegProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-hide_banner -loglevel panic -reconnect 1 -reconnect_streamed 1 " +
                        $"-reconnect_delay_max 5 -i \"{audioStream.Url}\" -f s16le -ar 44100 -ac 2 -loglevel warning pipe:1",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                _ffmpegProcess.Start();
                _waveOut.Play();

                await ReadStream(_ffmpegProcess.StandardOutput.BaseStream, _cancellationSource.Token);

                // Wait for playback to complete
                while (_bufferedProvider != null && !_playbackCompleted &&
                       _waveOut.PlaybackState != PlaybackState.Stopped &&
                       _bufferedProvider.BufferedBytes > 0)
                {
                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                StopPlayback();
            }
        }

        private static async Task ReadStream(Stream stream, CancellationToken ct)
        {
            byte[] buffer = new byte[ReadChunkSize];
            try
            {
                while (!ct.IsCancellationRequested && !_playbackCompleted)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                    _bufferedProvider.AddSamples(buffer, 0, bytesRead);

                    if(_bufferedProvider.BufferedDuration == TimeSpan.Zero) { 
                        _playbackCompleted = true; 
                        break; 
                    }

                    while (_bufferedProvider.BufferedDuration > TimeSpan.FromSeconds(BufferSizeSeconds * 0.8) &&
                           !ct.IsCancellationRequested)
                    {
                        await Task.Delay(100, ct);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when stopping
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read error: {ex.Message}");
                StopPlayback();
            }
        }

        public static void StopPlayback()
        {
            try
            {
                _cancellationSource?.Cancel();

                if (_ffmpegProcess != null && !_ffmpegProcess.HasExited)
                {
                    _ffmpegProcess.Kill();
                    _ffmpegProcess.Dispose();
                    _ffmpegProcess = null;
                }

                _waveOut?.Stop();
                Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping player: {ex.Message}");
            }
        }

        public static void Dispose()
        {
            if (!_disposed)
            {
                _waveOut?.Dispose();
                _waveOut = null;

                if (_bufferedProvider != null)
                {
                    _bufferedProvider.ClearBuffer();
                    _bufferedProvider = null;
                }

                _disposed = true;
            }
        }
    }
}