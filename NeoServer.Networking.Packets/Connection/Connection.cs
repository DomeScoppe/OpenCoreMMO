﻿using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using System;
using System.IO;
using System.Net.Sockets;

namespace NeoServer.Networking
{

    public class Connection : IConnection
    {
        public event EventHandler<ConnectionEventArgs> OnProcessEvent;
        public event EventHandler<ConnectionEventArgs> OnCloseEvent;
        public event EventHandler<ConnectionEventArgs> OnPostProcessEvent;

        private Socket Socket;
        private Stream Stream;

        private byte[] Buffer = new byte[16394];

        public IReadOnlyNetworkMessage InMessage { get; private set; }

        public uint[] Xtea { get; private set; }

        public void OnAccept(IAsyncResult ar)
        {
            if (ar == null)
            {
                throw new ArgumentNullException(nameof(ar));
            }

            Socket = ((TcpListener)ar.AsyncState).EndAcceptSocket(ar);
            Stream = new NetworkStream(Socket);

            BeginStreamRead();

        }

        public Connection()
        {
            Socket = null;
            Stream = null;
            Xtea = new uint[4];
            ResetBuffer();
        }
        public void BeginStreamRead() => Stream.BeginRead(Buffer, 0, 1024, OnRead, null);

        public void SetXtea(uint[] xtea)
        {
            Xtea = xtea;
        }

        public void ResetBuffer()
        {
            Buffer = new byte[16394];
        }

        private void OnRead(IAsyncResult ar)
        {
            //if (!CompleteRead(ar))
            //{
            //    return;
            //}
            if (!Stream.CanRead)
            {
                return;
            }

            int length = Stream.EndRead(ar);

            if (length == 0)
            {
                Close();
                return;
            }



            try
            {
                //Stream.Read(Buffer, 0, length);

                InMessage = new ReadOnlyNetworkMessage(Buffer);
                var eventArgs = new ConnectionEventArgs(this);
                OnProcessEvent(this, eventArgs);

            }
            catch (Exception e)
            {
                // Invalid data from the client
                // TODO: I must not swallow exceptions.
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);

                // TODO: is closing the connection really necesary?
                // Close();
            }
        }

        public void Close()
        {
            Stream.Close();
            Socket.Close();

            // Tells the subscribers of this event that this connection has been closed.
            OnCloseEvent?.Invoke(this, new ConnectionEventArgs(this));

        }

        private void SendMessage(INetworkMessage message, bool disconnect = false)
        {
            try
            {


                var streamMessage = message.GetMessageInBytes();
                Stream.BeginWrite(streamMessage, 0, streamMessage.Length, null, null);

                var eventArgs = new ConnectionEventArgs(this);
                OnPostProcessEvent(this, eventArgs);

            }
            catch (ObjectDisposedException)
            {
                Close();
            }
        }

        public void Send(IOutgoingPacket packet, bool encrypt = true)
        {

            var message = encrypt ? packet.GetMessage(Xtea) : packet.GetMessage();
            SendMessage(message, packet.Disconnect);

        }
        public void Send(string text)
        {
            var message = new TextMessagePacket(text).GetMessage(Xtea);
            SendMessage(message);
        }
    }
}