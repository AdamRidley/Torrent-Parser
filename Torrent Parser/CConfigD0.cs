using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torrent_Parser
{
    [Serializable()]
    public class CConfigDO
    {
        private string m_oLastTPBUrl;
        private bool m_oRemoteEnabled;
        private string m_oRemoteServer;
        private int m_oRemotePort;
        private string m_oRemoteUsername;
        private string m_oRemotePassword;
        private string m_oRemoteType;

        public string LastTPBUrl
        {
            get { return m_oLastTPBUrl; }
            set { m_oLastTPBUrl = value; }
        }
        public bool RemoteEnabled
        {
            get { return m_oRemoteEnabled; }
            set { m_oRemoteEnabled = value; }
        }
        public string RemoteServer
        {
            get { return m_oRemoteServer; }
            set { m_oRemoteServer = value; }
        }
        public int RemotePort
        {
            get { return m_oRemotePort; }
            set { m_oRemotePort = value; }
        }
        public string RemoteUsername
        {
            get { return m_oRemoteUsername; }
            set { m_oRemoteUsername = value; }
        }
        public string RemotePassword
        {
            get { return m_oRemotePassword; }
            set { m_oRemotePassword = value; }
        }
        public string RemoteType
        {
            get { return m_oRemoteType; }
            set { m_oRemoteType = value; }
        }
    }

    public class CConfigMng
    {
        private string m_sConfigFileName = System.IO.Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath) + ".xml";
        private CConfigDO m_oConfig = new CConfigDO();

        public CConfigDO Config
        {
            get { return m_oConfig; }
            set { m_oConfig = value; }
        }

        // Load configuration file
        public void LoadConfig()
        {
            if (System.IO.File.Exists(m_sConfigFileName))
            {
                System.IO.StreamReader srReader = System.IO.File.OpenText(m_sConfigFileName);
                Type tType = m_oConfig.GetType();
                System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                object oData = xsSerializer.Deserialize(srReader);
                m_oConfig = (CConfigDO)oData;
                srReader.Close();
            }
        }

        // Save configuration file
        public void SaveConfig()
        {
            System.IO.StreamWriter swWriter = System.IO.File.CreateText(m_sConfigFileName);
            Type tType = m_oConfig.GetType();
            if (tType.IsSerializable)
            {
                System.Xml.Serialization.XmlSerializer xsSerializer = new System.Xml.Serialization.XmlSerializer(tType);
                xsSerializer.Serialize(swWriter, m_oConfig);
                swWriter.Close();
            }
        }
    }
}
