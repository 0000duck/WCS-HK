using Keyence.AutoID.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFactoryApp.Common
{
    /// <summary>
    /// 基恩士智能相机
    /// </summary>
    public class KeyenceCameraHelper : IDisposable
    {
        private ReaderAccessor m_reader = new ReaderAccessor();
        private ReaderSearcher m_searcher = new ReaderSearcher();
        private LiveviewForm liveviewForm;
        private readonly int m_index = 0;

        public KeyenceCameraHelper(LiveviewForm liveviewForm, int index = 0)
        {
            this.liveviewForm = liveviewForm;
            this.m_index = index;
        }

        public delegate void delegateNewReaderData(string receivedData, int index);
        public event delegateNewReaderData NewReaderDataEvent = delegate { };
        /// <summary>
        /// 连接相机
        /// </summary>
        /// <param name="ip"></param>
        public void ConnectToCammera(string ip)
        {
            //Stop liveview.
            liveviewForm.EndReceive();
            //Set ip address of liveview.
            liveviewForm.IpAddress = ip;
            //Start liveview.
            liveviewForm.BeginReceive();
            //Set ip address of ReaderAccessor.
            m_reader.IpAddress = ip;
            //Connect TCP/IP.
            m_reader.Connect((data) =>
            {
                //Define received data actions here.Defined actions work asynchronously.
                //"ReceivedDataWrite" works when reading data was received.
                string camData = Encoding.ASCII.GetString(data);
                if (NewReaderDataEvent != null)
                {
                    NewReaderDataEvent(camData, m_index);
                }
                //this.Dispatcher.BeginInvoke(new delegateUserControl(ReceivedDataWrite), Encoding.ASCII.GetString(data));
            });
        }
        /// <summary>
        /// 执行LON命令
        /// </summary>
        public bool ExecCommandLon()
        {
            //ExecCommand("command")is for sending a command and getting a command response.
            string data = m_reader.ExecCommand("LON");
            if (NewReaderDataEvent != null)
            {
                NewReaderDataEvent(data, m_index);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 连接释放
        /// </summary>
        public void Dispose()
        {
            if (m_reader != null)
            {
                m_reader.Dispose();
            }
            if (m_searcher != null)
            {
                m_searcher.Dispose();
            }
            if (liveviewForm != null)
            {
                liveviewForm.Dispose();
            }
        }
    }
}
