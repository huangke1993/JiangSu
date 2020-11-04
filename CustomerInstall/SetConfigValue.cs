using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Text;

namespace CustomerInstall
{
    [RunInstaller(true)]
    public class SetConfigValue: Installer
    {
        private readonly IDictionary<string,int>_deviceType=new Dictionary<string, int>()
        {
            { "德生 TSW-M21D",0},
            { "德卡T10",1}
        }; 
        public override void Install(IDictionary stateSaver)
        {
            var deviceMac = Context.Parameters["DeviceMac"];
            var padUrl = Context.Parameters["PadUrl"];
            var tvUrl = Context.Parameters["TvUrl"];
            var deviceType = _deviceType[Context.Parameters["DeviceType"]];
            var filePath= Context.Parameters["FilePath"];
            WrictConfig(filePath,$"{{\"DeviceMac\":\"{deviceMac}\",\"PadUrl\":\"{padUrl}\",\"TvUrl\":\"{tvUrl}\",\"DeviceType\":{deviceType}}}");
            base.Install(stateSaver);
        }

        private void WrictConfig(string filePath,string content)
        {
            var contentBuffer = Encoding.UTF8.GetBytes(content);
            using (var fw = new FileStream(filePath, FileMode.Create))
            {
                fw.Write(contentBuffer,0, contentBuffer.Length);
            }
        }
    }
}
