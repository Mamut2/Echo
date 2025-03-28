using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NAudio.Wave;

namespace Echo
{
    public class VoipNode : IDisposable
    {
        // Audio configuration
        private const int SampleRate = 16000;
        private const int Channels = 1;
        private const int BitDepth = 16;

        // Network components
        private UdpClient _udpClient;
        private Thread _receiveThread;

        // Audio components
        private WaveInEvent _waveIn;
        private WaveOutEvent _waveOut;
        private BufferedWaveProvider _waveProvider;

        // Peer management
        private readonly ConcurrentDictionary<(IPAddress Address, int Port), PeerState> _peers = new ConcurrentDictionary<(IPAddress Address, int Port), PeerState>();
        private readonly object _audioCaptureLock = new object();

        // State
        private bool _isRunning;
        private bool _isRecording;

        // Events
        public event Action<string> LogMessage;
        public event Action<IPAddress, int, bool> PeerConnectionChanged;

        public int LocalPort => ((IPEndPoint)_udpClient.Client.LocalEndPoint)?.Port ?? -1;

        public IEnumerable<(IPAddress Address, int Port)> ConnectedPeers =>
            _peers.Where(p => p.Value.IsConnected).Select(p => p.Key);

        private class PeerState
        {
            public DateTime LastPacketReceived { get; set; } = DateTime.UtcNow;
            public bool IsConnected { get; set; }
        }

        public VoipNode(int localPort = 0)
        {
            _udpClient = new UdpClient(localPort);
            _udpClient.EnableBroadcast = true;

            _waveProvider = new BufferedWaveProvider(new WaveFormat(SampleRate, BitDepth, Channels))
            {
                DiscardOnBufferOverflow = true,
                BufferDuration = TimeSpan.FromMilliseconds(500)
            };

            _waveOut = new WaveOutEvent();
            _waveOut.Init(_waveProvider);

            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(SampleRate, BitDepth, Channels),
                BufferMilliseconds = 50,
                NumberOfBuffers = 3
            };

            _waveIn.DataAvailable += OnAudioDataAvailable;
            _waveIn.RecordingStopped += OnRecordingStopped;
        }

        public void Start()
        {
            if (_isRunning) return;

            _isRunning = true;
            _waveOut.Play();

            _receiveThread = new Thread(ReceiveLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            _receiveThread.Start();

            Log($"VoIP node started on port {LocalPort}");
        }

        public void ConnectToPeer(IPAddress peerAddress, int peerPort)
        {
            var peerKey = (peerAddress, peerPort);
            if (_peers.ContainsKey(peerKey)) return;

            var state = new PeerState { IsConnected = true };
            if (_peers.TryAdd(peerKey, state))
            {
                SendPunchPacket(peerAddress, peerPort);
                StartAudioCaptureIfNeeded();
                PeerConnectionChanged?.Invoke(peerAddress, peerPort, true);
                Log($"Connecting to peer at {peerAddress}:{peerPort}");
            }
        }

        public void DisconnectFromPeer(IPAddress peerAddress, int peerPort)
        {
            if (_peers.TryRemove((peerAddress, peerPort), out _))
            {
                StopAudioCaptureIfNotNeeded();
                PeerConnectionChanged?.Invoke(peerAddress, peerPort, false);
                Log($"Disconnected from peer at {peerAddress}:{peerPort}");
            }
        }

        public void DisconnectAllPeers()
        {
            foreach (var peer in _peers.Keys.ToList())
            {
                DisconnectFromPeer(peer.Address, peer.Port);
            }
        }

        private void StartAudioCaptureIfNeeded()
        {
            lock (_audioCaptureLock)
            {
                if (!_isRecording && _peers.Any(p => p.Value.IsConnected))
                {
                    _waveIn.StartRecording();
                    _isRecording = true;
                    Log("Started audio capture");
                }
            }
        }

        private void StopAudioCaptureIfNotNeeded()
        {
            lock (_audioCaptureLock)
            {
                if (_isRecording && !_peers.Any(p => p.Value.IsConnected))
                {
                    _waveIn.StopRecording();
                    _isRecording = false;
                    Log("Stopped audio capture");
                }
            }
        }

        private void SendPunchPacket(IPAddress peerAddress, int peerPort)
        {
            try
            {
                var punchPacket = System.Text.Encoding.ASCII.GetBytes("VOIP_PUNCH");
                _udpClient.Send(punchPacket, punchPacket.Length, new IPEndPoint(peerAddress, peerPort));
                Log($"Sent NAT punch packet to {peerAddress}:{peerPort}");
            }
            catch (Exception ex)
            {
                Log($"Error sending punch packet to {peerAddress}:{peerPort}: {ex.Message}");
                DisconnectFromPeer(peerAddress, peerPort);
            }
        }

        private void OnAudioDataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                foreach (var peer in _peers.Where(p => p.Value.IsConnected))
                {
                    try
                    {
                        var endpoint = new IPEndPoint(peer.Key.Address, peer.Key.Port);
                        _udpClient.Send(e.Buffer, e.BytesRecorded, endpoint);
                    }
                    catch (Exception ex)
                    {
                        Log($"Error sending audio to {peer.Key.Address}:{peer.Key.Port}: {ex.Message}");
                        DisconnectFromPeer(peer.Key.Address, peer.Key.Port);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"General audio capture error: {ex.Message}");
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            _isRecording = false;
            if (e.Exception != null)
            {
                Log($"Recording stopped with error: {e.Exception.Message}");
            }
        }

        private void ReceiveLoop()
        {
            try
            {
                while (_isRunning)
                {
                    IPEndPoint remoteEp = null;
                    byte[] data = _udpClient.Receive(ref remoteEp);
                    var peerKey = (remoteEp.Address, remoteEp.Port);

                    // Update last received time
                    if (_peers.TryGetValue(peerKey, out var state))
                    {
                        state.LastPacketReceived = DateTime.UtcNow;
                    }

                    // Handle punch packet
                    if (data.Length == 9 && System.Text.Encoding.ASCII.GetString(data) == "VOIP_PUNCH")
                    {
                        Log($"Received punch packet from {remoteEp.Address}:{remoteEp.Port}");
                        if (!_peers.ContainsKey(peerKey))
                        {
                            ConnectToPeer(remoteEp.Address, remoteEp.Port);
                        }
                        continue;
                    }

                    // Handle audio data
                    _waveProvider.AddSamples(data, 0, data.Length);
                }
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
            {
                // Expected during shutdown
            }
            catch (Exception ex)
            {
                Log($"Receive error: {ex.Message}");
            }
        }

        private void Log(string message)
        {
            LogMessage?.Invoke($"[VoIPNode] {message}");
        }

        public void Dispose()
        {
            _isRunning = false;

            DisconnectAllPeers();

            _waveIn?.StopRecording();
            _waveIn?.Dispose();

            _waveOut?.Stop();
            _waveOut?.Dispose();

            _udpClient?.Close();
            _udpClient?.Dispose();

            _receiveThread?.Join();

            Log("VoIP node stopped");
        }
    }
}