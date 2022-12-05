using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Runtime.Remoting.Messaging;
using System.Reactive.Disposables;
using System.Threading;
using System.IO;

namespace Bonsai.WebSockets
{
    public class EmpaticaClient : Source<string>
    {
        public string Host { get; set; }
        public int Port { get; set; }

        public override IObservable<string> Generate()
        {
            return Generate(null);
        }

        public IObservable<string> Generate(IObservable<string> messageSequence)
        {
            return Observable.Create<string>((observer, cancellationToken) =>
            {
                var client = new TcpClient(Host, Port);
                var stream = client.GetStream();

                if (messageSequence != null)
                {
                    var messager = messageSequence.Do(message =>
                    {
                        byte[] writeBuffer = Encoding.ASCII.GetBytes(message + Environment.NewLine);
                        stream.Write(writeBuffer, 0, writeBuffer.Length);
                    }).Subscribe();

                    cancellationToken.Register(() => { messager.Dispose(); });
                }

                return Task.Factory.StartNew(() =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        if (stream.DataAvailable)
                        {
                            byte[] readBuffer = new byte[client.Available];
                            StringBuilder sb = new StringBuilder();
                            int bytesRead = stream.Read(readBuffer, 0, readBuffer.Length);
                            if (bytesRead > 0)
                            {
                                var readString = Encoding.ASCII.GetString(readBuffer, 0, bytesRead);
                                using (StringReader reader = new StringReader(readString))
                                {
                                    string line;
                                    while ((line = reader.ReadLine()) != null)
                                    {
                                        observer.OnNext(line);
                                    }
                                }
                            }
                        }
                    }
                }).ContinueWith(task =>
                {
                    client.Dispose();
                    task.Dispose();
                });
            });
        }
    }
}