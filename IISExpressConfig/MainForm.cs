using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Net;

namespace IISExpressConfig
{
    public partial class MainForm : Form
    {

        public string configPath
        {
            get
            {
                string root = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                return string.Format("{0}\\IISExpress\\config\\applicationhost.config",root);
            }
        }


      
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

            //MessageBox.Show(configPath);
            //检测配置文件是否存在
            if (!File.Exists(configPath))
            {
                MessageBox.Show("IISExpress 未安装或路径不正确");
            }


            LoadConfig(configPath, listBox);


        }

        static string GetLocalIp()  
        {  
            string hostname = Dns.GetHostName();//得到本机名    
            IPHostEntry localhost = Dns.GetHostEntry(hostname);  
            IPAddress localaddr = localhost.AddressList[1];  
            return localaddr.ToString();  
        }

        public void LoadConfig(string configPath,ListBox listBox)
        {
            XmlDocument Doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader xmlReader = XmlReader.Create(configPath, settings);

            Doc.Load(xmlReader);

            XmlNode Root = Doc.DocumentElement;

            XmlNodeList nodeList = Root.SelectNodes("system.applicationHost/sites/site");

          

            foreach (XmlNode node in nodeList)
            {
                string siteName = node.Attributes["name"].Value;
                XmlNodeList bindings = node.SelectNodes("bindings/binding");

                bool canVisit = bindings.Count > 1;

                listBox.Items.Add(string.Format("{0}[{1}]",siteName,canVisit));
            }

            xmlReader.Dispose();
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            XmlDocument Doc = new XmlDocument();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            XmlReader xmlReader = XmlReader.Create(configPath, settings);

            Doc.Load(xmlReader);
            xmlReader.Dispose();

            XmlNode Root = Doc.DocumentElement;

            string Ip = GetLocalIp();
            XmlNodeList nodeList = Root.SelectNodes("system.applicationHost/sites/site");
            XmlNode node = nodeList.Item(listBox.SelectedIndex);
            XmlNodeList bindings = node.SelectNodes("bindings/binding");

            if (bindings.Count > 1)
            {
                MessageBox.Show("该端口已经绑定");
                return;
            }

            XmlNode bindfirst = bindings.Item(0);
            string port = bindfirst.Attributes["bindingInformation"].Value.Split(':')[1];

            XmlElement xnode = Doc.CreateElement("binding");
            xnode.SetAttribute("protocol", "http");
            xnode.SetAttribute("bindingInformation", string.Format(":{1}:{0}",Ip,port));

            XmlNode b = node.SelectSingleNode("bindings");
            b.AppendChild(xnode);

            Doc.Save(configPath);

            listBox.Items.Clear();
            LoadConfig(configPath, listBox);
            
        }
    }
}
